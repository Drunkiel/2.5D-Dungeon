using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeSaveData
{
    public string GUID;
    public NodeTypes nodeType;
    public Vector2 position;
    public short entityID;
    public string text;
    public List<ChoiceSaveData> choices = new();
    public DialogEvent action;
    public DialogCondition condition;
}