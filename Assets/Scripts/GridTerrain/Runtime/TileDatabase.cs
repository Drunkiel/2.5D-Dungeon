using UnityEngine;
using System.Collections.Generic;

public static class TileDatabase
{
    static Dictionary<string, RuleTile> tiles;

    public static void Build()
    {
        tiles = new();
        foreach (var tile in Resources.LoadAll<RuleTile>(""))
            tiles[tile.name] = tile;
    }

    public static RuleTile GetRuleTile(string id)
    {
        if (tiles == null) Build();
        return tiles.TryGetValue(id, out var t) ? t : null;
    }
}
