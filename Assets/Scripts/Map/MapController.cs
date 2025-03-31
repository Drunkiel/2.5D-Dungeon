using System.Collections;
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

    private void Start()
    {
        for (int i = 0; i < _rooms.Count; i++)
        {
            if (!_rooms[i].isVisible)
                _rooms[i].roomObject.SetActive(false);
        }
    }

    public void ChangeRoom(int index)
    {
        StartCoroutine(RoomChanger(index));
    }

    private IEnumerator RoomChanger(int index)
    {
        yield return new WaitForSeconds(1);
        ActivateRoom(index);
        yield return new WaitForSeconds(1);
        DisableRoom(currentRoomIndex);
        currentRoomIndex = index;
    }

    private void ActivateRoom(int index)
    {
        _rooms[index].isVisible = true;
        _rooms[index].roomObject.SetActive(true);
    }

    private void DisableRoom(int index)
    {
        _rooms[index].isVisible = false;
        _rooms[index].roomObject.SetActive(false);
    }
}
