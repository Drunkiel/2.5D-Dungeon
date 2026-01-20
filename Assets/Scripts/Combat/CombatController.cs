using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;

    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator CastSkill(SkillDataParser _skillDataParser, CollisionController _collisionController, CombatUI _combatUI)
    {
        if (_collisionController.targets.Count <= 0)
            yield break;

        if (_collisionController.targets.Count > 0 && _collisionController.targets[0] == null)
        {
            _collisionController.targets.RemoveAt(0);
            yield break;
        }

        //Get caster
        Transform casterTransform = _collisionController.transform.parent.parent.parent;
        EntityController _entityController = casterTransform.GetComponent<EntityController>();
        EntityStatistics _casterStatistics = _entityController._statistics;

        if (_casterStatistics == null)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Caster is unknown: {_collisionController.transform.parent.parent.parent.name}", OutputType.Error);
            yield break;
        }

        //Check if entity has enough mana to cast
        float manaUsage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });
        if (_casterStatistics.mana * _casterStatistics.manaUsageMultiplier < manaUsage)
        {
            ConsoleController.instance.ChatMessage(SenderType.Hidden, "Not enough mana to cast spell", OutputType.Warning);
            yield break;
        }

        //Play animation
        string animName = string.IsNullOrEmpty(_skillDataParser._skillData.animationName) ? "TakeDamage" : _skillDataParser._skillData.animationName;
        if (_entityController != null)
        {
            PlayAnimation(_entityController.anim, animName);
            if (_skillDataParser._skillData.allowedDistance != 0 && TryGetComponent(out EntityWalk _entityWalk))
                _entityWalk.allowedDistance = _skillDataParser._skillData.allowedDistance;
            _entityController.GetComponent<EntityCombat>().ManageCombat(casterTransform);
        }

        //Display skill effects
        EffectPlayer _effectPlayer = _collisionController.GetComponent<EffectPlayer>();
        PlayAnimation(_effectPlayer.anim, _effectPlayer.animationName);
        _effectPlayer.PlayParticle();

        //Check if entity needs to be stopped
        if (_skillDataParser._skillData.stopMovement)
            SetMovementState(_entityController, true);

        //Delay cast
        yield return new WaitForSeconds(_skillDataParser._skillData.delay);

        switch (_skillDataParser._skillData.type)
        {
            case SkillType.AttackMelee:
                AttackSkill(_skillDataParser, _collisionController, casterTransform, _casterStatistics, _combatUI);
                break;

            case SkillType.AttackRange:
                Projectile _projectile = _collisionController.transform.GetChild(0).GetComponentInParent<Projectile>();
                _projectile.SetData(_skillDataParser, _casterStatistics, casterTransform, _combatUI, _collisionController.targets, _collisionController.entityTags);
                break;

            case SkillType.Defence:
                BuffSkill(_skillDataParser, _collisionController, _casterStatistics, _combatUI);
                break;
        }

        _casterStatistics.TakeMana(manaUsage);
        if (_skillDataParser._skillData.stopMovement)
            SetMovementState(_entityController, false);
    }

    private void SetMovementState(EntityController _entityController, bool state)
    {
        _entityController.StopEntity(state);
    }

    public bool AttackSkill(SkillDataParser _skillDataParser, CollisionController _collisionController, Transform casterTransform, EntityStatistics _casterStatistics, CombatUI _combatUI)
    {
        //Get current target
        List<EntityStatistics> _targetsStatistics = new();
        List<EntityController> _enemyTargets = new();

        if (_collisionController.targets.Count <= 0)
            return false;

        foreach (GameObject target in _collisionController.targets)
        {
            if (target == null)
                return false;

            if (target.TryGetComponent(out EntityController _enemyComponent))
            {
                _targetsStatistics.Add(_enemyComponent._statistics);
                _enemyTargets.Add(_enemyComponent);
                _enemyComponent.GetComponent<EntityCombat>().ManageCombat(casterTransform);
            }
            else
            {
                ConsoleController.instance.ChatMessage(SenderType.System, $"Target: {target.name} does not have a controller script attached", OutputType.Error);
                return false;
            }
        }

        //Get stats
        float skillDamage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });

        //Checks what type of damage to deal
        Attributes _attributes = _skillDataParser._skillData._skillAttributes[0];
        int damageToDeal = _attributes.attributeType switch
        {
            AttributeTypes.MeleeDamage => _casterStatistics.meleeDamage,
            AttributeTypes.RangeDamage => _casterStatistics.rangeDamage,
            AttributeTypes.MagicDamage => _casterStatistics.magicDamage,
            _ => 0
        };

        for (int i = 0; i < _targetsStatistics.Count; i++)
        {
            _targetsStatistics[i].TakeDamage((skillDamage + damageToDeal) * _casterStatistics.damageMultiplier, _attributes.attributeType, _attributes.elementalTypes);
            PlayAnimation(_enemyTargets[i].anim, "TakeDamage");

            //If entity is killed check if is in the quest
            if (_enemyTargets[i]._statistics.health <= 0)
                QuestController.instance.InvokeKillEvent(_enemyTargets[i]._entityInfo.ID);
        }

        return true;
    }

    private void BuffSkill(SkillDataParser _skillDataParser, CollisionController _collisionController, EntityStatistics _casterStatistics, CombatUI _combatUI)
    {
        //Set buff for caster
        _casterStatistics._activeBuffs.Add(new Buff(
            _skillDataParser._skillData.displayedName,
            _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }),
            _skillDataParser.iconSprite,
            _combatUI.GetBuff(_skillDataParser),
            (int)_combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Buff })
        ));

        _casterStatistics.RecalculateStatistics(GameController.instance._player._holdingController._itemController._gearHolder);

        if (_skillDataParser._skillData.worksOnOthers)
        {
            for (int i = _skillDataParser._skillData.worksOnSelf ? 1 : 0; i < _collisionController.targets.Count; i++)
            {
                EntityStatistics _targetStatistics = _collisionController.targets[i].GetComponent<EntityController>()._statistics;

                _targetStatistics._activeBuffs.Add(new Buff(
                    _skillDataParser._skillData.displayedName,
                    _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }),
                    _skillDataParser.iconSprite,
                    _combatUI.GetBuff(_skillDataParser),
                    (int)_combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Buff })
                ));

                _targetStatistics.RecalculateStatistics(GameController.instance._player._holdingController._itemController._gearHolder);
            }
        }
    }

    public float PlayAnimation(Animator animator, string animationName)
    {
        if (string.IsNullOrEmpty(animationName))
            return 0f;

        // Play the animation and return how long animation takes
        animator.Play(animationName);
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
}