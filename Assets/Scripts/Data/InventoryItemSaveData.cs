using System;

[Serializable]
public class InventoryItemSaveData
{
    public string instanceID;

    public string itemID;

    public Rarity rarity;

    public EquipmentStats bonusStats =
        new EquipmentStats();
}
