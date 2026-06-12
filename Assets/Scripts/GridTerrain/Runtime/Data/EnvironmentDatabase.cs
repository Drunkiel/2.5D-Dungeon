using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid Terrain/Environment Database")]
public class EnvironmentDatabase : ScriptableObject
{
    public List<EnvironmentCategory> categories = new();
}