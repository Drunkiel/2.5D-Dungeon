using UnityEditor;
using UnityEngine;

public enum PainterState
{
    MapSelection,
    Editing
}

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

    PainterState currentState = PainterState.MapSelection;
    string newMapName = "NewMap";

    string[] availableMaps;
    int selectedMapIndex = -1;

    const string ROOT_PATH = "Assets/GridTerrains";

    [MenuItem("Tools/Grid Terrain Painter")]
    static void Open()
    {
        GetWindow<GridTerrainPainterWindow>("Grid Terrain Painter");
    }

    void OnEnable()
    {
        LoadTiles();
        RefreshMapList();
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
        GUILayout.Space(10);

        switch (currentState)
        {
            case PainterState.MapSelection:
                DrawMapSelectionView();
                break;

            case PainterState.Editing:
                DrawEditorView();
                break;
        }
    }

    void DrawMapSelectionView()
    {
        GUILayout.Label("Grid Terrain Painter", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Create New Map", EditorStyles.boldLabel);

        newMapName = EditorGUILayout.TextField("Map Name", newMapName);

        if (GUILayout.Button("Create Map"))
        {
            CreateNewMap(newMapName);
            LoadSelectedMapByName(newMapName);
            currentState = PainterState.Editing;
        }

        GUILayout.Space(20);
        GUILayout.Label("Load Existing Map", EditorStyles.boldLabel);

        DrawMapList();

        GUI.enabled = selectedMapIndex >= 0;

        if (GUILayout.Button("Load Map"))
        {
            LoadSelectedMap();
            currentState = PainterState.Editing;
        }

        GUI.enabled = true;
    }

    void DrawEditorView()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("← Back", GUILayout.Width(60)))
        {
            currentState = PainterState.MapSelection;
        }

        GUILayout.Label("Editing Mode", EditorStyles.boldLabel);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        DrawPaintControls();

        GUILayout.Space(20);

        GUILayout.Label("Map", EditorStyles.boldLabel);

        if (GUILayout.Button("Save"))
            SaveTerrain();

        if (GUILayout.Button("Rebuild Mesh"))
            FindObjectOfType<GridTerrainGenerator>()?.Rebuild();
    }

    void DrawPaintControls()
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
            GUILayout.Space(10);

            GridTerrainData data = FindObjectOfType<GridTerrainData>();

            if (data != null)
            {
                GUILayout.Label("Height", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    currentHeight -= data.heightStep;
                    currentHeight =
                        Mathf.Round(currentHeight / data.heightStep) * data.heightStep;
                }

                currentHeight =
                    EditorGUILayout.FloatField("Height (Y)", currentHeight);

                if (GUILayout.Button("+"))
                {
                    currentHeight += data.heightStep;
                    currentHeight =
                        Mathf.Round(currentHeight / data.heightStep) * data.heightStep;
                }

                EditorGUILayout.EndHorizontal();

                data.heightStep =
                    EditorGUILayout.FloatField("Height Step", data.heightStep);
            }

            GUILayout.Space(10);
            GUILayout.Label("Rotation", EditorStyles.boldLabel);

            snapRotation =
                EditorGUILayout.Toggle("Snap Rotation", snapRotation);

            currentRotation =
                EditorGUILayout.Vector3Field("Rotation", currentRotation);

            if (data != null)
            {
                data.autoSnapHeight =
                    EditorGUILayout.Toggle("Auto Snap Height", data.autoSnapHeight);
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Reload Tiles"))
            LoadTiles();
    }

    void LoadSelectedMapByName(string mapName)
    {
        RefreshMapList();

        for (int i = 0; i < availableMaps.Length; i++)
        {
            if (availableMaps[i] == mapName)
            {
                selectedMapIndex = i;
                LoadSelectedMap();
                break;
            }
        }
    }

    void CreateNewMap(string mapName)
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(mapName))
        {
            Debug.LogError("Map name is empty");
            return;
        }

        if (!AssetDatabase.IsValidFolder(ROOT_PATH))
            AssetDatabase.CreateFolder("Assets", "GridTerrain");

        string folderPath = $"{ROOT_PATH}/{mapName}";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder(ROOT_PATH, mapName);
        }
        else
        {
            Debug.LogError("Map folder already exists!");
            return;
        }

        // 🔥 Create Data Asset
        GridTerrainAsset dataAsset = CreateInstance<GridTerrainAsset>();
        AssetDatabase.CreateAsset(
            dataAsset,
            $"{folderPath}/{mapName}Data.asset"
        );

        // 🔥 Create Mesh Asset
        Mesh mesh = new();
        AssetDatabase.CreateAsset(
            mesh,
            $"{folderPath}/{mapName}Mesh.asset"
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Map {mapName} created!");

        RefreshMapList();
#endif
    }

    void DrawMapList()
    {
        if (availableMaps == null)
            RefreshMapList();

        if (availableMaps == null || availableMaps.Length == 0)
        {
            EditorGUILayout.HelpBox("No maps found", MessageType.Info);
            return;
        }

        selectedMapIndex = EditorGUILayout.Popup(
            "Available Maps",
            selectedMapIndex,
            availableMaps
        );
    }

    void RefreshMapList()
    {
#if UNITY_EDITOR
        if (!AssetDatabase.IsValidFolder(ROOT_PATH))
            return;

        string[] folders =
            AssetDatabase.GetSubFolders(ROOT_PATH);

        availableMaps = new string[folders.Length];

        for (int i = 0; i < folders.Length; i++)
            availableMaps[i] =
                System.IO.Path.GetFileName(folders[i]);

        if (availableMaps.Length > 0)
            selectedMapIndex = 0;
#endif
    }

    void LoadSelectedMap()
    {
#if UNITY_EDITOR
        if (selectedMapIndex < 0 || availableMaps.Length == 0)
            return;

        string mapName = availableMaps[selectedMapIndex];
        string folderPath = $"{ROOT_PATH}/{mapName}";

        string dataPath = $"{folderPath}/{mapName}Data.asset";
        string meshPath = $"{folderPath}/{mapName}Mesh.asset";

        GridTerrainAsset dataAsset =
            AssetDatabase.LoadAssetAtPath<GridTerrainAsset>(dataPath);

        Mesh meshAsset =
            AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);

        if (dataAsset == null || meshAsset == null)
        {
            Debug.LogError("Missing assets in map folder");
            return;
        }

        GridTerrainComponent terrain =
            FindObjectOfType<GridTerrainComponent>();

        if (terrain == null)
        {
            Debug.LogError("No GridTerrainComponent in scene");
            return;
        }

        terrain.data.asset = dataAsset;
        terrain.SetMesh(meshAsset);

        terrain.data.tiles =
            dataAsset.GetDictionary();

        FindObjectOfType<GridTerrainGenerator>().Rebuild();

        Debug.Log($"Loaded map {mapName}");
#endif
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
}
