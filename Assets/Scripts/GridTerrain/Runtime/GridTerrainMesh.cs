using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridTerrainMesh : MonoBehaviour
{
    Mesh mesh;
    GridTerrainData data;

    void OnEnable()
    {
        EnsureInitialized();
    }

    void Awake()
    {
        EnsureInitialized();
    }

    void EnsureInitialized()
    {
        if (data == null)
            data = GetComponent<GridTerrainData>();

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "GridTerrainMesh";

            MeshFilter mf = GetComponent<MeshFilter>();
            mf.sharedMesh = mesh;
        }
    }

    public void Build(GridTerrainData sourceData)
    {
        EnsureInitialized();

        if (sourceData == null || sourceData.tiles == null)
            return;

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        foreach (var kvp in sourceData.tiles)
        {
            AddTile(
                kvp.Key,
                kvp.Value,
                sourceData,
                vertices,
                triangles,
                uvs
            );
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void AddTile(
        Vector2Int cell,
        TileData tile,
        GridTerrainData data,
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector2> uvs)
    {
        if (string.IsNullOrEmpty(tile.tileId))
            return;

        var ruleTile = TileDatabase.GetRuleTile(tile.tileId);
        if (ruleTile == null)
            return;

        var sprite = RuleTileAdapter.ResolveSprite(ruleTile, cell, data);
        if (sprite == null)
            return;

        SpriteUV.Get(sprite, out Vector2 uvMin, out Vector2 uvMax);

        float size = data.cellSize;
        float height = tile.height;

        int vIndex = vertices.Count;

        Vector3 origin = new Vector3(
            cell.x * size,
            height,
            cell.y * size
        );

        vertices.Add(origin);
        vertices.Add(origin + new Vector3(size, 0, 0));
        vertices.Add(origin + new Vector3(size, 0, size));
        vertices.Add(origin + new Vector3(0, 0, size));

        triangles.Add(vIndex + 0);
        triangles.Add(vIndex + 2);
        triangles.Add(vIndex + 1);

        triangles.Add(vIndex + 0);
        triangles.Add(vIndex + 3);
        triangles.Add(vIndex + 2);

        uvs.Add(new Vector2(uvMin.x, uvMin.y));
        uvs.Add(new Vector2(uvMax.x, uvMin.y));
        uvs.Add(new Vector2(uvMax.x, uvMax.y));
        uvs.Add(new Vector2(uvMin.x, uvMax.y));
    }
}
