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

        //Determine who is caster
        Transform casterTransform = _collisionController.transform.parent.parent.parent;
        PlayerController _player = casterTransform.GetComponent<PlayerController>();
        EntityController _entityController = _player == null ? casterTransform.GetComponent<EntityController>() : null;

        EntityStatistics _casterStatistics = _player != null ? _player._statistics :
                                            _entityController != null ? _entityController._statistics : null;

        if (_casterStatistics == null)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Caster is unknown: {_collisionController.transform.parent.parent.parent.name}", OutputType.Error);
            yield break;
        }

        float manaUsage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });
        if (_casterStatistics.mana * _casterStatistics.manaUsageMultiplier < manaUsage)
        {
            ConsoleController.instance.ChatMessage(SenderType.Hidden, "Not enough mana to cast spell", OutputType.Warning);
            yield break;
        }

        string animName = string.IsNullOrEmpty(_skillDataParser._skillData.animationName) ? "TakeDamage" : _skillDataParser._skillData.animationName;
        if (_player != null)
        {
            PlayAnimation(_player.anim, animName);
            _player.GetComponent<EntityCombat>().ManageCombat(casterTransform);
        }
        else if (_entityController != null)
        {
            PlayAnimation(_entityController.anim, animName);
            if (_skillDataParser._skillData.allowedDistance != 0)
                _entityController._entityWalk.allowedDistance = _skillDataParser._skillData.allowedDistance;
            _entityController.GetComponent<EntityCombat>().ManageCombat(casterTransform);
        }

        //Display skill effects
        EffectPlayer _effectPlayer = _collisionController.GetComponent<EffectPlayer>();
        PlayAnimation(_effectPlayer.anim, _effectPlayer.animationName);
        _effectPlayer.PlayParticle();

        //Check if entity needs to be stopped
        if (_skillDataParser._skillData.stopMovement)
            SetMovementState(_player, _entityController, true);

        //Delay cast
        yield return new WaitForSeconds(_skillDataParser._skillData.delay);

        switch (_skillDataParser._skillData.type)
        {
            case SkillType.AttackMelee:
                AttackSkill(_skillDataParser, _collisionController, casterTransform, _casterStatistics, _combatUI);
                break;

            case SkillType.AttackRange:
                Projectile _projectile = _collisionController.transform.GetChild(0).GetComponentInParent<Projectile>();
                _projectile.SetData(_skillDataParser, _casterStatistics, casterTransform, _combatUI, _collisionController.entityTags);
                break;

            case SkillType.Defence:
                BuffSkill(_skillDataParser, _casterStatistics, _combatUI);
                break;
        }

        _casterStatistics.TakeMana(manaUsage);
        if (_skillDataParser._skillData.stopMovement)
            SetMovementState(_player, _entityController, false);
    }

    private void SetMovementState(PlayerController _player, EntityController _enemyController, bool state)
    {
        if (_enemyController == null)
            _player.isStopped = state;
        else
            _enemyController._entityWalk.isStopped = state;
    }

    public bool AttackSkill(SkillDataParser _skillDataParser, CollisionController _collisionController, Transform casterTransform, EntityStatistics _casterStatistics, CombatUI _combatUI)
    {
        //Get current target
        List<EntityStatistics> _targetsStatistics = new();
        List<EntityController> _enemyTargets = new();
        PlayerController _playerTarget = null;

        if (_collisionController.targets.Count <= 0)
            return false;

        foreach (GameObject target in _collisionController.targets)
        {
            if (target.TryGetComponent(out EntityController _enemyComponent))
            {
                _targetsStatistics.Add(_enemyComponent._statistics);
                _enemyTargets.Add(_enemyComponent);
                _enemyComponent.GetComponent<EntityCombat>().ManageCombat(casterTransform);
            }
            else if (target.TryGetComponent(out PlayerController _playerComponent))
            {
                _targetsStatistics.Add(_playerComponent._statistics);
                _playerTarget = _playerComponent;
                _playerTarget.GetComponent<EntityCombat>().ManageCombat(casterTransform);
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
            if (_playerTarget != null)
                PlayAnimation(_playerTarget.anim, "TakeDamage");
            else
                PlayAnimation(_enemyTargets[i].anim, "TakeDamage");
        }

        return true;
    }

    private void BuffSkill(SkillDataParser _skillDataParser, EntityStatistics _casterStatistics, CombatUI _combatUI)
    {
        _casterStatistics._activeBuffs.Add(new Buff(
            _skillDataParser._skillData.displayedName,
            _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }),
            _skillDataParser.iconSprite,
            _combatUI.GetBuff(_skillDataParser),
            (int)_combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Buff })
        ));

        _casterStatistics.RecalculateStatistics(PlayerController.instance._holdingController._itemController._gearHolder);
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