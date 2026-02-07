using UnityEngine;
using System.Reflection;
using System.Collections;

public static class RuleTileAdapter
{
    static FieldInfo rulesField;

    static RuleTileAdapter()
    {
        rulesField = FindRulesField();
    }

    static FieldInfo FindRulesField()
    {
        System.Type type = typeof(RuleTile);

        while (type != null)
        {
            var fields = type.GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public
            );

            foreach (var f in fields)
            {
                if (typeof(IEnumerable).IsAssignableFrom(f.FieldType))
                {
                    if (f.Name.ToLower().Contains("rule"))
                        return f;
                }
            }

            type = type.BaseType;
        }

        return null;
    }

    public static Sprite ResolveSprite(
        RuleTile ruleTile,
        Vector2Int position,
        GridTerrainData data)
    {
        if (ruleTile == null || data == null)
            return null;

        if (rulesField == null)
        {
            Debug.LogError(
                $"RuleTileAdapter: Unable to locate rules field via reflection ({ruleTile.name})"
            );
            return ruleTile.m_DefaultSprite;
        }

        var rulesObj = rulesField.GetValue(ruleTile);
        if (rulesObj is not IEnumerable rules)
            return ruleTile.m_DefaultSprite;

        foreach (var rule in rules)
        {
            if (rule == null)
                continue;

            if (RuleMatches(rule, position, data, ruleTile))
            {
                Sprite sprite = GetRuleSprite(rule);
                if (sprite != null)
                    return sprite;
            }
        }

        return ruleTile.m_DefaultSprite;
    }

    static bool RuleMatches(
        object rule,
        Vector2Int pos,
        GridTerrainData data,
        RuleTile ruleTile)
    {
        var neighborsField = rule.GetType().GetField(
            "m_Neighbors",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (neighborsField == null)
            return false;

        int[] neighbors = neighborsField.GetValue(rule) as int[];
        if (neighbors == null || neighbors.Length < 4)
            return false;

        return Check(pos + Vector2Int.up, neighbors[0], data, ruleTile) &&
               Check(pos + Vector2Int.right, neighbors[1], data, ruleTile) &&
               Check(pos + Vector2Int.down, neighbors[2], data, ruleTile) &&
               Check(pos + Vector2Int.left, neighbors[3], data, ruleTile);
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
            1 => same,
            2 => !same,
            _ => true
        };
    }

    static Sprite GetRuleSprite(object rule)
    {
        var spritesField = rule.GetType().GetField(
            "m_Sprites",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (spritesField == null)
            return null;

        return spritesField.GetValue(rule) is Sprite[] sprites && sprites.Length > 0
            ? sprites[0]
            : null;
    }
}
