using UnityEngine;

public class EasyVector3
{
    public float x;
    public float y;
    public float z;

    public EasyVector3() { }

    public EasyVector3(float x, float y, float z)
    {
        this.x = Mathf.Round(x * 100f) / 100f;
        this.y = Mathf.Round(y * 100f) / 100f;
        this.z = Mathf.Round(z * 100f) / 100f;
    }

    public EasyVector3(Vector3 value)
    {
        x = Mathf.Round(value.x * 100f) / 100f;
        y = Mathf.Round(value.y * 100f) / 100f;
        z = Mathf.Round(value.z * 100f) / 100f;
    }

    public Vector3 ConvertToVector3()
    {
        return new(x, y, z);
    }
}