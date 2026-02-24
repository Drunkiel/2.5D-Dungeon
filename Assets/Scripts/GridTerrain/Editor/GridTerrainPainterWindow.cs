using UnityEditor;
using UnityEngine;

public enum PaintMode
{
    Paint,
    Erase,
    Raise,
    Lower
}

public class GridTerrainPainterWindow : EditorWindow
{
    public static PaintMode paintMode = PaintMode.Paint;

    public static RuleTile currentRuleTile;

    public static bool autoSnapHeight = true;
    public static float currentHeight = 0;

    public static Vector3 currentRotation = Vector3.zero;
    public static bool snapRotation = true;
    public static float snapStep = 90f;

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
           new[] { "Paint", "Erase", "Raise", "Lower" }
        );

        GUILayout.Space(10);

        if (paintMode == PaintMode.Paint)
        {
            DrawTileSelection();
            GUILayout.Space(5);


            GridTerrainData data = FindObjectOfType<GridTerrainData>();

            if (data != null)
            {
                GUILayout.Space(10);
                GUILayout.Label("Height Controls", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    currentHeight -= data.heightStep;
                    currentHeight = Mathf.Round(currentHeight / data.heightStep) * data.heightStep;
                }

                currentHeight = EditorGUILayout.FloatField("Height (Y)", currentHeight);

                if (GUILayout.Button("+"))
                {
                    currentHeight += data.heightStep;
                    currentHeight = Mathf.Round(currentHeight / data.heightStep) * data.heightStep;
                }

                EditorGUILayout.EndHorizontal();

                data.heightStep = EditorGUILayout.FloatField("Height Step", data.heightStep);
            }

            GUILayout.Space(10);
            GUILayout.Label("Rotation", EditorStyles.boldLabel);

            snapRotation = EditorGUILayout.Toggle("Snap Rotation", snapRotation);

            currentRotation = EditorGUILayout.Vector3Field(
                "Rotation (XYZ)",
                currentRotation
            );

            if (data != null)
            {
                data.autoSnapHeight = EditorGUILayout.Toggle(
                    "Auto Snap Height",
                    data.autoSnapHeight
                );
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reload Tiles"))
            LoadTiles();

        if (GUILayout.Button("Save Terrain"))
            SaveTerrain();

        if (GUILayout.Button("Load Terrain"))
            LoadAsset();
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

    void SaveTerrain()
    {
        GridTerrainComponent terrain =
            FindObjectOfType<GridTerrainComponent>();

        if (terrain == null)
        {
            Debug.LogError("No GridTerrainComponent in scene");
            return;
        }

        terrain.SaveAsAsset();
        terrain.data.SaveToAsset();
    }

    void LoadAsset()
    {
        GridTerrainData data = FindObjectOfType<GridTerrainData>();
        GridTerrainGenerator generator = FindObjectOfType<GridTerrainGenerator>();

        data.tiles = data.asset.GetDictionary();
        generator.Rebuild();
    }
}
