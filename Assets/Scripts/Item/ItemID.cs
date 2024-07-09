using UnityEngine;

public enum ItemBuffs
{
    NoBuff,
    DamageMultiplier,
    ProtectionMultiplier,
    ManaReductionMultiplier,
}

[System.Serializable]
public class ItemBuff
{
    public ItemBuffs itemBuffs;
    [Range(0, 2)]
    public float amount = 1;
}

public class ItemID : MonoBehaviour
{
    public ItemType itemType;
    public WeaponItem _weaponItem;
    public ArmorItem _armorItem;
    public CollectableItem _collectableItem;
}
