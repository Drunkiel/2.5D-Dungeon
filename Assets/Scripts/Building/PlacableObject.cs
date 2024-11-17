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

        if (objectToManipulate != null)
            objectToManipulate.SetActive(false);

        gameObject.AddComponent<ObjectDrag>();
        Instantiate(BuildingSystem.instance.buildingMaterial, transform);
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