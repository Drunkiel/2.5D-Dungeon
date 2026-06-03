using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Events/Shop Event")]
public class ShopEvent : DialogEvent
{
    [SerializeField] private ShopListData _listData;

    public override void Execute()
    {
        ShopController.instance.CreateShop(_listData);
    }
}
