using System.Collections.Generic;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    public List<GameObject> allBuildings = new();
    [SerializeField] private Transform parent;
    [SerializeField] private BuildingCard buildingCardPrefab;
    private bool isOpen;
    public Animator animator;

    void Start()
    {
        SpawnCards();
    }

    public void SpawnCards()
    {
        for (int i = 0; i < allBuildings.Count; i++)
        {
            BuildingCard _singleCard = Instantiate(buildingCardPrefab, parent);
            BuildingID _buildingID = allBuildings[i].GetComponent<BuildingID>();
            int index = i;
            _singleCard.AssignData(_buildingID.showcaseImage, _buildingID.buildingName, () => BuildingSystem.instance.InitializeWithObject(allBuildings[index]));
        }
    }

    public void PlayAnimation()
    {
        isOpen = !isOpen;
        if (isOpen)
            animator.Play("CloseMenu");
        else
            animator.Play("OpenMenu");
    }

    public void Finish()
    {
        BuildingSystem _buildingSystem = BuildingSystem.instance;

        for (int i = 0; i < _buildingSystem.parent.childCount; i++)
        {
            GameObject part = _buildingSystem.parent.transform.GetChild(i).gameObject;
            Destroy(part.transform.GetChild(0).gameObject);
            Destroy(part.GetComponent<Rigidbody>());
            Destroy(part.GetComponent<BuildingID>());
            Destroy(part.GetComponent<PlacableObject>());
            Destroy(part.GetComponent<BoxCollider>());
        }
    }
}
