using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileData
{
    public string tileId;
    public float height;
    public Vector3 rotation;
}

public class GridTerrainData : MonoBehaviour
{
    public GridTerrainAsset asset;
    public Dictionary<Vector2Int, List<TileData>> tiles = new();

    public float heightStep = 0.2f;
    public float tileHeight = 1f;
    public bool autoSnapHeight = true;

    public void SaveToAsset()
    {
        if (asset == null)
            return;

        asset.SyncFromDictionary(tiles);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(asset);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void Clear()
    {
        tiles.Clear();
    }

    public void AddTile(Vector2Int pos, TileData data)
    {
        float snapped =
            Mathf.Round(data.height / heightStep) * heightStep;

        data.height = snapped;

        if (!tiles.TryGetValue(pos, out var list))
        {
            list = new List<TileData>();
            tiles[pos] = list;
        }

        list.Add(data);

        AutoFillNeighbors(pos, data);
    }

    public void RemoveTopTile(Vector2Int pos)
    {
        if (!tiles.TryGetValue(pos, out var list))
            return;

        if (list.Count == 0)
            return;

        list.RemoveAt(list.Count - 1);

        if (list.Count == 0)
            tiles.Remove(pos);
    }

    public bool TryGetTiles(Vector2Int pos, out List<TileData> list)
    {
        return tiles.TryGetValue(pos, out list);
    }

    public IEnumerable<KeyValuePair<Vector2Int, List<TileData>>> AllTiles()
    {
        return tiles;
    }

    void AutoFillNeighbors(Vector2Int pos, TileData placedTile)
    {
        Vector2Int[] dirs =
        {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

        foreach (var dir in dirs)
        {
            Vector2Int neighborPos = pos + dir;

            if (!tiles.TryGetValue(neighborPos, out var neighborList))
                continue;

            if (neighborList.Count == 0)
                continue;

            TileData neighborTop = neighborList[neighborList.Count - 1];

            float diff = placedTile.height - neighborTop.height;

            if (Mathf.Abs(diff) <= heightStep)
                continue;

            float min = Mathf.Min(placedTile.height, neighborTop.height);
            float max = Mathf.Max(placedTile.height, neighborTop.height);

            float current = min + heightStep;

            while (current < max)
            {
                TileData filler = new()
                {
                    tileId = placedTile.tileId,
                    height = current,
                    rotation = Vector3.zero
                };

                neighborList.Add(filler);

                current += heightStep;
            }
        }
    }

}
