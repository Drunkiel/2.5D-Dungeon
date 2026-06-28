using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dialog/Events/Shop Event")]
public class ShopEvent : DialogEvent
{
    [SerializeField] private ShopListData _listData;
    [SerializeField] private bool openInventory;
    [SerializeField] private bool openEQ;
    [SerializeField] private bool openSkills;

    public override void Execute()
    {
        ShopController.instance.CreateShop(_listData);

        if (openInventory)
            InventoryController.instance.OpenSelectedUI(0);

        if (openEQ)
            InventoryController.instance.OpenSelectedUI(1);
        
        if(openSkills)
            InventoryController.instance.OpenSelectedUI(2);
    }
}
