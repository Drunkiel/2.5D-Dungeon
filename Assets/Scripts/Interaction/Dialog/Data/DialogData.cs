using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Custom/Dialog/Dialog data")]
public class DialogData : ScriptableObject
{
    public List<string> dialogs = new();
    public UnityEvent onEndDialogEvent;
}
