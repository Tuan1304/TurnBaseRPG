using System.Collections.Generic;
using UnityEngine;

public static class HeroSerializer
{
    // =========================
    // SAVE
    // =========================

    public static HeroSaveData
        ToSaveData(HeroData hero)
    {
        HeroSaveData data =
            new HeroSaveData();

        data.heroID =
            hero.heroID;

        data.unitID =
            GetUnitID(
                hero);

        data.appearanceID =
            GetAppearanceID(
                hero.appearanceData);

        data.weaponType =
            hero.weaponType;

        data.level =
            hero.level;

        data.currentExp =
            hero.currentExp;

        // =====================
        // STATS
        // =====================

        data.stats.hp =
            hero.stats.hp;

        data.stats.attack =
            hero.stats.attack;

        data.stats.defense =
            hero.stats.defense;

        data.stats.magicDefense =
            hero.stats.magicDefense;

        data.stats.abilityPower =
            hero.stats.abilityPower;

        data.stats.critChance =
            hero.stats.critChance;

        data.stats.evade =
            hero.stats.evade;

        data.currentHP =
            hero.currentHP;

        data.isActive =
            hero.heroState ==
            HeroState.Active;

        data.equipment =
            hero.equipment;

        return data;
    }

    // =========================
    // LOAD
    // =========================

    public static HeroData
        ToHeroData(
            HeroSaveData save,
            UnitDatabase database,
            List<CharacterAppearanceData> appearances = null)
    {
        UnitData unit =
            FindUnit(
                save,
                database);

        if (unit == null)
        {
            Debug.LogError(
                "Missing UnitData: "
                + save.unitID);

            return null;
        }

        HeroData hero =
            new HeroData();

        hero.heroID =
            save.heroID;

        hero.unitID =
            save.unitID;

        hero.unitData =
            unit;

        hero.appearanceData =
            FindAppearance(
                save.appearanceID,
                appearances,
                unit.unitClass);

        hero.weaponType =
            save.weaponType;

        hero.portrait =
            unit.avatar;

        hero.level =
            save.level;

        if (hero.level <= 0)
        {
            hero.level = 1;
        }

        hero.currentExp =
            save.currentExp;

        // =====================
        // STATS
        // =====================

        hero.stats.hp =
            save.stats.hp;

        hero.stats.attack =
            save.stats.attack;

        hero.stats.defense =
            save.stats.defense;

        hero.stats.magicDefense =
            save.stats.magicDefense;

        hero.stats.abilityPower =
            save.stats.abilityPower;

        hero.stats.critChance =
            save.stats.critChance;

        hero.stats.evade =
            save.stats.evade;

        hero.currentHP =
            save.currentHP;

        hero.heroState =
            save.isActive
            ? HeroState.Active
            : HeroState.Waiting;

        hero.equipment =
            save.equipment != null
            ? save.equipment
            : new HeroEquipmentSaveData();

        return hero;
    }

    private static string GetAppearanceID(
        CharacterAppearanceData appearance)
    {
        if (appearance == null)
            return "";

        if (!string.IsNullOrEmpty(
            appearance.id))
        {
            return appearance.id;
        }

        return appearance.name;
    }

    private static string GetUnitID(
        HeroData hero)
    {
        if (hero.unitData != null)
        {
            return hero.unitData.name;
        }

        return hero.unitID;
    }

    private static UnitData FindUnit(
        HeroSaveData save,
        UnitDatabase database)
    {
        if (database == null ||
            database.units == null)
        {
            return null;
        }

        UnitData match =
            database.units.Find(
                unit =>
                    unit != null &&
                    unit.name == save.unitID);

        if (match != null)
            return match;

        List<UnitData> idMatches =
            database.units.FindAll(
                unit =>
                    unit != null &&
                    unit.id == save.unitID);

        if (idMatches.Count == 1)
            return idMatches[0];

        if (idMatches.Count > 1)
        {
            UnitClass expectedClass =
                GetClassFromWeapon(
                    save.weaponType);

            UnitData bestMatch = null;

            int bestScore = int.MaxValue;

            foreach (UnitData unit
                in idMatches)
            {
                if (unit.unitClass !=
                    expectedClass)
                {
                    continue;
                }

                int score =
                    GetStatDistance(
                        unit.baseStats,
                        save.stats);

                if (score < bestScore)
                {
                    bestScore = score;

                    bestMatch = unit;
                }
            }

            if (bestMatch != null)
            {
                Debug.LogWarning(
                    "Duplicate UnitData id found: "
                    + save.unitID
                    + ". Loaded closest match: "
                    + bestMatch.name);

                return bestMatch;
            }

            Debug.LogWarning(
                "Duplicate UnitData id found: "
                + save.unitID
                + ". Loaded first match: "
                + idMatches[0].name);

            return idMatches[0];
        }

        return null;
    }

    private static UnitClass GetClassFromWeapon(
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

    private static int GetStatDistance(
        HeroStats baseStats,
        HeroStats savedStats)
    {
        if (baseStats == null ||
            savedStats == null)
        {
            return int.MaxValue;
        }

        return
            Mathf.Abs(baseStats.hp - savedStats.hp) +
            Mathf.Abs(baseStats.attack - savedStats.attack) +
            Mathf.Abs(baseStats.defense - savedStats.defense) +
            Mathf.Abs(baseStats.magicDefense - savedStats.magicDefense) +
            Mathf.Abs(baseStats.abilityPower - savedStats.abilityPower);
    }

    private static CharacterAppearanceData
        FindAppearance(
            string appearanceID,
            List<CharacterAppearanceData> appearances,
            UnitClass fallbackClass)
    {
        if (appearances == null ||
            appearances.Count == 0)
        {
            return null;
        }

        CharacterAppearanceData match = null;

        if (!string.IsNullOrEmpty(
            appearanceID))
        {
            match = appearances.Find(
                appearance =>
                    appearance != null &&
                    (
                        appearance.id == appearanceID ||
                        appearance.name == appearanceID
                    ));
        }

        if (match != null)
            return match;

        return appearances.Find(
            appearance =>
                appearance != null &&
                appearance.allowedClass ==
                fallbackClass);
    }
}
