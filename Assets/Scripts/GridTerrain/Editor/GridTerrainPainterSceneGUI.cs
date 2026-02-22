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

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3 world = hit.point;

            float size = generator.data.asset.cellSize / 2f;

            Vector2Int cell = new(
                Mathf.FloorToInt(world.x / size),
                Mathf.FloorToInt(world.z / size)
            );

            DrawPreview(cell, generator);
            HandlePainting(e, cell, generator);
        }
    }

    static void DrawPreview(
        Vector2Int cell,
        GridTerrainGenerator generator)
    {
        var data = generator.data;
        if (data == null)
            return;

        float size = data.asset.cellSize / 2f;

        float worldHeight =
            GridTerrainPainterWindow.currentHeight *
            data.tileHeight;

        Vector3 center = new(
            cell.x * size + size / 2f,
            worldHeight,
            cell.y * size + size / 2f
        );

        Quaternion rot =
            Quaternion.Euler(GridTerrainPainterWindow.currentRotation);

        Vector3[] quad =
        {
            new(-size/2, 0, -size/2),
            new(size/2, 0, -size/2),
            new(size/2, 0, size/2),
            new(-size/2, 0, size/2),
        };

        float minY = float.MaxValue;
        Vector3[] rotated = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            rotated[i] = rot * quad[i];

            if (rotated[i].y < minY)
                minY = rotated[i].y;
        }

        Vector3 snapOffset = Vector3.zero;

        if (data.autoSnapHeight)
        {
            snapOffset = new Vector3(0, -minY, 0);
        }

        Handles.color =
            GridTerrainPainterWindow.paintMode == PaintMode.Paint
            ? Color.green
            : Color.red;

        for (int i = 0; i < 4; i++)
        {
            rotated[i] += center + snapOffset;
        }

        Handles.DrawLine(rotated[0], rotated[1]);
        Handles.DrawLine(rotated[1], rotated[2]);
        Handles.DrawLine(rotated[2], rotated[3]);
        Handles.DrawLine(rotated[3], rotated[0]);
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

            if (GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            {
                if (GridTerrainPainterWindow.currentRuleTile == null)
                    return;

                gen.data.AddTile(cell, new TileData
                {
                    tileId = GridTerrainPainterWindow.currentRuleTile.name,
                    height = GridTerrainPainterWindow.currentHeight,
                    rotation = GridTerrainPainterWindow.currentRotation
                });
            }
            else
            {
                gen.data.RemoveTopTile(cell);
            }

            gen.Rebuild();
            lastPaintedCell = cell;
            e.Use();
        }
    }
}
