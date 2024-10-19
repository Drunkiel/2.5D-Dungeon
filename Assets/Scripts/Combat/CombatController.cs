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

    public void CastSkill(SkillDataParser _skillDataParser, AttackController _attackController)
    {
        if (_attackController.targets.Count <= 0)
            return;

        EnemyController _enemyController = _attackController.targets[0].transform.GetComponent<EnemyController>();
        EntityStatistics _enemyStatistics = _enemyController._statistics;
        EntityStatistics _playerStatistics = PlayerController.instance._statistics;

        //Get stats
        int skillDamage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
        int protection = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
        int manaUsage = _combatUI.GetSkillModifier(_skillDataParser._skillData, new() { AttributeTypes.ManaUsage });

        //Checks if player has enough mana to cast skill
        if (_playerStatistics.mana * _playerStatistics.manaUsageMultiplier < manaUsage)
        {
            print($"Not enough mana: {Mathf.Abs(_playerStatistics.mana - manaUsage)}");
            return;
        }

        //Checks what type of damage to deal
        Attributes _attributes = _skillDataParser._skillData._skillAttributes[0];
        int damageToDeal = 0;
        switch (_attributes.attributeType)
        {
            case AttributeTypes.MeleeDamage:
                damageToDeal = _playerStatistics.meleeDamage;
                break;

            case AttributeTypes.RangeDamage:
                damageToDeal = _playerStatistics.rangeDamage;
                break;

            case AttributeTypes.MagicDamage:
                damageToDeal = _playerStatistics.magicDamage;
                break;
        }

        _enemyStatistics.TakeDamage((skillDamage + damageToDeal) * _playerStatistics.damageMultiplier, _attributes.attributeType, _attributes.elementalTypes);
        PlayAnimation(_enemyController.anim, "TakeDamage");
        _playerStatistics.TakeMana(manaUsage);
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
