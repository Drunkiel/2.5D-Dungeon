#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GridTerrainComponent : MonoBehaviour
{
    public GridTerrainData data;

#if UNITY_EDITOR
    public void SaveAsAsset(string folder = "Assets/GridTerrains")
    {
        var meshComp = GetComponent<GridTerrainMesh>();
        if (meshComp == null)
        {
            Debug.LogError("No GridTerrainMesh found");
            return;
        }

        Mesh runtimeMesh = meshComp.GetMesh();
        if (runtimeMesh == null)
        {
            Debug.LogError("No mesh to save");
            return;
        }

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets", "GridTerrains");
        }

        string meshPath = $"{folder}/{name}_Mesh.asset";

        Mesh meshCopy = Instantiate(runtimeMesh);
        meshCopy.name = name + "_Mesh";

        AssetDatabase.CreateAsset(meshCopy, meshPath);

        // Material
        var renderer = GetComponent<MeshRenderer>();
        if (renderer && renderer.sharedMaterial)
        {
            string matPath = $"{folder}/{name}_Material.mat";

            Material matCopy = Instantiate(renderer.sharedMaterial);
            AssetDatabase.CreateAsset(matCopy, matPath);

            renderer.sharedMaterial = matCopy;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Terrain saved as mesh asset");
    }
#endif
}
