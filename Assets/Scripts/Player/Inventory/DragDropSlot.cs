using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropSlot : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public InventorySlot currentSlot;
    public Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryController.instance.isMovingItem = true;

        if (currentSlot._itemID != null && currentSlot.itemRestriction != ItemType.None)
        {
            GearHolder _gearHolder = PlayerController.instance._holdingController._itemController._gearHolder;

            if (currentSlot._itemID._armorItem != null)
            {
                //Find armor piece
                ArmorItem _armorItem = _gearHolder.GetHoldingArmor(currentSlot._itemID._armorItem.armorType);

                //If not null then destroy
                if (_armorItem != null)
                {
                    //Destroy armor piece
                    Destroy(_armorItem);
                    switch (currentSlot._itemID._armorItem.armorType)
                    {
                        case ArmorType.Helmet:
                            Destroy(_gearHolder.headTransform.GetChild(0).gameObject);
                            break;
                        case ArmorType.Chestplate:
                            Destroy(_gearHolder.bodyTransform.GetChild(0).gameObject);
                            break;
                        case ArmorType.Boots:
                            Destroy(_gearHolder.leftFeetTransform.GetChild(1).gameObject);
                            Destroy(_gearHolder.rightFeetTransform.GetChild(1).gameObject);
                            break;
                    }
                }

                //Update inventory preview
                InventoryController.instance._entityPreview.UpdateArmorLook(currentSlot._itemID._armorItem.armorType, InventoryController.instance.sprite);
            }

            if (currentSlot._itemID._weaponItem != null)
            {
                //Find weapon
                WeaponItem _weaponItem = _gearHolder.GetHoldingWeapon(currentSlot._itemID._weaponItem.holdingType);

                //Destroy current holding weapon
                if (_weaponItem != null)
                    Destroy(_weaponItem.gameObject);
            }
        }

        currentSlot._itemID = null;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / 1.5f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ItemID _itemID = transform.GetChild(1).GetComponent<ItemID>();

        //Check if pointer is over any UI element
        if (!EventSystem.current.gameObject.TryGetComponent(out InventorySlot _inventorySlot) || _itemID._itemData.itemType != _inventorySlot.itemRestriction)
        {
            GearHolder _gearHolder = PlayerController.instance._holdingController._itemController._gearHolder;

            //Checks if there is any armor equipped
            if (_itemID._armorItem != null && _gearHolder.GetHoldingArmor(transform.GetChild(1).GetComponent<ItemID>()._armorItem.armorType) == null)
                currentSlot.OnDrop(eventData);

            //Checks if there is any weapon equipped
            if (_itemID._weaponItem != null && _gearHolder.GetHoldingWeapon(transform.GetChild(1).GetComponent<ItemID>()._weaponItem.holdingType) == null)
                currentSlot.OnDrop(eventData);
        }

        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
        currentSlot._itemID = transform.GetChild(1).GetComponent<ItemID>();
        InventoryController.instance.isMovingItem = false;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentSlot == null)
            currentSlot = eventData.pointerEnter.transform.parent.parent.GetComponent<InventorySlot>();

        if (eventData.button == PointerEventData.InputButton.Right)
            ComparisonController.instance.MakeComparison(currentSlot._itemID);

        rectTransform.SetParent(InventoryController.instance.slotParent.parent);
    }

    public void OnDrop(PointerEventData eventData)
    {
        rectTransform.SetParent(currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}