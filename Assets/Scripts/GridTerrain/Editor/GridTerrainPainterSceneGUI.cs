using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GridTerrainPainterSceneGUI
{
    static Vector2Int? lastPaintedCell = null;

    static GridTerrainPainterSceneGUI()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.alt)
            return;

        if (!Selection.activeGameObject)
            return;

        var generator = Selection.activeGameObject.GetComponent<GridTerrainGenerator>();
        if (!generator || !generator.data)
            return;

        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        float paintHeight =
            generator.data.tileHeight *
            GridTerrainPainterWindow.currentHeight;

        // 🔥 pozioma płaszczyzna NA WYSOKOŚCI MALOWANIA
        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0, paintHeight, 0)
        );

        if (!groundPlane.Raycast(ray, out float enter))
            return;

        Vector3 world = ray.GetPoint(enter);

        float size = generator.data.asset.cellSize / 2f;

        Vector2Int cell = new(
            Mathf.FloorToInt(world.x / size),
            Mathf.FloorToInt(world.z / size)
        );

        DrawLocalGrid(cell, generator);
        DrawPreview(cell, generator);
        HandlePainting(e, cell, generator);
    }

    static void DrawPreview(Vector2Int cell, GridTerrainGenerator generator)
    {
        var data = generator.data;
        if (data == null)
            return;

        float size = data.asset.cellSize / 2f;
        float tileHeight = data.tileHeight;
        float previewHeight =
            GridTerrainPainterWindow.currentHeight * tileHeight;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int c = cell + new Vector2Int(x, y);

                float originalBase =
                    GetSimulatedHeight(c, cell, previewHeight, data, false);

                float simulatedBase =
                    GetSimulatedHeight(c, cell, previewHeight, data, true);

                // jeśli tile nie istnieje ani przed ani po – pomijamy
                if (originalBase == 0f && simulatedBase == 0f)
                    continue;

                var originalCorners =
                    GetCorners(c, originalBase, cell, previewHeight, data, false);

                var simulatedCorners =
                    GetCorners(c, simulatedBase, cell, previewHeight, data, true);

                if (!CornersChanged(originalCorners, simulatedCorners))
                    continue;

                DrawTileFromCorners(c, simulatedBase, simulatedCorners, size);
            }
        }
    }

    static float GetSimulatedHeight(
        Vector2Int checkCell,
        Vector2Int previewCell,
        float previewHeight,
        GridTerrainData data,
        bool simulate)
    {
        if (simulate &&
            checkCell == previewCell &&
            GridTerrainPainterWindow.paintMode == PaintMode.Paint)
        {
            return previewHeight;
        }

        if (data.TryGetTile(checkCell, out var tile))
            return tile.height * data.tileHeight;

        return 0f;
    }

    static void DrawSimulatedTile(
        Vector2Int cell,
        float baseHeight,
        float previewHeight,
        Vector2Int previewCell,
        GridTerrainData data,
        float size)
    {
        float half = size / 2f;

        float hBL = GetCornerSim(cell, new Vector2Int(-1, -1), baseHeight, previewCell, previewHeight, data);
        float hBR = GetCornerSim(cell, new Vector2Int(1, -1), baseHeight, previewCell, previewHeight, data);
        float hTL = GetCornerSim(cell, new Vector2Int(-1, 1), baseHeight, previewCell, previewHeight, data);
        float hTR = GetCornerSim(cell, new Vector2Int(1, 1), baseHeight, previewCell, previewHeight, data);

        Vector3 center = new(
            cell.x * size + size / 2f,
            baseHeight,
            cell.y * size + size / 2f
        );

        Vector3[] v =
        {
        center + new Vector3(-half, hBL - baseHeight, -half),
        center + new Vector3(half,  hBR - baseHeight, -half),
        center + new Vector3(half,  hTR - baseHeight,  half),
        center + new Vector3(-half, hTL - baseHeight,  half),
    };

        Handles.color = new Color(0f, 1f, 1f, 0.8f);

        Handles.DrawLine(v[0], v[1]);
        Handles.DrawLine(v[1], v[2]);
        Handles.DrawLine(v[2], v[3]);
        Handles.DrawLine(v[3], v[0]);
    }

    static float[] GetCorners(
    Vector2Int cell,
    float baseHeight,
    Vector2Int previewCell,
    float previewHeight,
    GridTerrainData data,
    bool simulate)
    {
        return new float[]
        {
        GetCorner(cell, new Vector2Int(-1,-1), baseHeight, previewCell, previewHeight, data, simulate),
        GetCorner(cell, new Vector2Int( 1,-1), baseHeight, previewCell, previewHeight, data, simulate),
        GetCorner(cell, new Vector2Int( 1, 1), baseHeight, previewCell, previewHeight, data, simulate),
        GetCorner(cell, new Vector2Int(-1, 1), baseHeight, previewCell, previewHeight, data, simulate)
        };
    }

    static float GetCornerSim(
        Vector2Int cell,
        Vector2Int cornerOffset,
        float baseHeight,
        Vector2Int previewCell,
        float previewHeight,
        GridTerrainData data)
    {
        float maxHeight = baseHeight;

        Vector2Int[] influence =
        {
        new(0,0),
        new(cornerOffset.x,0),
        new(0,cornerOffset.y),
        new(cornerOffset.x,cornerOffset.y)
    };

        foreach (var offset in influence)
        {
            Vector2Int pos = cell + offset;

            float h = 0f;

            if (pos == previewCell &&
                GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            {
                h = previewHeight;
            }
            else if (data.TryGetTile(pos, out var tile))
            {
                h = tile.height * data.tileHeight;
            }

            if (h > maxHeight)
                maxHeight = h;
        }

        return maxHeight;
    }

    static float GetCorner(
        Vector2Int cell,
        Vector2Int cornerOffset,
        float baseHeight,
        Vector2Int previewCell,
        float previewHeight,
        GridTerrainData data,
        bool simulate)
    {
        float maxHeight = baseHeight;

        Vector2Int[] influence =
        {
        new(0,0),
        new(cornerOffset.x,0),
        new(0,cornerOffset.y),
        new(cornerOffset.x,cornerOffset.y)
    };

        foreach (var offset in influence)
        {
            Vector2Int pos = cell + offset;
            float h = 0f;

            if (simulate &&
                pos == previewCell &&
                GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            {
                h = previewHeight;
            }
            else if (data.TryGetTile(pos, out var tile))
            {
                h = tile.height * data.tileHeight;
            }

            if (h > maxHeight)
                maxHeight = h;
        }

        return maxHeight;
    }

    static bool CornersChanged(float[] a, float[] b)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!Mathf.Approximately(a[i], b[i]))
                return true;
        }

        return false;
    }

    static void DrawTileFromCorners(
        Vector2Int cell,
        float baseHeight,
        float[] corners,
        float size)
    {
        float half = size / 2f;

        Vector3 center = new(
            cell.x * size + size / 2f,
            baseHeight,
            cell.y * size + size / 2f
        );

        Vector3[] v =
        {
        center + new Vector3(-half, corners[0] - baseHeight, -half),
        center + new Vector3( half, corners[1] - baseHeight, -half),
        center + new Vector3( half, corners[2] - baseHeight,  half),
        center + new Vector3(-half, corners[3] - baseHeight,  half),
    };

        Handles.color = new Color(0f, 1f, 1f, 0.9f);

        Handles.DrawLine(v[0], v[1]);
        Handles.DrawLine(v[1], v[2]);
        Handles.DrawLine(v[2], v[3]);
        Handles.DrawLine(v[3], v[0]);
    }

    static void HandlePainting(Event e, Vector2Int cell, GridTerrainGenerator gen)
    {
        if (e.type == EventType.MouseUp)
        {
            lastPaintedCell = null;
            return;
        }

        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
        {
            if (lastPaintedCell.HasValue && lastPaintedCell.Value == cell)
                return;

            Undo.RecordObject(gen.data, "Grid Terrain Paint");

            switch (GridTerrainPainterWindow.paintMode)
            {
                case PaintMode.Paint:
                    if (GridTerrainPainterWindow.currentRuleTile == null)
                        return;

                    gen.data.AddTile(cell, new TileData
                    {
                        tileId = GridTerrainPainterWindow.currentRuleTile.name,
                        height = GridTerrainPainterWindow.currentHeight,
                        rotation = GridTerrainPainterWindow.currentRotation
                    });

                    break;

                case PaintMode.Erase:
                    gen.data.RemoveTile(cell);
                    break;
            }

            gen.Rebuild();
            lastPaintedCell = cell;
            e.Use();
        }
    }

    static void DrawLocalGrid(Vector2Int centerCell, GridTerrainGenerator generator)
    {
        var data = generator.data;
        if (data == null || data.asset == null)
            return;

        float size = data.asset.cellSize / 2f;

        // 🔥 wysokość zależna od aktualnej wysokości malowania
        float height =
            GridTerrainPainterWindow.currentHeight *
            data.tileHeight;

        int halfRange = 2; // 5x5 grid (2 w każdą stronę)

        Handles.color = new Color(1f, 1f, 1f, 0.25f);

        for (int x = -halfRange; x <= halfRange + 1; x++)
        {
            Vector3 from = new Vector3(
                (centerCell.x + x) * size,
                height,
                (centerCell.y - halfRange) * size
            );

            Vector3 to = new Vector3(
                (centerCell.x + x) * size,
                height,
                (centerCell.y + halfRange + 1) * size
            );

            Handles.DrawLine(from, to);
        }

        for (int y = -halfRange; y <= halfRange + 1; y++)
        {
            Vector3 from = new Vector3(
                (centerCell.x - halfRange) * size,
                height,
                (centerCell.y + y) * size
            );

            Vector3 to = new Vector3(
                (centerCell.x + halfRange + 1) * size,
                height,
                (centerCell.y + y) * size
            );

            Handles.DrawLine(from, to);
        }
    }
}
