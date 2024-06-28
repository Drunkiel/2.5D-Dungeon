using System.Collections.Generic;
using UnityEngine;

public enum PortalPosition
{
    West,
    North,
    East,
    South,
}

public class RoomConfiguration : MonoBehaviour
{
    [SerializeField] private Vector2 roomSize;

    //Portal config
    public List<PortalPosition> portalPositions { private set; get; } = new();
    [SerializeField] private GameObject portalPrefab;

    public List<GameObject> spawnedPortals = new();

    private void Awake()
    {
        roomSize = new(transform.GetChild(0).localScale.x, transform.GetChild(0).localScale.z);
    }

    public void Config(List<PortalPosition> portalPositions)
    {
        this.portalPositions = portalPositions;

        SpawnPortals();
    }

    private void SpawnPortals()
    {
        for (int i = 0; i < portalPositions.Count; i++)
        {
            GameObject newPortal = Instantiate(portalPrefab, transform);
            spawnedPortals.Add(newPortal);

            switch (portalPositions[i])
            {
                case PortalPosition.West:
                    newPortal.transform.localPosition = new(-roomSize.x / 2 + 1, 0.5f, 0);
                    break;

                case PortalPosition.North:
                    newPortal.transform.localPosition = new(0, 0.5f, roomSize.y / 2 - 1);
                    break;

                case PortalPosition.East:
                    newPortal.transform.localPosition = new(roomSize.x / 2 - 1, 0.5f, 0);
                    break;

                case PortalPosition.South:
                    newPortal.transform.localPosition = new(0, 0.5f, -roomSize.y / 2 + 1);
                    break;
            }

            newPortal.transform.name = $"Portal_{portalPositions[i]}";
        }
    }

    public void ConfigurePortal(PortalPosition portalPosition, Transform objectToTeleport)
    {
        GameObject portal = GetPortal(portalPosition);

        if (portal == null) 
            return;

        portal.GetComponent<EventTriggerController>().enterEvent.AddListener(() =>
        {
            portal.GetComponent<TeleportEvent>().TeleportToObject(objectToTeleport);
        });
    }

    public GameObject GetPortal(PortalPosition portalPosition)
    {
        for (int i = 0; i < spawnedPortals.Count; i++)
        {
            if (spawnedPortals[i].transform.name[7] == portalPosition.ToString()[0])
                return spawnedPortals[i];
        }

        return null;
    }

    public GameObject GetOppositePortal(PortalPosition portalPosition)
    {
        return portalPosition switch
        {
            PortalPosition.West => GetPortal(PortalPosition.East),
            PortalPosition.North => GetPortal(PortalPosition.South),
            PortalPosition.East => GetPortal(PortalPosition.West),
            PortalPosition.South => GetPortal(PortalPosition.North),
            _ => null,
        };
    }
}
