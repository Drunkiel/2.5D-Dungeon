using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private readonly int countOfSlots = 24;
    public List<InventorySlot> _gearSlots = new();
    public List<InventorySlot> _inventorySlots = new();
    [SerializeField] private GameObject slotPrefab;
    public Transform slotParent;

    public EntityPreview _entityPreview;
    public Sprite sprite;

    public bool isMovingItem;
    [SerializeField] private OpenCloseUI _openCloseUI;

    private void Start()
    {
        for (int i = 0; i < countOfSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            InventorySlot slot = newSlot.GetComponent<InventorySlot>();
            slot.slotID = i;
            _inventorySlots.Add(slot);
        }

        _entityPreview.UpdateAllByEntity(PlayerController.instance.GetComponent<EntityLookController>()._entityLook, PlayerController.instance._holdingController._itemController._gearHolder);

        instance = this;
    }

    public void ManageInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!PlayerController.instance.isStopped && !GameController.isPaused && !isMovingItem)
            _openCloseUI.OpenClose();
    }

    public void AddToInventory(ItemID _itemID, int slotIndex)
    {
        if (slotIndex == -1)
            return;

        _inventorySlots[slotIndex]._itemID = _itemID;
        GameObject slot = Instantiate(_inventorySlots[slotIndex].itemPlacePrefab, _inventorySlots[slotIndex].transform);
        _inventorySlots[slotIndex]._itemID.transform.SetParent(slot.transform, false);

        switch (_itemID._itemData.itemType)
        {
            case ItemType.Weapon:
                slot.GetComponent<DragDropSlot>().image.sprite = _itemID._weaponItem.iconSprite;
                break;

            case ItemType.Armor:
                slot.GetComponent<DragDropSlot>().image.sprite = _itemID._armorItem.iconSprite;
                break;

            case ItemType.Collectable:
                slot.GetComponent<DragDropSlot>().image.sprite = _itemID._collectableItem.iconSprite;
                break;
        }

        QuestController.instance.InvokeCollectEvent(_itemID._itemData.ID);
    }

    public void AddToGearInventory(ItemID _itemID, int slotIndex)
    {
        GameObject slot = Instantiate(_gearSlots[slotIndex].itemPlacePrefab, _gearSlots[slotIndex].transform);
        ItemController _itemController = PlayerController.instance._holdingController._itemController;

        switch (_itemID._itemData.itemType)
        {
            case ItemType.Weapon:
                slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemID._weaponItem.iconSprite;

                if (!_itemController.CanPickWeapon(_itemID._weaponItem.holdingType))
                    _itemController.ReplaceItem(_itemID);

                _itemController.SetWeapon(_itemID);
                break;
            
            case ItemType.Armor:
                slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemID._armorItem.iconSprite;

                if (!_itemController.CanPickArmor(_itemID._armorItem.armorType))
                    _itemController.ReplaceItem(_itemID);

                _itemController.SetArmor(_itemID);
                break;
        }
    }

    public void RemoveItemByID(int itemID)
    {
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i]._itemID == null)
                continue;

            if (_inventorySlots[i]._itemID._itemData.ID == itemID)
            {
                Destroy(_inventorySlots[i]._itemID.transform.parent.gameObject);
                return;
            }
        }

        ConsoleController.instance.ChatMessage(SenderType.System, $"Item: <color=red>{itemID}</color> is not found in the inventory", OutputType.Error);
    }

    public void DeleteGearInventory(int slotIndex)
    {
        if (_gearSlots[slotIndex].transform.childCount <= 0)
            return;

        GameObject item = _gearSlots[slotIndex].transform.GetChild(1).gameObject;
        Destroy(item);
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
