using UnityEngine;

public class PickInteraction : MonoBehaviour
{
    public ItemID _itemID;

    private void Start()
    {
        GetComponent<EventTriggerController>().enterEvent.AddListener(() => ComparisonController.instance.MakeComparison(_itemID));
    }

    public void Pick()
    {
        if (!PlayerController.instance._holdingController.PickItem(_itemID))
            return;

        GetComponent<EventTriggerController>().canBeShown = false;
        GetComponent<HintEvent>().ChangeHint(0);
    }
}
