using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;
    public ItemID _itemID;
    public GameObject itemPlacePrefab;
    [HideInInspector] public ItemType itemRestriction;
    [HideInInspector] public WeaponType[] weaponTypes;
    [HideInInspector] public ArmorType armorType;

    public void OnDrop(PointerEventData eventData)
    {
        if (itemRestriction != ItemType.None && eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>()._itemData.itemType != itemRestriction)
            return;

        switch (itemRestriction)
        {
            case ItemType.Weapon:
                bool isFound = false;
                for (int i = 0; i < weaponTypes.Length; i++)
                {
                    if (eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>()._weaponItem.weaponType == weaponTypes[i])
                    {
                        isFound = true;
                        break;
                    }
                }

                //If weapon is not correct then return
                if (!isFound)
                    return;

                break;

            case ItemType.Armor:
                if (eventData.pointerDrag.transform.GetChild(1).GetComponent<ItemID>()._armorItem.armorType != armorType)
                    return;
                break;
        }

        RectTransform rectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        rectTransform.GetComponent<DragDrop>().currentSlot = this;
        rectTransform.SetParent(rectTransform.GetComponent<DragDrop>().currentSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }
}
