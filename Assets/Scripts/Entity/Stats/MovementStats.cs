using System.Collections.Generic;

[System.Serializable]
public class MovementStats
{
    public float speedForce;
    public float maxSpeed = 1.2f;
    public float jumpForce;

    public List<bool> additionalJumps = new();
}