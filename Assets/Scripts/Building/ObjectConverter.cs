using System.Collections.Generic;
using UnityEngine;

public class ObjectConverter : MonoBehaviour
{
    public List<GameObject> objectsToConvert = new();
    public List<Sprite> sprites = new();
    public GameObject target;

    private void Start()
    {
        //Convert();
    }

    public void Convert()
    {
        for (int i = 0; i < objectsToConvert.Count; i++)
        {
            GameObject model = Instantiate(objectsToConvert[i]);
            GameObject newObject = Instantiate(target);
            model.transform.parent = newObject.transform;
            model.name = model.name[..^7];
            newObject.name = model.name;
            newObject.GetComponent<BuildingID>().buildingName = model.name;
            int index = i;
            newObject.GetComponent<BuildingID>().showcaseImage = sprites[index];
        }
    }
}
