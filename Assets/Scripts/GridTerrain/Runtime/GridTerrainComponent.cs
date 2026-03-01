#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GridTerrainComponent : MonoBehaviour
{
    public GridTerrainData data;
    Mesh runtimeMesh;

#if UNITY_EDITOR
    public void SaveAsAsset()
    {
        if (data == null || data.asset == null)
            return;

        MeshFilter mf = GetComponent<MeshFilter>();

        if (mf.sharedMesh == null)
            return;

        string folderPath =
            System.IO.Path.GetDirectoryName(
                AssetDatabase.GetAssetPath(data.asset)
            );

        string meshAssetPath =
            $"{folderPath}/{data.asset.name.Replace("Data", "Mesh")}.asset";

        Mesh meshAsset =
            AssetDatabase.LoadAssetAtPath<Mesh>(meshAssetPath);

        if (meshAsset == null)
        {
            Debug.LogError("Mesh asset not found!");
            return;
        }

        meshAsset.Clear();
        meshAsset.vertices = mf.sharedMesh.vertices;
        meshAsset.triangles = mf.sharedMesh.triangles;
        meshAsset.uv = mf.sharedMesh.uv;
        meshAsset.normals = mf.sharedMesh.normals;

        EditorUtility.SetDirty(meshAsset);
        AssetDatabase.SaveAssets();

        Debug.Log("Mesh saved to asset.");
    }

    public void SetMesh(Mesh assetMesh)
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        runtimeMesh = new Mesh
        {
            name = assetMesh.name + "_Runtime"
        };

        mf.sharedMesh = runtimeMesh;
    }
#endif
}
