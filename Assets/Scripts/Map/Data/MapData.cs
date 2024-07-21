using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom/Maps/Map data")]
public class MapData : ScriptableObject
{   
    public short ID;
    public string displayedName;
    public Vector2Int size; //Automatically is assigned
    public int distanceBetweenRooms = 10;
    [Multiline]
    public string layout = "0,1,0\n" +
                            "0,1,0\n" +
                            "0,1,0\n" +
                            "0,1,0\n" +
                            "0,1,0\n";
}
