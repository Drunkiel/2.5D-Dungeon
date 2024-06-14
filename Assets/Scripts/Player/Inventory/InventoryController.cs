using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private readonly int countOfSlots = 24;
    public List<InventorySlot> _gearSlots = new();
    public List<InventorySlot> _inventorySlots = new();
    [SerializeField] private GameObject slotPrefab;
    public Transform slotParent;

    private void Start()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            InventorySlot slot = newSlot.GetComponent<InventorySlot>();
            slot.slotID = i;
            _inventorySlots.Add(slot);
        }

        instance = this;
    }

    public void AddToInventory(ItemID _itemID, int slotIndex)
    {
        if (slotIndex == -1)
            return;

        _inventorySlots[slotIndex]._itemID = _itemID;
        GameObject slot = Instantiate(_inventorySlots[slotIndex].itemPlacePrefab, _inventorySlots[slotIndex].transform);
        _inventorySlots[slotIndex]._itemID.transform.SetParent(slot.transform, false);
        slot.GetComponent<DragDrop>().image.sprite = _itemID._collectableItem._itemData.itemSprite;
    }

    public void AddToGearInventory(ItemData _itemData, int slotIndex)
    {
        GameObject slot = Instantiate(_gearSlots[slotIndex].itemPlacePrefab, _gearSlots[slotIndex].transform);
        slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemData.itemSprite;
    }

    public int GetAvailableSlotIndex()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            if (_inventorySlots[i]._itemID == null)
                return i;
        }

        return -1;
    }
}
