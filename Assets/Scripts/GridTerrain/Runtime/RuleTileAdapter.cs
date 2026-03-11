using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class RuleTileAdapter
{
    public static Sprite ResolveSprite(
        RuleTile ruleTile,
        Vector2Int position,
        GridTerrainData data)
    {
        if (ruleTile == null || data == null)
            return null;

        for (int i = 0; i < ruleTile.m_TilingRules.Count; i++)
        {
            var rule = ruleTile.m_TilingRules[i];

            if (RuleMatches(rule, position, data, ruleTile))
            {
                Sprite sprite = GetRuleSprite(rule, position, i);
                if (sprite != null)
                    return sprite;
            }
        }

        return ruleTile.m_DefaultSprite;
    }

    static bool RuleMatches(
        RuleTile.TilingRule rule,
        Vector2Int centerPos,
        GridTerrainData data,
        RuleTile ruleTile)
    {
        var neighbors = rule.m_Neighbors;
        var positions = rule.m_NeighborPositions;

        if (neighbors == null || positions == null)
            return false;

        if (neighbors.Count != positions.Count)
            return false;

        for (int i = 0; i < neighbors.Count; i++)
        {
            Vector3Int offset3D = positions[i];

            Vector2Int checkPos = centerPos + new Vector2Int(
                offset3D.x,
                offset3D.y
            );

            if (!Check(checkPos, neighbors[i], data, ruleTile))
                return false;
        }

        return true;
    }

    static bool Check(
        Vector2Int pos,
        int rule,
        GridTerrainData data,
        RuleTile ruleTile)
    {
        bool same = false;

        if (data.TryGetTile(pos, out var tile))
        {
            if (tile.tileId == ruleTile.name)
                same = true;
        }

        return rule switch
        {
            1 => same,
            2 => !same,
            _ => true
        };
    }

    static Sprite GetRuleSprite(
        RuleTile.TilingRule rule,
        Vector2Int position,
        int ruleIndex)
    {
        var sprites = rule.m_Sprites;

        if (sprites == null || sprites.Length == 0)
            return null;

        if (sprites.Length == 1)
            return sprites[0];

        if (rule.m_Output != OutputSprite.Random)
            return sprites[0];

        int seed = Hash(position, ruleIndex);
        var rng = new System.Random(seed);
        int index = rng.Next(sprites.Length);

        return sprites[index];
    }

    static int Hash(Vector2Int pos, int ruleIndex)
    {
        unchecked
        {
            uint x = (uint)pos.x;
            uint y = (uint)pos.y;
            uint r = (uint)ruleIndex;

            uint hash = x * 374761393u;
            hash += y * 668265263u;
            hash += r * 1446648777u;

            hash = (hash ^ (hash >> 13)) * 1274126177u;
            hash ^= hash >> 16;

            return (int)hash;
        }
    }
}
