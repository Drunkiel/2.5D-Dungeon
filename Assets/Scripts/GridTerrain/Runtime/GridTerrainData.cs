using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Empty,
    Grass,
    Road
}

[System.Serializable]
public struct TileData
{
    public string tileId; // = RuleTile.name
    public int height;
}

public class GridTerrainData : MonoBehaviour
{
    public float cellSize = 1f;

    public Dictionary<Vector2Int, TileData> tiles = new();

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
}
