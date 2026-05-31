using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public static ItemDatabase CachedItemDatabase;

    public ItemDatabase itemDatabase;

    private void Awake()
    {
        Instance = this;

        if (itemDatabase != null)
        {
            CachedItemDatabase = itemDatabase;
        }
    }

    public static ItemData GetCachedItemData(
        InventoryItemSaveData itemSave)
    {
        if (itemSave == null ||
            CachedItemDatabase == null)
        {
            return null;
        }

        return CachedItemDatabase.GetItem(
            itemSave.itemID);
    }

    public List<InventoryItemSaveData> GetItemsBySlot(
        EquipmentSlot slot)
    {
        List<InventoryItemSaveData> result =
            new List<InventoryItemSaveData>();

        if (GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null ||
            itemDatabase == null)
        {
            return result;
        }

        foreach (InventoryItemSaveData itemSave
            in GameDataManager.Instance.Data.inventory)
        {
            ItemData itemData =
                itemDatabase.GetItem(
                    itemSave.itemID);

            if (itemData == null ||
                itemData.itemType != ItemType.Equipment ||
                itemData.equipmentSlot != slot)
            {
                continue;
            }

            result.Add(itemSave);
        }

        return result;
    }

    public InventoryItemSaveData GetInventoryItem(
        string instanceID)
    {
        if (string.IsNullOrEmpty(instanceID) ||
            GameDataManager.Instance == null ||
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
                    item.instanceID == instanceID);
    }

    public ItemData GetItemData(
        InventoryItemSaveData itemSave)
    {
        if (itemSave == null ||
            itemDatabase == null)
        {
            return null;
        }

        return itemDatabase.GetItem(
            itemSave.itemID);
    }

    public InventoryItemSaveData CraftItem(
        ItemData item)
    {
        if (item == null)
            return null;

        GameData data =
            GameDataManager.Instance.Data;

        if (data.gold < item.goldCost ||
            data.wood < item.woodCost ||
            data.stone < item.stoneCost ||
            data.food < item.foodCost)
        {
            return null;
        }

        data.gold -= item.goldCost;
        data.wood -= item.woodCost;
        data.stone -= item.stoneCost;
        data.food -= item.foodCost;

        InventoryItemSaveData craftedItem =
            CreateCraftedItem(item);

        data.inventory.Add(craftedItem);

        GameDataManager.Instance.SaveGame();

        return craftedItem;
    }

    public bool EquipItem(
        HeroData hero,
        InventoryItemSaveData itemSave)
    {
        if (hero == null ||
            itemSave == null)
        {
            return false;
        }

        ItemData itemData =
            GetItemData(itemSave);

        if (itemData == null ||
            itemData.itemType != ItemType.Equipment ||
            !CanHeroEquip(hero, itemData))
        {
            return false;
        }

        HeroStats oldStats =
            EquipmentUtility.GetTotalStats(
                hero,
                this);

        SetEquippedInstanceID(
            hero.equipment,
            itemData.equipmentSlot,
            itemSave.instanceID);

        HeroStats newStats =
            EquipmentUtility.GetTotalStats(
                hero,
                this);

        UpdateHeroHPAfterStatChange(
            hero,
            oldStats.hp,
            newStats.hp);

        UnitManager.Instance.SaveHeroes();

        return true;
    }

    public void UnequipItem(
        HeroData hero,
        EquipmentSlot slot)
    {
        if (hero == null)
            return;

        HeroStats oldStats =
            EquipmentUtility.GetTotalStats(
                hero,
                this);

        SetEquippedInstanceID(
            hero.equipment,
            slot,
            "");

        HeroStats newStats =
            EquipmentUtility.GetTotalStats(
                hero,
                this);

        UpdateHeroHPAfterStatChange(
            hero,
            oldStats.hp,
            newStats.hp);

        UnitManager.Instance.SaveHeroes();
    }

    public InventoryItemSaveData GetEquippedItem(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot)
    {
        if (equipment == null)
            return null;

        return GetInventoryItem(
            GetEquippedInstanceID(
                equipment,
                slot));
    }

    public bool CanHeroEquip(
        HeroData hero,
        ItemData itemData)
    {
        if (hero == null ||
            itemData == null)
        {
            return false;
        }

        if (itemData.equipmentSlot !=
            EquipmentSlot.Weapon)
        {
            return true;
        }

        return hero.unitData.unitClass ==
            GetClassFromWeapon(
                itemData.weaponType);
    }

    public bool IsEquippedByOtherHero(
        string instanceID,
        HeroData currentHero)
    {
        if (string.IsNullOrEmpty(instanceID) ||
            UnitManager.Instance == null)
        {
            return false;
        }

        foreach (HeroData hero
            in UnitManager.Instance.activeHeroes)
        {
            if (IsHeroUsingItem(
                hero,
                currentHero,
                instanceID))
            {
                return true;
            }
        }

        foreach (HeroData hero
            in UnitManager.Instance.waitingHeroes)
        {
            if (IsHeroUsingItem(
                hero,
                currentHero,
                instanceID))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsHeroUsingItem(
        HeroData hero,
        HeroData currentHero,
        string instanceID)
    {
        if (hero == null ||
            hero.equipment == null)
        {
            return false;
        }

        if (currentHero != null &&
            hero.heroID == currentHero.heroID)
        {
            return false;
        }

        return
            hero.equipment.weaponInstanceID == instanceID ||
            hero.equipment.armorInstanceID == instanceID ||
            hero.equipment.helmetInstanceID == instanceID ||
            hero.equipment.pantsInstanceID == instanceID;
    }

    private InventoryItemSaveData CreateCraftedItem(
        ItemData item)
    {
        InventoryItemSaveData craftedItem =
            new InventoryItemSaveData();

        craftedItem.instanceID =
            System.Guid.NewGuid().ToString();

        craftedItem.itemID =
            item.id;

        craftedItem.bonusStats =
            RollStats(item);

        craftedItem.rarity =
            CalculateRarity(
                item.bonusStats,
                craftedItem.bonusStats);

        return craftedItem;
    }

    private EquipmentStats RollStats(
        ItemData item)
    {
        EquipmentStats stats =
            new EquipmentStats();

        stats.hp =
            RollInt(
                item.bonusStats.hp,
                item.randomRange.hp);

        stats.attack =
            RollInt(
                item.bonusStats.attack,
                item.randomRange.attack);

        stats.defense =
            RollInt(
                item.bonusStats.defense,
                item.randomRange.defense);

        stats.magicDefense =
            RollInt(
                item.bonusStats.magicDefense,
                item.randomRange.magicDefense);

        stats.abilityPower =
            RollInt(
                item.bonusStats.abilityPower,
                item.randomRange.abilityPower);

        stats.critChance =
            RollFloat(
                item.bonusStats.critChance,
                item.randomRange.critChance);

        stats.evade =
            RollFloat(
                item.bonusStats.evade,
                item.randomRange.evade);

        return stats;
    }

    private int RollInt(
        int baseValue,
        int range)
    {
        return Mathf.Max(
            0,
            baseValue +
            Random.Range(
                -range,
                range + 1));
    }

    private float RollFloat(
        float baseValue,
        float range)
    {
        return Mathf.Max(
            0f,
            baseValue +
            Random.Range(
                -range,
                range));
    }

    private Rarity CalculateRarity(
        EquipmentStats baseStats,
        EquipmentStats rolledStats)
    {
        float baseScore =
            GetScore(baseStats);

        if (baseScore <= 0f)
            return Rarity.Common;

        float ratio =
            GetScore(rolledStats) /
            baseScore;

        if (ratio >= 2f)
            return Rarity.Legendary;

        if (ratio >= 1.5f)
            return Rarity.Epic;

        if (ratio >= 1f)
            return Rarity.Rare;

        return Rarity.Common;
    }

    private float GetScore(
        EquipmentStats stats)
    {
        if (stats == null)
            return 0f;

        return
            stats.hp +
            stats.attack +
            stats.defense +
            stats.magicDefense +
            stats.abilityPower +
            stats.critChance +
            stats.evade;
    }

    private void UpdateHeroHPAfterStatChange(
        HeroData hero,
        int oldMaxHP,
        int newMaxHP)
    {
        int delta =
            newMaxHP - oldMaxHP;

        hero.currentHP += delta;

        hero.currentHP =
            Mathf.Clamp(
                hero.currentHP,
                1,
                newMaxHP);
    }

    private string GetEquippedInstanceID(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot)
    {
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

    private void SetEquippedInstanceID(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot,
        string instanceID)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon:

                equipment.weaponInstanceID = instanceID;

                break;

            case EquipmentSlot.Armor:

                equipment.armorInstanceID = instanceID;

                break;

            case EquipmentSlot.Helmet:

                equipment.helmetInstanceID = instanceID;

                break;

            case EquipmentSlot.Pants:

                equipment.pantsInstanceID = instanceID;

                break;
        }
    }

    private UnitClass GetClassFromWeapon(
        WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Bow:

                return UnitClass.Archer;

            case WeaponType.Staff:

                return UnitClass.Mage;
        }

        return UnitClass.Warrior;
    }
}
