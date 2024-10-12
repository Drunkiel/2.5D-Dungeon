using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float interval;
    
    // Start is called before the first frame update
    void Start()
    {
        if (interval > 0)
            Destroy(gameObject, interval);
    }

    public void InstantDestroy()
    {
        Destroy(gameObject);
    }
}
