using UnityEngine;

public class ComparisonController : MonoBehaviour
{
    public static ComparisonController instance;
    [SerializeField] private Transform parent;

    [SerializeField] private ComparisonItem _comparisonItemPrefab;

    private void Awake()
    {
        instance = this;
    }

    public void MakeComparison(ItemID _otherItemID)
    {
        ComparisonItem _comparisonItem = Instantiate(_comparisonItemPrefab, parent);

        _comparisonItem._itemID = _otherItemID;

        //Checks if item exists
        if (_comparisonItem._itemID == null)
            return;

        _comparisonItem.OverrideData();
    }
}
