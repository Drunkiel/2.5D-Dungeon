using System;
using UnityEngine;

[Serializable]
public class DialogNodeData
{
    public string GUID;
    public Rect position;
    public short entityID;

    [TextArea(3, 10)]
    public string text;
}