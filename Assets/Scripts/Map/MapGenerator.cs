using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject roomPrefab;

    [SerializeField] private List<GameObject> spawnedRooms = new();

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject newRoom = Instantiate(roomPrefab, transform);
            spawnedRooms.Add(newRoom);
            newRoom.transform.localPosition = new(0, 0, 10 * i);
            RoomConfiguration _configuration = newRoom.GetComponent<RoomConfiguration>();
            _configuration.Config(new() { PortalPosition.North, PortalPosition.South });

            if (i == 1)
            {
                _configuration.ConfigurePortal(PortalPosition.South, startRoom.transform);
                startRoom.GetComponent<EventTriggerController>().enterEvent.AddListener(() =>
                {
                    startRoom.GetComponent<TeleportEvent>().TeleportToObject(_configuration.GetPortal(PortalPosition.South).transform);
                });
            }
            else
            {
                //_configuration.ConfigurePortal(PortalPosition.South, _configuration.GetOppositePortal(PortalPosition.South).transform);
                _configuration.ConfigurePortal(PortalPosition.South, spawnedRooms[i].GetComponent<RoomConfiguration>().GetOppositePortal(PortalPosition.South).transform);

            }
        }
    }
}
