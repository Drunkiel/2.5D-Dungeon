using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogGraph))]
public class DialogGraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Open Graph"))
            DialogGraphWindow.Open(target as DialogGraph);
    }
}