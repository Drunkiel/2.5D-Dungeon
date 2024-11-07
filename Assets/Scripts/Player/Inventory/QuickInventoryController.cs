using System.Collections.Generic;
using UnityEngine;

public class QuickInventoryController : MonoBehaviour
{
    [SerializeField] private bool lockedUp;
    public List<InventorySlot> _inventorySlots = new();

    private void Start()
    {
        ChangeLockState();
    }

    public void ChangeLockState()
    {
        lockedUp = !lockedUp;
        foreach (InventorySlot _slot in _inventorySlots)
        {
            if (_slot._itemID == null)
                continue;

            if (_slot._itemID._itemData.itemType == ItemType.Spell)
                _slot._itemID.transform.parent.GetComponent<DragDropSlot>().lockedUp = lockedUp;
        }
    }
}
