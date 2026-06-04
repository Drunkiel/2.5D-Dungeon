using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public TMP_Text shopTitleText;

    public Image itemImage;
    public TMP_Text itemTitleText;
    public TMP_Text itemPriceText;

    public Transform slotParent;

    private ItemID currentlyPickedItem;

    public void UpdateDisplay(ItemID _itemID, int price)
    {
        itemImage.sprite = _itemID.GetSprite();
        itemTitleText.text = _itemID._itemData.displayedName;
        itemPriceText.text = $"{price}";
        currentlyPickedItem = _itemID;
    }

    public void BuyItem()
    {
        if (currentlyPickedItem == null)
            return;

        ShopController.instance.BuyItem(currentlyPickedItem);
    }

    public void CloseUI()
    {
        GetComponent<AutoDestroy>().InstantDestroy();
        GameController.instance._player.StopEntity(false);
    }
}
