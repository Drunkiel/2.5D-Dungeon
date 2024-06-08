using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private readonly int countOfSlots = 24;
    public List<InventorySlot> _slots = new();
    [SerializeField] private GameObject slotPrefab;
    public Transform slotParent;

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

    public void AddToInventory(ItemID _itemID, int slotIndex)
    {
        if (slotIndex == -1)
            return;

        _slots[slotIndex]._itemID = _itemID;
        GameObject slot = Instantiate(_slots[slotIndex].itemPlacePrefab, _slots[slotIndex].transform);
        _slots[slotIndex]._itemID.transform.SetParent(slot.transform, false);
        slot.GetComponent<DragDrop>().image.sprite = _itemID._collectableItem._itemData.itemSprite;
    }

    public int GetAvailableSlotIndex()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            if (_slots[i]._itemID == null)
                return i;
        }

        return -1;
    }
}
