using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridTerrainMesh : MonoBehaviour
{
    public Mesh mesh;

    void EnsureMeshReference()
    {
        var mf = GetComponent<MeshFilter>();

        if (mf.sharedMesh != null)
        {
            mesh = mf.sharedMesh;
            return;
        }

        mesh = new Mesh
        {
            name = "GridTerrain_Runtime"
        };
        mf.sharedMesh = mesh;
    }

    public Mesh GetMesh()
    {
        EnsureMeshReference();
        return mesh;
    }

    public void Build(GridTerrainData sourceData)
    {
        if (sourceData == null)
            return;

        EnsureMeshReference();

        if ((sourceData.tiles == null || sourceData.tiles.Count == 0)
            && mesh != null
            && mesh.vertexCount > 0)
            return;

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        foreach (var kvp in sourceData.AllTiles())
        {
            Vector2Int cell = kvp.Key;
            List<TileData> tileStack = kvp.Value;

            foreach (var tile in tileStack)
            {
                AddTile(
                    cell,
                    tile,
                    sourceData,
                    vertices,
                    triangles,
                    uvs
                );
            }
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

        float size = data.asset.cellSize / 2;
        float baseHeight = tile.height * data.tileHeight;

        int vIndex = vertices.Count;

        Vector3 center = new(
            cell.x * size + size / 2f,
            baseHeight,
            cell.y * size + size / 2f
        );

        //Snapping corners
        float hBL = GetCornerHeight(cell, new Vector2Int(-1, -1), baseHeight, data);
        float hBR = GetCornerHeight(cell, new Vector2Int(1, -1), baseHeight, data);
        float hTL = GetCornerHeight(cell, new Vector2Int(-1, 1), baseHeight, data);
        float hTR = GetCornerHeight(cell, new Vector2Int(1, 1), baseHeight, data);

        float half = size / 2f;

        vertices.Add(center + new Vector3(-half, hBL - baseHeight, -half)); // 0 BL
        vertices.Add(center + new Vector3(half, hBR - baseHeight, -half)); // 1 BR
        vertices.Add(center + new Vector3(half, hTR - baseHeight, half)); // 2 TR
        vertices.Add(center + new Vector3(-half, hTL - baseHeight, half)); // 3 TL

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

    float GetCornerHeight(
        Vector2Int cell,
        Vector2Int cornerOffset,
        float baseHeight,
        GridTerrainData data)
    {
        float maxHeight = baseHeight;

        Vector2Int[] influence =
        {
            new(0,0),
            new(cornerOffset.x,0),
            new(0,cornerOffset.y),
            new(cornerOffset.x,cornerOffset.y)
        };

        foreach (var offset in influence)
        {
            Vector2Int checkPos = cell + offset;

            if (data.TryGetTiles(checkPos, out var list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    float h = list[i].height * data.tileHeight;
                    if (h > maxHeight)
                        maxHeight = h;
                }
            }
        }

        return maxHeight;
    }
}