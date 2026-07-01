using System.Collections.Generic;
using UnityEngine;

public class DialogInteraction : MonoBehaviour
{
    public DialogGraph _graph;

    public void StartDialog()
    {
        //Checks if mission is assigned for that NPC
        if (CheckFoMission())
            return;

        //If not then start normal dialog IF there is any
        if (_graph == null)
            return;
            
        DialogController.instance._graph = _graph;
        DialogController.instance.StartDialog();
    }

    private bool CheckFoMission()
    {
        short entityID = GetComponent<EntityController>()._entityInfo.ID;
        QuestController _questController = QuestController.instance;

        for (int i = 0; i < _questController.talkQuestIndexes.Count; i++)
        {
            List<RequirementData> _requirementsData = _questController._allQuests[_questController.talkQuestIndexes[i]]._requirements;
            for (int j = 0; j < _requirementsData.Count; j++)
            {
                if (_requirementsData[j].type != RequirementType.Talk)
                    continue;
                
                if (_requirementsData[j].targetID == entityID)
                {
                    _questController.InvokeTalkEvent(entityID);
                    return true;
                }
            }
        }

        return false;
    }
}
