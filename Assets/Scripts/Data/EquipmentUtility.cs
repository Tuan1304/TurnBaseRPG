public static class EquipmentUtility
{
    public static HeroStats GetTotalStats(
        HeroData hero,
        InventoryManager inventoryManager)
    {
        HeroStats total =
            HeroStatsUtility.Clone(
                hero.stats);

        ApplyEquipmentStats(
            total,
            GetEquippedItem(
                hero.equipment,
                EquipmentSlot.Weapon,
                inventoryManager));

        ApplyEquipmentStats(
            total,
            GetEquippedItem(
                hero.equipment,
                EquipmentSlot.Armor,
                inventoryManager));

        ApplyEquipmentStats(
            total,
            GetEquippedItem(
                hero.equipment,
                EquipmentSlot.Helmet,
                inventoryManager));

        ApplyEquipmentStats(
            total,
            GetEquippedItem(
                hero.equipment,
                EquipmentSlot.Pants,
                inventoryManager));

        return total;
    }

    public static InventoryItemSaveData GetEquippedItem(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot,
        InventoryManager inventoryManager)
    {
        string instanceID =
            GetEquippedInstanceID(
                equipment,
                slot);

        if (string.IsNullOrEmpty(instanceID))
            return null;

        if (inventoryManager != null)
        {
            InventoryItemSaveData item =
                inventoryManager.GetInventoryItem(
                    instanceID);

            if (item != null)
                return item;
        }

        if (GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null)
        {
            return null;
        }

        return GameDataManager
            .Instance
            .Data
            .inventory
            .Find(
                item =>
                    item != null &&
                    item.instanceID == instanceID);
    }

    private static string GetEquippedInstanceID(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot)
    {
        if (equipment == null)
            return "";

        switch (slot)
        {
            case EquipmentSlot.Weapon:

                return equipment.weaponInstanceID;

            case EquipmentSlot.Armor:

                return equipment.armorInstanceID;

            case EquipmentSlot.Helmet:

                return equipment.helmetInstanceID;

            case EquipmentSlot.Pants:

                return equipment.pantsInstanceID;
        }

        return "";
    }

    private static void ApplyEquipmentStats(
        HeroStats total,
        InventoryItemSaveData item)
    {
        if (item == null ||
            item.bonusStats == null)
        {
            return;
        }

        total.hp += item.bonusStats.hp;
        total.attack += item.bonusStats.attack;
        total.defense += item.bonusStats.defense;
        total.magicDefense += item.bonusStats.magicDefense;
        total.abilityPower += item.bonusStats.abilityPower;
        total.critChance += item.bonusStats.critChance;
        total.evade += item.bonusStats.evade;
    }
}
