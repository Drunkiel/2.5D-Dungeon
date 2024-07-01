using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject roomPrefab;
    public string mapLayout = "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n" +
                              "0,1,0\n";
    public string mapLayoutCopy;

    public Vector2Int mapSize;

    [SerializeField] private List<GameObject> spawnedRooms = new();

    private void Start()
    {
        if (CheckMap())
            GenerateMap();
    }

    public void GenerateMap()
    {
        mapLayoutCopy = ExtractNumbers(mapLayout);

        int index = 0;

        // Loop through the map size starting from bottom-left
        for (int y = mapSize.y - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                char number = mapLayoutCopy[index];

                if (number != '0')
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
    }

    private void SpawnPortals()
    {
        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            if (Vector3.Distance(spawnedRooms[i].transform.position, startRoom.transform.position) <= 10)
                print($"{spawnedRooms[i]}");

            for (int j = 0; j < spawnedRooms.Count; j++)
            {
                //if i and j are equal then skip
                if (i == j)
                    j++;

                //Checks if j is out of bounds
                if (j >= spawnedRooms.Count)
                    return;

                //Checks the distance between rooms
                if (Vector3.Distance(spawnedRooms[i].transform.position, spawnedRooms[j].transform.position) <= 10)
                    print($"{spawnedRooms[i].name} is nearby {spawnedRooms[j].name}");
            }
        }
    }

    public bool CheckMap()
    {
        int collumnNumber = 0; //Checker if map is correctly made

        for (int i = 0; i < mapLayout.Length; i++)
        {
            if (mapLayout[i] != ',' && mapLayout[i] != '\n')
                collumnNumber++;

            //Getting to the last collumn
            if (mapLayout[i] == '\n')
            {
                if (mapSize.x == 0)
                    mapSize.x = (i + 1) / 2;

                mapSize.y++;

                //Checking if every row is the same size
                if (collumnNumber != mapSize.x)
                {
                    Debug.LogError($"There is diffrent row length on row: {mapSize.y}");
                    return false;
                }
                else
                    collumnNumber = 0;
            }
        }

        return true;
    }

    string ExtractNumbers(string input)
    {
        string[] rows = input.Split('\n');
        string numbersOnly = "";

        // Loop through each row
        foreach (string row in rows)
        {
            // Split the row by commas
            string[] numbers = row.Split(',');

            // Loop through each number in the row
            foreach (string number in numbers)
                numbersOnly += number;
        }

        return numbersOnly;
    }
}
