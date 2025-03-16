using UnityEngine;

public class PlacableObject : MonoBehaviour
{
    public bool isPlaced;
    [HideInInspector] public Vector3 size;
    public GameObject objectToManipulate;
    public InteractableObject _interactableObject;

    private void Start()
    {
        CalculateSizeInCells();
    }

    public virtual void Place()
    {
        Destroy(GetComponent<ObjectDrag>());
        Destroy(GetComponent<TriggerController>());
        transform.GetComponentInChildren<AutoSize>().AutoDestroy();
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        BuildingSystem.instance._objectToPlace = null;

        if (objectToManipulate != null)
            objectToManipulate.SetActive(true);

        isPlaced = true;
    }

    public virtual void Move()
    {
        if (BuildingSystem.inBuildingMode)
            return;

        BuildingSystem.inBuildingMode = true;

        isPlaced = false;
        BuildingSystem.instance._objectToPlace = this;
        BuildingSystem.instance.OpenUI(false);
        transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

        if (objectToManipulate != null)
            objectToManipulate.SetActive(false);

        gameObject.AddComponent<ObjectDrag>();
        TriggerController triggerController = gameObject.AddComponent<TriggerController>();
        triggerController.objectsTag = new() { "Finish" };
        triggerController.isTriggered = true;
        triggerController.reverseReturn = true;
        Instantiate(BuildingSystem.instance.buildingMaterial, transform);
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public void Rotate(int angle)
    {
        transform.RotateAround(transform.position, Vector3.up, angle);
    }

    private void CalculateSizeInCells()
    {
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        size = new Vector3(collider.size.x, collider.size.y, collider.size.z);
    }
}