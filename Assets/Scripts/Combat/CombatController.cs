using System;
using System.Collections;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    public static CombatController instance;
    public bool inCombat;
    [SerializeField] private bool isPlayerTurn;
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
        //Do some animation stuff

        //Taking action
        action();

        //Checks
        CombatEntities _combatEntities = CombatEntities.instance;
        if (_combatEntities.player.GetComponent<PlayerController>()._statistics.health <= 0 || 
            _combatEntities.enemy.GetComponent<EnemyController>()._statistics.health <= 0)
        {
            EndCombat();
            return;
        }

        //Ending turn
        isPlayerTurn = !isPlayerTurn;

        if (!isPlayerTurn)
            TakeTurn(() => 
            {
                EntityStatistics _enemyStatistics = CombatEntities.instance.enemy.GetComponent<EnemyController>()._statistics;
                EntityStatistics _playerStatistics = PlayerController.instance._statistics;

                SkillHolder _skillHolder = CombatEntities.instance.enemy.GetComponent<EnemyController>()._holdingController._skillsController._skillHolder;
                SkillData _skillData = _skillHolder._skillDatas[UnityEngine.Random.Range(0, _skillHolder._skillDatas.Count)];
                
                int skillDamage = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.MeleeDamage, AttributeTypes.RangeDamage, AttributeTypes.MagicDamage });
                int protection = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.AllProtection, AttributeTypes.MeleeProtection, AttributeTypes.RangeProtection, AttributeTypes.MagicProtection });
                int manaUsage = _combatUI.GetSkillModifier(_skillData, new() { AttributeTypes.ManaUsage });

                //Checks if player has enough mana to cast skill
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
            });
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    IEnumerator WaitAndLoadScene()
    {
        //Some effects before starting battle
        PlayerController.instance.isStopped = true;
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
            _combatUI.SetSkillToBTN(i, PlayerController.instance._holdingController._skillsController._skillHolder._skillDatas[i]);

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
}
