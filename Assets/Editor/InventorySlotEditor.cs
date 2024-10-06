#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventorySlot))]
public class InventorySlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InventorySlot script = (InventorySlot)target;

        //Saving changes
        Undo.RecordObject(script, "Modified InventorySlot");

        DrawDefaultInspector();

        //Draw the EnumFlagsField for itemRestriction
        script.itemRestriction = (ItemType)EditorGUILayout.EnumFlagsField("Item Restriction", script.itemRestriction);

        //Show the WeaponType array if the Weapon flag is set
        if (script.itemRestriction.HasFlag(ItemType.Weapon))
        {
            EditorGUILayout.LabelField("Weapon Types");

            //Draw the array for WeaponType
            if (script.weaponTypes != null)
            {
                for (int i = 0; i < script.weaponTypes.Length; i++)
                    script.weaponTypes[i] = (WeaponType)EditorGUILayout.EnumPopup($"Weapon Type {i}", script.weaponTypes[i]);
            }

            //Add button to allow adding new WeaponType elements
            if (GUILayout.Button("Add Weapon Type"))
            {
                //Resize the array and add a new element
                int newSize = (script.weaponTypes != null) ? script.weaponTypes.Length + 1 : 1;
                System.Array.Resize(ref script.weaponTypes, newSize);
                script.weaponTypes[newSize - 1] = WeaponType.Sword; // Default new entry to a valid enum value
            }

            //Add button to allow removing the last WeaponType element
            if (script.weaponTypes != null && script.weaponTypes.Length > 0 && GUILayout.Button("Remove Weapon Type"))
            {
                //Resize the array to remove the last element
                int newSize = script.weaponTypes.Length - 1;
                System.Array.Resize(ref script.weaponTypes, newSize);
            }
        }

        //Show the ArmorType field if the Armor flag is set
        if (script.itemRestriction.HasFlag(ItemType.Armor))
            script.armorType = (ArmorType)EditorGUILayout.EnumPopup("Armor Type", script.armorType);

        if (GUI.changed)
            EditorUtility.SetDirty(script);
    }
}
#endif
