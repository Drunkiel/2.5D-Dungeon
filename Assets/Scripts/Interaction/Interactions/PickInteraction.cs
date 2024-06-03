using UnityEngine;

public class PickInteraction : MonoBehaviour
{
    public ItemID _itemID;

    public void Pick()
    {
        HoldingController.instance.PickItem(_itemID);
        GetComponent<EventTriggerController>().canBeShown = false;
        GetComponent<HintEvent>().ChangeHint(0);
    }
}
