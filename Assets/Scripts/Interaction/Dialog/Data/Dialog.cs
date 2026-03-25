using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Dialog
{
    public int ID;
    public short entityID;
    [TextArea(3, 10)]
    public string text;
    public List<DialogResponseOptions> _responseOptions = new();
}

[Serializable]
public class DialogResponseOptions
{
    [TextArea(3, 10)]
    public string text;
    public UnityEvent actionToDo;
}
