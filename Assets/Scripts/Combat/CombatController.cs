using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public bool inCombat;
    public float timeToResetCombat;

    public CombatUI _combatUI;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void CastSkill(SkillDataParser _skillDataParser, CollisionController _collisionController)
    {
        if (_collisionController.targets.Count <= 0)
            return;

        //Get caster
        EntityStatistics _casterStatistics = null;
        EnemyController _enemyController = null;
        //Check if caster is player
        if (_collisionController.transform.parent.parent.parent.TryGetComponent(out PlayerController _player))
            _casterStatistics = _player._statistics;
        //If not then check if enemy
        else if (_collisionController.transform.parent.parent.parent.TryGetComponent(out EnemyController _enemy))
        {
            _casterStatistics = _enemy._statistics;
            _enemyController = _enemy;
        }

        //If still null then return
        if (_casterStatistics == null)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"Caster is unknown: {_collisionController.transform.parent.parent.parent.name}", OutputType.Error);
            return;
        }

        //Checks if player has enough mana to cast skill
        float manaUsage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });
        if (_casterStatistics.mana * _casterStatistics.manaUsageMultiplier < manaUsage)
        {
            ConsoleController.instance.ChatMessage(SenderType.Hidden, "Not enough mana to cast spell", OutputType.Warning);
            return;
        }

        //Check if animation exists
        string animName = _skillDataParser._skillData.animationName;
        if (string.IsNullOrEmpty(animName))
            animName = "TakeDamage";

        //Play animation
        if (_player != null)
            PlayAnimation(_player.anim, animName);
        else if (_enemyController != null)
            PlayAnimation(_enemyController.anim, animName);


        switch (_skillDataParser._skillData.type)
        {
            case SkillType.Attack:
                AttackSkill(_skillDataParser, _collisionController, _casterStatistics);
                break;

            case SkillType.Defence:
                BuffSkill(_skillDataParser, _casterStatistics);
                break;
        }

        if (!inCombat)
        {
            StartCoroutine(ResetCombat());
            inCombat = true;
        }
        else
            timeToResetCombat = 0;

        _casterStatistics.TakeMana(manaUsage);
    }

    private void AttackSkill(SkillDataParser _skillDataParser, CollisionController _collisionController, EntityStatistics _casterStatistics)
    {
        //Get current target
        List<EntityStatistics> _targetsStatistics = new();
        List<EnemyController> _enemyTargets = new();
        PlayerController _playerTarget = null;
        foreach (GameObject target in _collisionController.targets)
        {
            if (target.TryGetComponent(out EnemyController _enemyComponent))
            {
                _targetsStatistics.Add(_enemyComponent._statistics);
                _enemyTargets.Add(_enemyComponent);
            }
            else if (target.TryGetComponent(out PlayerController _playerComponent))
            {
                _targetsStatistics.Add(_playerComponent._statistics);
                _playerTarget = _playerComponent;
            }
            else
                ConsoleController.instance.ChatMessage(SenderType.System, $"Target: {target.name} does not have a controller script attached", OutputType.Error);
        }

        //Get stats
        float skillDamage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });

        //Checks what type of damage to deal
        Attributes _attributes = _skillDataParser._skillData._skillAttributes[0];
        int damageToDeal = 0;
        switch (_attributes.attributeType)
        {
            case AttributeTypes.MeleeDamage:
                damageToDeal = _casterStatistics.meleeDamage;
                break;

            case AttributeTypes.RangeDamage:
                damageToDeal = _casterStatistics.rangeDamage;
                break;

            case AttributeTypes.MagicDamage:
                damageToDeal = _casterStatistics.magicDamage;
                break;
        }

        for (int i = 0; i < _targetsStatistics.Count; i++)
        {
            _targetsStatistics[i].TakeDamage((skillDamage + damageToDeal) * _casterStatistics.damageMultiplier, _attributes.attributeType, _attributes.elementalTypes);
            if (_playerTarget != null)
                PlayAnimation(_playerTarget.anim, "TakeDamage");
            else
                PlayAnimation(_enemyTargets[i].anim, "TakeDamage");
        }
    }

    private void BuffSkill(SkillDataParser _skillDataParser, EntityStatistics _casterStatistics)
    {
        _casterStatistics._activeBuffs.Add(new(_skillDataParser._skillData.displayedName, _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Cooldown }), _combatUI.GetBuff(_skillDataParser), (int)_combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.Buff })));
        _casterStatistics.RecalculateStatistics(PlayerController.instance._holdingController._itemController._gearHolder);
    }

    public float PlayAnimation(Animator animator, string animationName)
    {
        // Play the animation and return how long animation takes
        animator.Play(animationName);
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private IEnumerator ResetCombat()
    {
        timeToResetCombat += Time.deltaTime;

        //Wait 5s to reset combat
        yield return new WaitUntil(() => timeToResetCombat < 5);

        inCombat = false;
    }
}
