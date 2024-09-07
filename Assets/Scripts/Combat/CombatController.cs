using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public bool inCombat;
    [SerializeField] private bool isPlayerTurn;
    private bool playerTookAction;
    public Transform playerPlace;
    public Transform enemyPlace;
    public Transform cameraLookPoint;
    public CombatUI _combatUI;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Awake()
    {
        instance = this;
    }

    public void StartCombat()
    {
        CombatEntities _combatEntities = CombatEntities.instance;
        PlayerController _playerController = PlayerController.instance;

        _playerController._statistics.RecalculateStatistics(_playerController._holdingController._itemController._gearHolder);
        _combatEntities.playerPreviousPosition = _playerController.transform.position;
        _combatEntities.playerXScale = _playerController.transform.localScale.x;
        isPlayerTurn = true;
        inCombat = true;

        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndSetForCombat());
    }

    public void EndCombat()
    {
        StartCoroutine(WaitAndLoadScene());
        StartCoroutine(WaitAndReset());
        inCombat = false;
    }

    public void TakeTurn(Action action)
    {
        if (!inCombat || (playerTookAction && isPlayerTurn))
            return;

        CombatEntities _combatEntities = CombatEntities.instance;

        // Start the Coroutine to handle animation and wait for it to finish
        if (isPlayerTurn)
        {
            playerTookAction = true;
            StartCoroutine(PlayAnimationAndWait(_combatEntities, _combatEntities.player.GetComponent<PlayerController>().anim, "Test_CastingSpell", action));
        }
        else
        {
            playerTookAction = false;
            StartCoroutine(PlayAnimationAndWait(_combatEntities, _combatEntities.enemy.GetComponent<EnemyController>().anim, "Test_CastingSpell", action));
        }
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    IEnumerator WaitAndLoadScene()
    {
        //Some effects before starting battle
        PlayerController.instance.isStopped = true;
        CombatEntities.instance.enemy.GetComponent<EnemyController>().isStopped = true;
        StartCoroutine(CameraController.instance.ZoomTo(20, 1f));

        yield return new WaitForSeconds(1);

        //Starting transition
        TransitionController.instance.StartTransition(1);

        yield return new WaitForSeconds(1);
    }

    IEnumerator WaitAndSetForCombat()
    {
        //Set player skills
        for (int i = 0; i < 6; i++)
        {
            SkillDataParser _skillDataParser = PlayerController.instance._holdingController._skillsController._skillHolder._skillDatas[i];
            if (_skillDataParser != null)
            {
                _combatUI.SetSkillToBTN(i, _skillDataParser);
                _combatUI.skillButtons[i].transform.GetChild(0).gameObject.SetActive(true);
                _combatUI.skillButtons[i].transform.GetChild(1).gameObject.SetActive(false);
                _combatUI.skillButtons[i].interactable = true;
            }
            else
            {
                _combatUI.skillButtons[i].transform.GetChild(0).gameObject.SetActive(false);
                _combatUI.skillButtons[i].transform.GetChild(1).gameObject.SetActive(true);
                _combatUI.skillButtons[i].interactable = false;
            }
        }

        yield return new WaitForSeconds(2);

        //Setting player to combat state
        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = playerPlace.position;
        _combatEntities.player.transform.GetChild(0).localScale = new(1, 1, 1);

        //Setting enemy to combat state
        _combatEntities.enemy.transform.position = enemyPlace.position;
        _combatEntities.enemy.transform.GetChild(0).localScale = new(-1, 1, 1);
        _combatEntities.enemy.GetComponent<EnemyController>().isStopped = true;

        //Setting camera to combat state
        CameraController.instance.ResetZoom();
        CameraController.instance.SetCamera(1);
        _openCloseUI.Open();
    }

    IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(2);

        //Resetting player to previous state
        CombatEntities _combatEntities = CombatEntities.instance;
        _combatEntities.player.transform.position = _combatEntities.playerPreviousPosition;
        _combatEntities.player.transform.GetChild(0).localScale = new(_combatEntities.playerXScale, 1, 1);
        PlayerController.instance.ResetMovement();
        PlayerController.instance.isStopped = false;

        //Destroying enemy
        Destroy(_combatEntities.enemy);

        //Resetting camera and UI
        CameraController.instance.ResetZoom();
        CameraController.instance.SetCamera(0);
        _openCloseUI.Close();
    }

    private IEnumerator PlayAnimationAndWait(CombatEntities _combatEntities, Animator animator, string animationName, Action action)
    {
        // Play the animation
        animator.Play(animationName);

        // Wait until the animation is done
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        //Taking action
        action();

        //Checks
        if (_combatEntities.player.GetComponent<PlayerController>()._statistics.health <= 0 || 
            _combatEntities.enemy.GetComponent<EnemyController>()._statistics.health <= 0)
        {
            EndCombat();
            yield return null; 
        }

        //Ending turn
        isPlayerTurn = !isPlayerTurn;

        if (!isPlayerTurn)
            TakeTurn(EnemyTurn);
    }

    private void EnemyTurn()
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
        _enemyStatistics.TakeMana(manaUsage);
    }
}
