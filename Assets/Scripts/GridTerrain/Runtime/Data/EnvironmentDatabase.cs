using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Grid Terrain/Environment Database")]
public class EnvironmentDatabase : ScriptableObject
{
    public List<EnvironmentCategory> categories = new();
}