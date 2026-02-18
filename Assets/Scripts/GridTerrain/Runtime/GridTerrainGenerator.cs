using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(GridTerrainMesh))]
[RequireComponent(typeof(GridTerrainData))]
public class GridTerrainGenerator : MonoBehaviour
{
    GridTerrainMesh meshComp;
    public GridTerrainData data;

    void OnEnable()
    {
        meshComp = GetComponent<GridTerrainMesh>();
        data = GetComponent<GridTerrainData>();
    }

    public void Rebuild()
    {
        if (meshComp == null)
            meshComp = GetComponent<GridTerrainMesh>();

        if (data == null)
            data = GetComponent<GridTerrainData>();

        meshComp.Build(data);
    }
}
