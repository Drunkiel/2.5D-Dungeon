using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public bool inCombat;

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
        int manaUsage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });
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
        int skillDamage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });

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
        _casterStatistics._activeBuffs.Add(new("Ala", 5f, Buffs.MaxSpeed, 100));
        _casterStatistics.RecalculateStatistics(PlayerController.instance._holdingController._itemController._gearHolder);
    }

    private void EnemyAttack()
    {
        EntityStatistics _enemyStatistics = CombatEntities.instance.enemy.GetComponent<EnemyController>()._statistics;
        EntityStatistics _playerStatistics = PlayerController.instance._statistics;
        SkillHolder _skillHolder = CombatEntities.instance.enemy.GetComponent<EnemyController>()._holdingController._skillsController._skillHolder;

        //Random pick enemy skill
        List<int> usableSkills = new();
        for (int i = 0; i < _skillHolder._skillDatas.Count; i++)
        {
            if (_skillHolder._skillDatas[i] != null)
                usableSkills.Add(i);
        }
        SkillData _skillData = _skillHolder._skillDatas[usableSkills[UnityEngine.Random.Range(0, usableSkills.Count)]]._skillData;

        int skillDamage = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
        int protection = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
        int manaUsage = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.ManaUsage });

        //Checks if enemy has enough mana to cast skill
        if (_enemyStatistics.mana * _enemyStatistics.manaUsageMultiplier < manaUsage)
        {
            print($"Not enough mana: {Mathf.Abs(_enemyStatistics.mana - manaUsage)}");
            return;
        }

        //Do stuff
        Attributes _attributes = _skillData._skillAttributes[0];
        int enemyDamage = 0;
        switch (_attributes.attributeType)
        {
            case AttributeTypes.MeleeDamage:
                enemyDamage = _enemyStatistics.meleeDamage;
                break;

            case AttributeTypes.RangeDamage:
                enemyDamage = _enemyStatistics.rangeDamage;
                break;

            case AttributeTypes.MagicDamage:
                enemyDamage = _enemyStatistics.magicDamage;
                break;
        }

        _playerStatistics.TakeDamage((skillDamage + enemyDamage) * _enemyStatistics.damageMultiplier, _attributes.attributeType, _attributes.elementalTypes);
        PlayAnimation(PlayerController.instance.anim, "TakeDamage");
        _enemyStatistics.TakeMana(manaUsage);
    }

    private IEnumerator PlayAnimationAndWait(CombatEntities _combatEntities, Animator animator, string animationName, Action action)
    {
        EntityStatistics _playerStatistics = PlayerController.instance._statistics;
        EntityStatistics _enemyStatistics = _combatEntities.enemy.GetComponent<EnemyController>()._statistics;

        // Wait until the animation is done
        yield return new WaitForSeconds(PlayAnimation(animator, animationName));

        //Taking action
        action();
        UpdateSliders(_playerStatistics, _enemyStatistics, false);

        //Checks
        //if (_playerStatistics.health <= 0 || 
        //    _enemyStatistics.health <= 0)
        //{
        //    EndCombat();
        //    yield return null; 
        //}
    }

    public float PlayAnimation(Animator animator, string animationName)
    {
        // Play the animation and return how long animation takes
        animator.Play(animationName);
        return animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private void UpdateSliders(EntityStatistics _playerStatistics, EntityStatistics _enemyStatistics, bool firstLoad)
    {
        _combatUI._playerStats.UpdateHealthSlider((float)_playerStatistics.health / _playerStatistics.maxHealth, true, firstLoad);
        _combatUI._playerStats.UpdateManaSlider((float)_playerStatistics.mana / _playerStatistics.maxMana, true, firstLoad);

        _combatUI._enemyStats.UpdateHealthSlider((float)_enemyStatistics.health / _enemyStatistics.maxHealth, true, firstLoad);
        _combatUI._enemyStats.UpdateManaSlider((float)_enemyStatistics.mana / _enemyStatistics.maxMana, true, firstLoad);
    }
}
