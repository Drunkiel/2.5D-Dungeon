using UnityEngine;

public class StandController : MonoBehaviour
{
    public string itemName;
    public ItemType itemType;
    public float parentPositionY;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private PickInteraction _pickInteraction;

    void Start()
    {
        parentTransform.localPosition = new(0, parentPositionY, 0);
        ItemID _itemCopy = ItemContainer.instance.GetItemByNameAndType(itemName, itemType);
        _itemCopy.transform.parent = parentTransform.GetChild(0);

        if (_itemCopy == null)
        {
            ConsoleController.instance.ChatMessage(SenderType.System, $"There is no item with name: {itemName} and type: {itemType}");
            return;
        }

        _itemCopy.transform.localPosition = Vector3.zero;
        _itemCopy.transform.localRotation = Quaternion.identity;
        _pickInteraction._itemID = _itemCopy;
    }
}
