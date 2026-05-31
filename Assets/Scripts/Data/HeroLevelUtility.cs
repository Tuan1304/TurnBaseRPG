using UnityEngine;

public static class HeroLevelUtility
{
    public const int MaxLevel = 50;

    public static int GetRequiredExp(
        int level)
    {
        level =
            Mathf.Max(
                1,
                level);

        return 100 +
            ((level - 1) * 50);
    }

    public static bool AddExp(
        HeroData hero,
        int amount)
    {
        if (hero == null ||
            amount <= 0)
        {
            return false;
        }

        if (hero.level <= 0)
        {
            hero.level = 1;
        }

        if (hero.level >= MaxLevel)
        {
            hero.currentExp = 0;

            return false;
        }

        bool leveledUp = false;

        hero.currentExp += amount;

        while (hero.level < MaxLevel &&
            hero.currentExp >=
            GetRequiredExp(hero.level))
        {
            hero.currentExp -=
                GetRequiredExp(hero.level);

            LevelUp(hero);

            leveledUp = true;
        }

        if (hero.level >= MaxLevel)
        {
            hero.currentExp = 0;
        }

        return leveledUp;
    }

    private static void LevelUp(
        HeroData hero)
    {
        hero.level++;

        int oldMaxHP =
            hero.stats.hp;

        HeroStats growth =
            GetGrowth(hero);

        hero.stats.hp +=
            growth.hp;

        hero.stats.attack +=
            growth.attack;

        hero.stats.defense +=
            growth.defense;

        hero.stats.magicDefense +=
            growth.magicDefense;

        hero.stats.abilityPower +=
            growth.abilityPower;

        hero.stats.critChance +=
            growth.critChance;

        hero.stats.evade +=
            growth.evade;

        int hpIncrease =
            hero.stats.hp - oldMaxHP;

        hero.currentHP =
            Mathf.Min(
                hero.stats.hp,
                hero.currentHP + hpIncrease);
    }

    private static HeroStats GetGrowth(
        HeroData hero)
    {
        HeroStats growth =
            new HeroStats();

        growth.hp =
            Random.Range(5, 11);

        growth.attack =
            Random.Range(1, 4);

        growth.defense =
            Random.Range(1, 4);

        growth.magicDefense =
            Random.Range(0, 3);

        growth.abilityPower =
            Random.Range(0, 3);

        growth.critChance = 0f;
        growth.evade = 0f;

        if (hero == null ||
            hero.unitData == null)
        {
            return growth;
        }

        switch (hero.unitData.unitClass)
        {
            case UnitClass.Warrior:

                growth.hp =
                    Random.Range(8, 15);

                growth.attack =
                    Random.Range(1, 4);

                growth.defense =
                    Random.Range(1, 4);

                growth.magicDefense =
                    Random.Range(0, 3);

                growth.abilityPower = 0;

                break;

            case UnitClass.Archer:

                growth.hp =
                    Random.Range(5, 11);

                growth.attack =
                    Random.Range(2, 5);

                growth.defense =
                    Random.Range(1, 3);

                growth.magicDefense =
                    Random.Range(0, 3);

                growth.abilityPower = 0;

                growth.critChance =
                    Random.Range(0f, 0.31f);

                break;

            case UnitClass.Mage:

                growth.hp =
                    Random.Range(4, 9);

                growth.attack =
                    Random.Range(0, 3);

                growth.defense =
                    Random.Range(0, 3);

                growth.magicDefense =
                    Random.Range(1, 4);

                growth.abilityPower =
                    Random.Range(2, 6);

                break;
        }

        ApplyRarityBonus(
            hero.unitData.rarity,
            growth);

        return growth;
    }

    private static void ApplyRarityBonus(
        Rarity rarity,
        HeroStats growth)
    {
        float multiplier = 1f;

        switch (rarity)
        {
            case Rarity.Rare:

                multiplier = 1.1f;

                break;

            case Rarity.Epic:

                multiplier = 1.2f;

                break;

            case Rarity.Legendary:

                multiplier = 1.35f;

                break;
        }

        growth.hp =
            Mathf.RoundToInt(
                growth.hp * multiplier);

        growth.attack =
            Mathf.RoundToInt(
                growth.attack * multiplier);

        growth.defense =
            Mathf.RoundToInt(
                growth.defense * multiplier);

        growth.magicDefense =
            Mathf.RoundToInt(
                growth.magicDefense * multiplier);

        growth.abilityPower =
            Mathf.RoundToInt(
                growth.abilityPower * multiplier);
    }
}
