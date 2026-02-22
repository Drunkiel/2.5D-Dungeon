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
        public List<TileData> dataList;
    }

    public List<TileEntry> tiles = new();

    Dictionary<Vector2Int, List<TileData>> runtimeLookup;

    public void BuildLookup()
    {
        runtimeLookup = new Dictionary<Vector2Int, List<TileData>>();

        foreach (var t in tiles)
            runtimeLookup[t.position] = new List<TileData>(t.dataList);
    }

    public Dictionary<Vector2Int, List<TileData>> GetDictionary()
    {
        if (runtimeLookup == null)
            BuildLookup();

        return runtimeLookup;
    }

    public void SyncFromDictionary(Dictionary<Vector2Int, List<TileData>> dict)
    {
        tiles.Clear();

        foreach (var kvp in dict)
        {
            tiles.Add(new TileEntry
            {
                position = kvp.Key,
                dataList = new List<TileData>(kvp.Value)
            });
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
