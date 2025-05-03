using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MiniMapConfig
{
    public Sprite mapSprite;
    public Vector2 mapWorldSize;
    public Vector2 worldOffset;
}

[Serializable]
public class NPCIcon
{
    public int npcID;
    public RectTransform rectTransform;

    public NPCIcon(int npcID, RectTransform rectTransform)
    {
        this.npcID = npcID;
        this.rectTransform = rectTransform;
    }
}

public class MiniMapController : MonoBehaviour
{
    public static MiniMapController instance;

    public RectTransform minimapRect;
    private Transform playerTransform;        
    public RectTransform playerIcon;
    [SerializeField] private GameObject npcIconPrefab;

    [SerializeField] private int configIndex;
    public List<MiniMapConfig> _miniMapConfigs = new();
    [SerializeField] private List<NPCIcon> _npcIcons = new();

    private void Start()
    {
        playerTransform = PlayerController.instance.transform;
        instance = this;
        SetNewMiniMap(0);
    }

    void Update()
    {
        UpdatePlayerIconPosition();
    }

    private void UpdatePlayerIconPosition()
    {
        Vector2 worldPos = new Vector2(playerTransform.position.x, playerTransform.position.z) - _miniMapConfigs[configIndex].worldOffset;

        float xRatio = worldPos.x / _miniMapConfigs[configIndex].mapWorldSize.x;
        float yRatio = worldPos.y / _miniMapConfigs[configIndex].mapWorldSize.y;

        float uiX = xRatio * minimapRect.rect.width;
        float uiY = yRatio * minimapRect.rect.height;

        playerIcon.anchoredPosition = new Vector2(uiX, uiY);
        playerIcon.rotation = Quaternion.Euler(0, 0, -playerTransform.eulerAngles.y);
    }

    public void AddNPCIcon(EntityController _entityController)
    {
        Instantiate(npcIconPrefab);
        //RectTransform rectTransform = Instantiate(npcIconPrefab).GetComponent<RectTransform>();
        //NPCIcon newIcon = new(_entityController._entityInfo.ID, rectTransform);
        //_npcIcons.Add(newIcon);
    }

    public void SetNewMiniMap(int index)
    {
        configIndex = index;
        minimapRect.GetComponent<Image>().sprite = _miniMapConfigs[configIndex].mapSprite;
    }
}
