using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private readonly int countOfSlots = 24;
    public List<InventorySlot> _slots = new();
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    private void Start()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            InventorySlot slot = newSlot.GetComponent<InventorySlot>();
            slot.slotID = i;
            _slots.Add(slot);
        }

        instance = this;
    }

    public void AddToInventory(ItemID _itemID)
    {
        int availableSlot = GetAvailableSlotIndex();
        if (availableSlot == -1)
            return;

        _slots[availableSlot]._itemID = _itemID;
    }

    public int GetAvailableSlotIndex()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            if (_slots[i]._itemID == null)
            {
                print(i);
                return i;
            }
        }

        return -1;
    }
}
