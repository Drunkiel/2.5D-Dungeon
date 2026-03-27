using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeleportData
{
    public short ID;
    public GameObject teleportObject;
}

public class TeleportDataHolder : MonoBehaviour
{
    public List<TeleportData> _teleportDatas = new();
}
