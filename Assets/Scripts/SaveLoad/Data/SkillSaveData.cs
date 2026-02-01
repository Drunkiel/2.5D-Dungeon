using System.Collections.Generic;

[System.Serializable]
public class SkillSaveData
{
    public List<int> skillIDs = new();

    public SkillSaveData() { }

    public SkillSaveData(List<InventorySlot> _skillSlots)
    {
        skillIDs.Clear();
        for (int i = 0; i < _skillSlots.Count; i++)
        {
            if (_skillSlots[i]._itemID != null && _skillSlots[i]._itemID._skillDataParser != null)
                skillIDs.Add(_skillSlots[i]._itemID._skillDataParser._skillData.ID);
            else
                skillIDs.Add(0);
        }
    }
}
