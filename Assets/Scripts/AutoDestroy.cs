using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float interval;
    
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, interval);
    }
}
