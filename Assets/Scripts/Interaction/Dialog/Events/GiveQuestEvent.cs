using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dialog/Events/Give Quest Event")]
public class GiveQuestEvent : DialogEvent
{
    [SerializeField] private QuestData _questData;

    public override void Execute()
    {
        QuestController.instance.GiveQuest(_questData.id - 1);
    }
}
