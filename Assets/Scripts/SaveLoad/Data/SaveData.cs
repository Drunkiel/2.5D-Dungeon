[System.Serializable]
public class SaveData
{
    public string skinPath;
    public string sceneName;
    public EasyVector3 position;
    public int lumens;
    public InventoryData _inventoryData;
    public SkillSaveData _skillData;
    public string gameVersion;

    public SaveData(string version)
    {
        skinPath = "Player/Human";
        sceneName = "ValVille";
        position = new(-0.5f, 0.2f, -23);
        lumens = 0;
        _inventoryData = new(new(), InventoryController.instance._inventorySlots);
        _skillData = new(InventoryController.instance._skillSlots);
        gameVersion = version;
    }
}