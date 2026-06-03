using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public static ShopController instance;

    [SerializeField] private Transform shopUiParent;
    [SerializeField] private GameObject shopUiPrefab;
    [SerializeField] private GameObject slotPrefab;

    public ShopListData testList;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        CreateShop(testList);
    }

    public void CreateShop(ShopListData _listData)
    {
        ShopUI _shopUI = Instantiate(shopUiPrefab, shopUiParent).GetComponent<ShopUI>();

        for (int i = 0; i < _listData._shopItems.Count; i++)
        {
            InventorySlot _slot = Instantiate(slotPrefab, _shopUI.slotParent).GetComponent<InventorySlot>();
            DragDropSlot _dragDropSlot = Instantiate(_slot.itemPlacePrefab, _slot.transform).GetComponent<DragDropSlot>();
            _dragDropSlot.currentSlot = _slot;
            _dragDropSlot.lockedUp = true;

            ItemID _itemCopy = Instantiate(ItemContainer.instance.GetItemByID(_listData._shopItems[i].itemID), _dragDropSlot.transform).GetComponent<ItemID>();
            _slot._itemID = _itemCopy;
            int a = i;
            _dragDropSlot.image.sprite = _itemCopy.GetSprite();
            _dragDropSlot.GetComponent<Button>().onClick.AddListener(() => _shopUI.UpdateDisplay(_itemCopy, _listData._shopItems[a].price));

            if (i == 0)
                _shopUI.UpdateDisplay(_itemCopy, _listData._shopItems[a].price);
        }
    }

    public void BuyItem(ItemID _itemID)
    {
        int availableSlotIndex = InventoryController.instance.GetAvailableSlotIndex();
        if (availableSlotIndex == -1)
            return;

        InventoryController.instance.AddToInventory(ItemContainer.instance.GetItemByID(_itemID._itemData.ID), availableSlotIndex);
    }
}
