using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGenerator : SaveLoadSystem
{
    public GameObject startRoom;
    public List<GameObject> roomPrefabs = new();
    public MapData _mapData;
    public List<MapData> _mapDatas = new();
    public List<int> mapLayoutCopy = new();

    [SerializeField] private List<GameObject> spawnedRooms = new();

    private void Start()
    {
        Load(mapSavePath);

        GenerateMap(_mapData);
    }

    public override void Load(string path)
    {
        List<string> allMaps = GetAllMaps();

        for (int i = 0; i < allMaps.Count; i++)
        {
            //Here load data from file
            MapData newMap = ScriptableObject.CreateInstance<MapData>();
            newMap.mapID = (short)i;
            string saveFile = ReadFromFile(mapSavePath + allMaps[i]);
            JsonUtility.FromJsonOverwrite(saveFile, newMap);

            //Checks if map is in standard
            if (CheckMap(newMap))
                _mapDatas.Add(newMap);
        }
    }

    public List<string> GetAllMaps()
    {
        List<string> foundedMaps = new();

        //Gets all files from folder
        DirectoryInfo dir = new(mapSavePath);
        FileInfo[] info = dir.GetFiles("*.*");

        //Fetch files with extension .json
        foreach (FileInfo singleFile in info)
        {
            if (Path.GetExtension(mapSavePath + singleFile.Name) == ".json")
                foundedMaps.Add(singleFile.Name);
        }

        return foundedMaps;
    }

    public void GenerateMap(MapData _mapToGenerate)
    {
        if (_mapData == null)
        {
            print("No room to generate");
            return;
        }

        mapLayoutCopy.Clear();
        mapLayoutCopy = ExtractNumbersFromString(_mapData.mapLayout);

        int index = 0;

        // Loop through the map size starting from bottom-left
        for (int y = _mapToGenerate.mapSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < _mapToGenerate.mapSize.x; x++)
            {
                int number = mapLayoutCopy[index];

                if (number != 0 && number < roomPrefabs.Count)
                {
                    // Calculate the spawn position
                    Vector3 spawnPosition = new(x - Mathf.FloorToInt(_mapToGenerate.mapSize.x / 2), y + 1);

                    // Instantiate the room prefab at the calculated position
                    GameObject newRoom = Instantiate(roomPrefabs[number], transform);
                    newRoom.transform.localPosition = new(
                        spawnPosition.x * _mapData.distanceBetweenRooms,
                        0,
                        spawnPosition.y * _mapData.distanceBetweenRooms
                    );
                    spawnedRooms.Add(newRoom);
                    newRoom.name = $"Room_{index}";
                }

                index++;
            }
        }

        SpawnPortals();
        SetRoomPortals();
    }

    public void ClearMap()
    {
        for (int i = 0; i < spawnedRooms.Count; i++)
            Destroy(spawnedRooms[i]);

        spawnedRooms.Clear();
    }

    private void SpawnPortals()
    {
        RoomConfiguration _newRoomConfiguration;

        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            _newRoomConfiguration = spawnedRooms[i].GetComponent<RoomConfiguration>();

            List<PortalPosition> portalPositions = new();
            float spawnDistance = Vector3.Distance(spawnedRooms[i].transform.position, startRoom.transform.position);

            if (spawnDistance <= _mapData.distanceBetweenRooms)
            {
                portalPositions.Add(PortalPosition.South);
                startRoom.GetComponent<RoomConfiguration>().nearbyRooms.Add(_newRoomConfiguration);
                _newRoomConfiguration.nearbyRooms.Add(startRoom.GetComponent<RoomConfiguration>());
            }

            for (int j = 0; j < spawnedRooms.Count; j++)
            {
                //if i and j are equal then skip
                if (i == j)
                    continue;

                //Checks if j is out of bounds
                if (j >= spawnedRooms.Count)
                    return;

                //Checks the distance between rooms
                float distance = Vector3.Distance(spawnedRooms[i].transform.position, spawnedRooms[j].transform.position);

                if (distance <= _mapData.distanceBetweenRooms)
                {
                    Vector3 directionVector = spawnedRooms[j].transform.position - spawnedRooms[i].transform.position;

                    if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
                    {
                        // Mostly horizontal
                        if (directionVector.x > 0)
                            portalPositions.Add(PortalPosition.East);
                        else
                            portalPositions.Add(PortalPosition.West);
                    }
                    else
                    {
                        // Mostly vertical
                        if (directionVector.z > 0)
                            portalPositions.Add(PortalPosition.North);
                        else
                            portalPositions.Add(PortalPosition.South);
                    }

                    _newRoomConfiguration.nearbyRooms.Add(spawnedRooms[j].GetComponent<RoomConfiguration>());
                }
            }

            _newRoomConfiguration.CustomConfig(portalPositions);
        }
    }

    private void SetRoomPortals()
    {
        //Configuring start room portal
        RoomConfiguration _startRoomConfiguration = startRoom.GetComponent<RoomConfiguration>();

        if (_startRoomConfiguration.nearbyRooms.Count > 0)
            _startRoomConfiguration.ConfigurePortal(PortalPosition.North, _startRoomConfiguration.nearbyRooms[0].GetOppositePortal(PortalPosition.North).transform);

        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            RoomConfiguration _roomConfiguration = spawnedRooms[i].GetComponent<RoomConfiguration>();

            //Connecting portals to each other
            for (int j = 0; j < _roomConfiguration.nearbyRooms.Count; j++)
            {
                switch (_roomConfiguration.portalPositions[j])
                {
                    case PortalPosition.West:
                        Transform oppositeWestPortal = _roomConfiguration.nearbyRooms[j].GetOppositePortal(PortalPosition.West).transform;

                        if (oppositeWestPortal == null)
                            break;

                        _roomConfiguration.ConfigurePortal(PortalPosition.West, oppositeWestPortal);
                        break;

                    case PortalPosition.North:
                        Transform oppositeNorthPortal = _roomConfiguration.nearbyRooms[j].GetOppositePortal(PortalPosition.North).transform;

                        if (oppositeNorthPortal == null)
                            break;

                        _roomConfiguration.ConfigurePortal(PortalPosition.North, oppositeNorthPortal);
                        break;

                    case PortalPosition.East:
                        Transform oppositeEastPortal = _roomConfiguration.nearbyRooms[j].GetOppositePortal(PortalPosition.East).transform;

                        if (oppositeEastPortal == null)
                            break;

                        _roomConfiguration.ConfigurePortal(PortalPosition.East, oppositeEastPortal);
                        break;

                    case PortalPosition.South:
                        Transform oppositeSouthPortal = _roomConfiguration.nearbyRooms[j].GetOppositePortal(PortalPosition.South).transform;

                        if (oppositeSouthPortal == null)
                            break;

                        _roomConfiguration.ConfigurePortal(PortalPosition.South, oppositeSouthPortal);
                        break;
                }
            }
        }
    }

    private bool CheckMap(MapData _mapToCheck)
    {
        int columnNumber = 0; //Checker if map is correctly made

        for (int i = 0; i < _mapToCheck.mapLayout.Length; i++)
        {
            if (_mapToCheck.mapLayout[i] == ',' || _mapToCheck.mapLayout[i] == '\n')
                columnNumber++;

            //Getting to the last column
            if (_mapToCheck.mapLayout[i] == '\n')
            {
                if (_mapToCheck.mapSize.x == 0)
                    _mapToCheck.mapSize.x = columnNumber;

                _mapToCheck.mapSize.y++;

                //Checking if every row is the same size
                if (columnNumber != _mapToCheck.mapSize.x)
                {
                    Debug.LogError($"There is different row length on row: {_mapToCheck.mapSize.y}");
                    return false;
                }
                else
                    columnNumber = 0;
            }
        }

        return true;
    }

    private List<int> ExtractNumbersFromString(string input)
    {
        string[] rows = input.Split('\n');
        List<int> numbersOnly = new();

        // Loop through each row
        foreach (string row in rows)
        {
            // Split the row by commas
            string[] numbers = row.Split(',');

            // Loop through each number in the row
            foreach (string number in numbers)
            {
                if (!int.TryParse(number, out int newNumber))
                    newNumber = 0;

                numbersOnly.Add(newNumber); 
            }
        }

        numbersOnly.RemoveAt(numbersOnly.Count - 1);

        return numbersOnly;
    }
}
