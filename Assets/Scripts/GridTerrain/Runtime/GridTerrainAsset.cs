using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid Terrain/Terrain Data")]
public class GridTerrainAsset : ScriptableObject
{
    public float cellSize = 0.5f;

    [System.Serializable]
    public struct TileEntry
    {
        public Vector2Int position;
        public TileData data;
    }

    public List<TileEntry> tiles = new();

    Dictionary<Vector2Int, TileData> runtimeLookup;

    public void BuildLookup()
    {
        runtimeLookup = new Dictionary<Vector2Int, TileData>();

        foreach (var t in tiles)
        {
            runtimeLookup[t.position] = t.data;
        }
    }

    public Dictionary<Vector2Int, TileData> GetDictionary()
    {
        if (runtimeLookup == null)
            BuildLookup();

        return runtimeLookup;
    }

    public void SyncFromDictionary(Dictionary<Vector2Int, TileData> dict)
    {
        tiles.Clear();

        foreach (var kvp in dict)
        {
            tiles.Add(new TileEntry
            {
                position = kvp.Key,
                data = kvp.Value
            });
        }
    }
}
