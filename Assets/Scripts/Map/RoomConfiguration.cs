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

    [SerializeField] private GameObject portalWest;
    [SerializeField] private GameObject portalNorth;
    [SerializeField] private GameObject portalEast;
    [SerializeField] private GameObject portalSouth;

    public List<RoomConfiguration> nearbyRooms = new();

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

            switch (portalPositions[i])
            {
                case PortalPosition.West:
                    if (portalWest != null)
                        return;

                    portalWest = newPortal;
                    newPortal.transform.localPosition = new(-roomSize.x / 2 + 1, 0.5f, 0);
                    break;

                case PortalPosition.North:
                    if (portalNorth != null)
                        return;

                    portalNorth = newPortal;
                    newPortal.transform.localPosition = new(0, 0.5f, roomSize.y / 2 - 1);
                    break;

                case PortalPosition.East:
                    if (portalEast != null)
                        return;

                    portalEast = newPortal;
                    newPortal.transform.localPosition = new(roomSize.x / 2 - 1, 0.5f, 0);
                    break;

                case PortalPosition.South:
                    if (portalSouth != null)
                        return;

                    portalSouth = newPortal;
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
        {
            Debug.Log($"{gameObject.name} does not have any portals");
            return;
        }

        portal.GetComponent<EventTriggerController>().enterEvent.AddListener(() =>
        {
            portal.GetComponent<TeleportEvent>().TeleportToObject(objectToTeleport);
        });
    }

    public GameObject GetPortal(PortalPosition portalPosition)
    {
        switch (portalPosition)
        {
            case PortalPosition.West:
                if (portalWest == null)
                    return null;

                return portalWest;

            case PortalPosition.North:
                if (portalNorth == null)
                    return null;

                return portalNorth;

            case PortalPosition.East:
                if (portalEast == null)
                    return null;

                return portalEast;

            case PortalPosition.South:
                if (portalSouth == null)
                    return null;

                return portalSouth;
        }

        return null;
    }

    public GameObject GetOppositePortal(PortalPosition portalPosition)
    {
        return portalPosition switch
        {
            PortalPosition.West => portalEast,
            PortalPosition.North => portalSouth,
            PortalPosition.East => portalWest,
            PortalPosition.South => portalNorth,
            _ => null,
        };
    }
}
