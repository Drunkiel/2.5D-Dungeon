using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityLookController))]
public class EntityLookControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EntityLookController controller = (EntityLookController)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Skin In Editor"))
        {
            controller.ApplySkinInEditor();
        }
    }
}