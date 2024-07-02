using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject roomPrefab;

    [Multiline]
    public string mapLayout = "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n";
    public List<int> mapLayoutCopy = new();

    public Vector2Int mapSize;

    [SerializeField] private List<GameObject> spawnedRooms = new();

    private void Start()
    {
        if (CheckMap())
            GenerateMap();
    }

    public void GenerateMap()
    {
        mapLayoutCopy.Clear();
        mapLayoutCopy = ExtractNumbersFromString(mapLayout);

        int index = 0;

        // Loop through the map size starting from bottom-left
        for (int y = mapSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                int number = mapLayoutCopy[index];

                if (number != 0)
                {
                    // Calculate the spawn position
                    Vector3 spawnPosition = new(x - 1, y + 1);

                    // Instantiate the room prefab at the calculated position
                    GameObject newRoom = Instantiate(roomPrefab, transform);
                    newRoom.transform.localPosition = new(spawnPosition.x * 10, 0, spawnPosition.y * 10);
                    spawnedRooms.Add(newRoom);
                    newRoom.name = $"Room_{index}";
                }

                index++;
            }
        }

        SpawnPortals();
        SetRoomPortals();
    }

    private void SpawnPortals()
    {
        RoomConfiguration _newRoomConfiguration;

        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            _newRoomConfiguration = spawnedRooms[i].GetComponent<RoomConfiguration>();

            List<PortalPosition> portalPositions = new();
            float spawnDistance = Vector3.Distance(spawnedRooms[i].transform.position, startRoom.transform.position);

            if (spawnDistance <= 10)
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

                if (distance <= 10)
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

    private bool CheckMap()
    {
        int columnNumber = 0; //Checker if map is correctly made

        for (int i = 0; i < mapLayout.Length; i++)
        {
            if (mapLayout[i] == ',' || mapLayout[i] == '\n')
                columnNumber++;

            //Getting to the last collumn
            if (mapLayout[i] == '\n')
            {
                if (mapSize.x == 0)
                    mapSize.x = (i + 1) / 2;

                mapSize.y++;

                //Checking if every row is the same size
                if (columnNumber != mapSize.x)
                {
                    Debug.LogError($"There is diffrent row length on row: {mapSize.y}");
                    return false;
                }
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

        return numbersOnly;
    }
}
