using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TileData
{
    public string tileId;
    public int height;
}

public class GridTerrainData : MonoBehaviour
{
    public GridTerrainAsset asset;
    public Dictionary<Vector2Int, TileData> tiles = new();

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

    public bool HasTile(Vector2Int pos)
    {
        return tiles.ContainsKey(pos);
    }

    public void SetTile(Vector2Int pos, TileData data)
    {
        tiles[pos] = data;
    }

    public void RemoveTile(Vector2Int pos)
    {
        tiles.Remove(pos);
    }

    public bool TryGetTile(Vector2Int pos, out TileData data)
    {
        return tiles.TryGetValue(pos, out data);
    }

    public IEnumerable<KeyValuePair<Vector2Int, TileData>> AllTiles()
    {
        return tiles;
    }
}
