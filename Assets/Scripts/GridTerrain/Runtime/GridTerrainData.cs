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

    // 🔥 jedna komórka = jeden tile
    public Dictionary<Vector2Int, TileData> tiles = new();

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

        // 🔥 nadpisujemy zamiast dodawać
        tiles[pos] = data;
    }

    public void RemoveTile(Vector2Int pos)
    {
        if (tiles.ContainsKey(pos))
            tiles.Remove(pos);
    }

    public bool TryGetTile(Vector2Int pos, out TileData tile)
    {
        return tiles.TryGetValue(pos, out tile);
    }

    public IEnumerable<KeyValuePair<Vector2Int, TileData>> AllTiles()
    {
        return tiles;
    }
}