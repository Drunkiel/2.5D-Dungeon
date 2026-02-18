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
        bool same =
            data.TryGetTile(pos, out TileData t) &&
            t.tileId == ruleTile.name;

        return rule switch
        {
            1 => same,      // MUST HAVE
            2 => !same,     // MUST NOT HAVE
            _ => true       // DON'T CARE
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

        //Single sprite
        if (sprites.Length == 1)
            return sprites[0];

        //Select random sprite
        if (rule.m_Output != OutputSprite.Random)
            return sprites[0];

        int seed = Hash(position, ruleIndex);
        int index = Mathf.Abs(seed) % sprites.Length;

        return sprites[index];
    }

    static int Hash(Vector2Int pos, int ruleIndex)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + pos.x;
            hash = hash * 31 + pos.y;
            hash = hash * 31 + ruleIndex;
            return hash;
        }
    }

}
