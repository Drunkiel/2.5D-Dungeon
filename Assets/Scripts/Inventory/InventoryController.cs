using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    public QuickInventoryController _quickInventory;

    public List<InventorySlot> _gearSlots = new();
    public List<InventorySlot> _inventorySlots = new();
    public List<InventorySlot> _skillSlots = new();
    [SerializeField] private GameObject slotPrefab;
    public Transform slotParent;

    public EntityPreview _entityPreview;
    public Sprite sprite;

    public bool isMovingItem;
    [SerializeField] private OpenCloseUI _inventoryUI;
    [SerializeField] private OpenCloseUI _playerEqUI;
    [SerializeField] private OpenCloseUI _skillsUI;

    private void Start()
    {
        EntityLookController _lookController = GameController.instance._player.GetComponent<EntityLookController>();
        _entityPreview.UpdateAllByEntity(_lookController._entityLook, _lookController._spriteHolder, GameController.instance._player._itemController._gearHolder);

        instance = this;
    }

    public void ManageInventory(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!GameController.instance._player.isStopped && !GameController.isPaused && !isMovingItem)
            _inventoryUI.OpenClose();
    }

    public void ManageSkills(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!GameController.instance._player.isStopped && !GameController.isPaused && !isMovingItem)
            _skillsUI.OpenClose();
    }

    public void ManagePlayerEQ(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!GameController.instance._player.isStopped && !GameController.isPaused && !isMovingItem)
            _playerEqUI.OpenClose();
    }

    public void OpenSelectedUI(int index)
    {
        switch (index)
        {
            case 0:
                _inventoryUI.Open();
                break;

            case 1:
                _playerEqUI.Open();
                break;

            case 2:
                _skillsUI.Open();
                break;
        }
    }

    public void AddToInventory(ItemID _itemID, int slotIndex)
    {
        if (slotIndex == -1)
            return;

        _inventorySlots[slotIndex]._itemID = _itemID;
        DragDropSlot slot = Instantiate(_inventorySlots[slotIndex].itemPlacePrefab, _inventorySlots[slotIndex].transform).GetComponent<DragDropSlot>();
        slot.currentSlot = _inventorySlots[slotIndex];
        slot.image.sprite = _itemID.GetSprite();
        _inventorySlots[slotIndex]._itemID.transform.SetParent(slot.transform, false);

        QuestController.instance.InvokeCollectEvent(_itemID._itemData.ID);
    }

    public void LoadToGearInventory(ItemID _itemID, int slotIndex)
    {
        GameObject slot = Instantiate(_gearSlots[slotIndex].itemPlacePrefab, _gearSlots[slotIndex].transform);
        ItemController _itemController = GameController.instance._player._itemController;

        slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemID.GetSprite();
        switch (_itemID._itemData.itemType)
        {
            case ItemType.Weapon:
                _itemController.SetWeapon(_itemID);
                break;

            case ItemType.Armor:
                _itemController.SetArmor(_itemID);
                break;
        }

        ItemID _itemCopy = Instantiate(_itemID.gameObject, slot.transform).GetComponent<ItemID>();
        slot.transform.parent.GetComponent<InventorySlot>()._itemID = _itemCopy;
        slot.GetComponent<DragDropSlot>().currentSlot = _gearSlots[slotIndex];
    }

    public void LoadToInventory(ItemID _itemID, int slotIndex)
    {
        GameObject slot = Instantiate(_inventorySlots[slotIndex].itemPlacePrefab, _inventorySlots[slotIndex].transform);
        slot.transform.GetChild(0).GetComponent<Image>().sprite = _itemID.GetSprite();

        ItemID _itemCopy = Instantiate(_itemID.gameObject, slot.transform).GetComponent<ItemID>();
        slot.transform.parent.GetComponent<InventorySlot>()._itemID = _itemCopy;
        slot.GetComponent<DragDropSlot>().currentSlot = _inventorySlots[slotIndex];
    }

    public void LoadToSkillInventory(SkillDataParser _skillDataParser, int slotIndex)
    {
        GameObject slot = Instantiate(_skillSlots[slotIndex].itemPlacePrefab, _skillSlots[slotIndex].transform);
        ItemID _itemID = slot.transform.GetChild(1).GetComponent<ItemID>();
        _itemID._skillDataParser = _skillDataParser;
        SkillController _skillController = GameController.instance._player._skillsController;
        _skillController._skillHolder.skillsID.Add(_itemID._skillDataParser._skillData.ID);

        slot.transform.GetChild(1).GetComponent<Image>().sprite = _itemID._skillDataParser.iconSprite;
        slot.transform.parent.GetComponent<InventorySlot>()._itemID = _itemID;
        slot.GetComponent<DragDropSlot>().currentSlot = _skillSlots[slotIndex];

        _skillController.SetUpSkill(_skillDataParser, slotIndex);
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
        for (int i = 0; i < _inventorySlots.Count; i++)
        {
            if (_inventorySlots[i]._itemID == null)
                return i;
        }

        return -1;
    }
}
