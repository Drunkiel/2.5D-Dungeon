using UnityEngine;

public class AutoSize : MonoBehaviour
{
    [SerializeField] private Vector3 additionalSize = new(0.02f, 0.02f, 0.02f);

    private void Start()
    {
        transform.localScale = GetScale();
        transform.localPosition = GetPosition();
    }

    private Vector3 GetScale()
    {
        return transform.parent.GetComponent<PlacableObject>().size + additionalSize;
    }

    private Vector3 GetPosition()
    {
        return transform.localPosition + transform.parent.GetComponent<BoxCollider>().center;
    }

    public void AutoDestroy()
    {
        Destroy(gameObject);
    }
}