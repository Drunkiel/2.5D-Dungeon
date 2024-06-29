using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject roomPrefab;
    public string mapLayout = "010\n" +
                              "010\n" +
                              "010\n" +
                              "010\n" +
                              "010\n";
    public Vector2 mapSize;

    [SerializeField] private List<GameObject> spawnedRooms = new();

    private void Start()
    {
        GetMapSize();
        //GenerateMap();
    }

    public void GenerateMap()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject newRoom = Instantiate(roomPrefab, transform);
            spawnedRooms.Add(newRoom);
            newRoom.transform.localPosition = new(0, 0, 10 * (i + 1));
            newRoom.name = $"Room_{i}";
            RoomConfiguration _newRoomConfiguration = newRoom.GetComponent<RoomConfiguration>();
            _newRoomConfiguration.Config(new() { PortalPosition.North, PortalPosition.South });

            RoomConfiguration _previousRoomConfiguration;

            if (i == 0)
                _previousRoomConfiguration = startRoom.GetComponent<RoomConfiguration>();
            else
                _previousRoomConfiguration = spawnedRooms[i - 1].GetComponent<RoomConfiguration>();

            _newRoomConfiguration.ConfigurePortal(PortalPosition.South, _previousRoomConfiguration.GetOppositePortal(PortalPosition.South).transform);
            _previousRoomConfiguration.ConfigurePortal(PortalPosition.North, _newRoomConfiguration.GetOppositePortal(PortalPosition.North).transform);
        }
    }

    public void GetMapSize()
    {
        for (int i = 0; i < mapLayout.Length; i++)
        {
            if (mapLayout[i] == '\n')
            {
                if (mapSize.x == 0)
                    mapSize.x = i;

                mapSize.y++;
            }
        }
    }
}
