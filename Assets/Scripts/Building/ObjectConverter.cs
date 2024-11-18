using System.Collections.Generic;
using UnityEngine;

public class ObjectConverter : MonoBehaviour
{
    public List<GameObject> objectsToConvert;
    public GameObject target;

    public void Convert()
    {
        foreach (GameObject singleObject in objectsToConvert)
        {
            GameObject model = Instantiate(singleObject);
            GameObject newObject = Instantiate(target);
            model.transform.parent = newObject.transform;
            model.name = model.name[..^7];
            newObject.name = model.name;
            newObject.GetComponent<BuildingID>().buildingName = model.name;
        }
    }
}
