using UnityEngine;

public class PickInteraction : MonoBehaviour
{
    public ItemID _itemID;

    public void Pick()
    {
        if (!GameController.instance._player._holdingController._itemController.PickItem(_itemID))
            return;

        GetComponent<EventTriggerController>().canBeShown = false;
        GetComponent<HintEvent>().ChangeHint(0);
    }
}
