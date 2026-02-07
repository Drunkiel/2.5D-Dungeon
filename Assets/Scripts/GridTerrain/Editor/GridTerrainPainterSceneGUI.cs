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
        float planeHeight = GetRaycastPlaneHeight(generator);
        Plane plane = new(Vector3.up, new Vector3(0, planeHeight, 0));

        if (!plane.Raycast(ray, out float dist))
            return;

        Vector3 world = ray.GetPoint(dist);
        float size = generator.data.cellSize;

        Vector2Int cell = new Vector2Int(
            Mathf.FloorToInt(world.x / size),
            Mathf.FloorToInt(world.z / size)
        );

        float previewHeight = GetPreviewHeight(cell, generator);

        DrawPreview(cell, size, previewHeight);
        HandlePainting(e, cell, generator);
    }

    static float GetRaycastPlaneHeight(GridTerrainGenerator gen)
    {
        if (GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            return GridTerrainPainterWindow.currentHeight;

        //Erase
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);

        if (zeroPlane.Raycast(ray, out float dist))
        {
            Vector3 world = ray.GetPoint(dist);
            Vector2Int cell = new Vector2Int(
                Mathf.FloorToInt(world.x / gen.data.cellSize),
                Mathf.FloorToInt(world.z / gen.data.cellSize)
            );

            if (gen.data.TryGetTile(cell, out var tile))
                return tile.height;
        }

        return 0f;
    }

    static float GetPreviewHeight(Vector2Int cell, GridTerrainGenerator gen)
    {
        if (GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            return GridTerrainPainterWindow.currentHeight;

        //Erase - show where is erasing
        if (gen.data.TryGetTile(cell, out var tile))
            return tile.height;

        return 0f;
    }

    static void DrawPreview(Vector2Int cell, float size, float height)
    {
        Handles.color =
            GridTerrainPainterWindow.paintMode == PaintMode.Paint
            ? Color.green
            : Color.red;

        Handles.DrawWireCube(
            new Vector3(
                cell.x * size + size / 2f,
                height,
                cell.y * size + size / 2f
            ),
            new Vector3(size, 0.01f, size)
        );
    }

    static void HandlePainting(Event e, Vector2Int cell, GridTerrainGenerator gen)
    {
        //Reset when release on mouse
        if (e.type == EventType.MouseUp)
        {
            lastPaintedCell = null;
            return;
        }

        //Paint
        if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
        {
            if (lastPaintedCell.HasValue && lastPaintedCell.Value == cell)
                return;

            Undo.RecordObject(gen.data, "Grid Terrain Paint");

            if (GridTerrainPainterWindow.paintMode == PaintMode.Paint)
            {
                if (GridTerrainPainterWindow.currentRuleTile == null)
                    return;

                gen.data.SetTile(cell, new TileData
                {
                    tileId = GridTerrainPainterWindow.currentRuleTile.name,
                    height = GridTerrainPainterWindow.currentHeight
                });
            }
            else
            {
                gen.data.RemoveTile(cell);
            }

            gen.Rebuild();
            lastPaintedCell = cell;
            e.Use();
        }
    }
}
