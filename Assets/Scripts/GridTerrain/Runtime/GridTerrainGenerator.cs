using UnityEngine;

[ExecuteAlways]
public class GridTerrainGenerator : MonoBehaviour
{
    public GridTerrainData data;
    public GridTerrainMesh mesh;

    void OnEnable()
    {
        if (!data) data = GetComponent<GridTerrainData>();
        if (!mesh) mesh = GetComponent<GridTerrainMesh>();
    }

    public void Rebuild()
    {
        if (data && mesh)
            mesh.Build(data);
    }
}
