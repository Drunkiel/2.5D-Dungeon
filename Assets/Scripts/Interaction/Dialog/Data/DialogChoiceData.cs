using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogChoiceData
{
    public string GUID;
    public Rect position;
    public List<ChoicePortData> choices = new();
}