using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dialog/Dialog data")]
public class DialogData : ScriptableObject
{
    public List<string> dialogs = new();
}
