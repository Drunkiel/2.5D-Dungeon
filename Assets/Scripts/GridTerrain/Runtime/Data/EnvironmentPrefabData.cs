using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentPrefabData
{
    public string name;
    public GameObject prefab;
    public int materialIdToUse = 0;
    public List<Material> materials = new();
    public bool randomScale = true;
    public float minScale = 1f;
    public float maxScale = 2f;
    public bool randomYRotation = true;
}