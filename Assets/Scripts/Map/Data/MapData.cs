using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Maps/Map data")]
public class MapData : ScriptableObject
{   
    public short mapID;
    public int distanceBetweenRooms = 10;
    [Multiline]
    public string mapLayout = "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n";
}
