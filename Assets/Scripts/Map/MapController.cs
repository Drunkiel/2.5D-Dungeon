using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room
{
    public bool isVisible;
    public GameObject roomObject;
}

public class MapController : MonoBehaviour
{
    public List<Room> _rooms = new();
    public int currentRoomIndex;

    public void ChangeRoom(int index)
    {
        ActivateRoom(index);
        Invoke(nameof(DisableRoom), 1f);
        currentRoomIndex = index;
    }

    private void ActivateRoom(int index)
    {
        _rooms[index].isVisible = true;
        _rooms[index].roomObject.SetActive(true);
    }

    private void DisableRoom()
    {
        _rooms[currentRoomIndex].isVisible = false;
        _rooms[currentRoomIndex].roomObject.SetActive(false);
    }
}
