using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Maps/Map data")]
public class MapData : ScriptableObject
{   
    public short mapID;
    public string mapName;
    public Vector2Int mapSize; //Automatically is assigned
    public int distanceBetweenRooms = 10;
    [Multiline]
    public string mapLayout = "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n";
}
