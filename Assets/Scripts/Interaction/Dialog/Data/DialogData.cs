using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Dialog
{
    public string text;
    public List<Response> responses = new();
}

[Serializable]
public class Response
{
    public string text;
    public Dialog nextDialog;   
    public UnityEvent unityEvent;
}

[CreateAssetMenu(menuName = "Custom/Dialog/Dialog data")]
public class DialogData : ScriptableObject
{
    public List<Dialog> dialogs = new();
}
