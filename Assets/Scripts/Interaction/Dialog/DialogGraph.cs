using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dialog/Dialog Graph")]
public class DialogGraph : ScriptableObject
{
    public List<NodeSaveData> nodes = new();
    public List<EdgeSaveData> edges = new();
}