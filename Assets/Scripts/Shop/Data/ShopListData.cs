using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItem
{
    public short itemID;
    public int price;
}

[CreateAssetMenu(menuName = "Shop/Shop List")]
public class ShopListData : ScriptableObject
{
    public List<ShopItem> _shopItems;
}
