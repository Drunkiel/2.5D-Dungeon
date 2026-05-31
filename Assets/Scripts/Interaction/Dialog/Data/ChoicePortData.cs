using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ChoicePortData
{
    public string GUID;
    [TextArea(2, 5)]
    public string text;
    public UnityEvent actionToDo;
}