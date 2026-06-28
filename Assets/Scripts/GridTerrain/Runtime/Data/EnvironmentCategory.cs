using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Grid Terrain/Environment Category")]
public class EnvironmentCategory : ScriptableObject
{
    public string categoryName;
    public List<EnvironmentPrefabData> prefabs = new();
}