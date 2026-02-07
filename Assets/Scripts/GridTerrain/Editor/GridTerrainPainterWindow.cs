using UnityEditor;
using UnityEngine;

public enum PaintMode
{
    Paint,
    Erase
}

public class GridTerrainPainterWindow : EditorWindow
{
    public static PaintMode paintMode = PaintMode.Paint;

    public static RuleTile currentRuleTile;
    public static int currentHeight = 0;

    RuleTile[] availableTiles;
    string[] tileNames;
    int selectedIndex = -1;

    [MenuItem("Tools/Grid Terrain Painter")]
    static void Open()
    {
        GetWindow<GridTerrainPainterWindow>("Grid Terrain Painter");
    }

    void OnEnable()
    {
        LoadTiles();
    }

    void LoadTiles()
    {
        availableTiles = Resources.LoadAll<RuleTile>("");

        tileNames = new string[availableTiles.Length];
        for (int i = 0; i < availableTiles.Length; i++)
            tileNames[i] = availableTiles[i].name;

        if (availableTiles.Length > 0)
        {
            selectedIndex = 0;
            currentRuleTile = availableTiles[0];
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Mode", EditorStyles.boldLabel);
        paintMode = (PaintMode)GUILayout.Toolbar(
            (int)paintMode,
            new[] { "Paint", "Erase" }
        );

        GUILayout.Space(10);

        if (paintMode == PaintMode.Paint)
        {
            DrawTileSelection();
            GUILayout.Space(5);
            currentHeight = EditorGUILayout.IntField("Height (Y)", currentHeight);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reload Tiles"))
            LoadTiles();
    }

    void DrawTileSelection()
    {
        GUILayout.Label("Rule Tile", EditorStyles.boldLabel);

        if (availableTiles == null || availableTiles.Length == 0)
        {
            EditorGUILayout.HelpBox(
                "No RuleTile found in Resources/",
                MessageType.Warning
            );
            return;
        }

        int newIndex = EditorGUILayout.Popup(
            "Tile",
            selectedIndex,
            tileNames
        );

        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            currentRuleTile = availableTiles[selectedIndex];
        }
    }
}
