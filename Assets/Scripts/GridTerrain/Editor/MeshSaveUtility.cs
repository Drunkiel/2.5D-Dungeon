#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class MeshSaveUtility
{
    public static Mesh SaveMesh(
        Mesh mesh,
        string assetPath,
        bool overwrite = true)
    {
        if (mesh == null)
            return null;

        //Copy of mesh
        Mesh meshCopy = Object.Instantiate(mesh);
        meshCopy.name = System.IO.Path.GetFileNameWithoutExtension(assetPath);

        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        if (existing != null)
        {
            if (!overwrite)
                return existing;

            AssetDatabase.DeleteAsset(assetPath);
        }

        AssetDatabase.CreateAsset(meshCopy, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return meshCopy;
    }
}
#endif
