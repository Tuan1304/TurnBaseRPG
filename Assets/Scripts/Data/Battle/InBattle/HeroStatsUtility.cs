public static class HeroStatsUtility
{
    public static HeroStats Clone(
        HeroStats source)
    {
        HeroStats copy =
            new HeroStats();

        copy.hp =
            source.hp;

        copy.attack =
            source.attack;

        copy.defense =
            source.defense;

        copy.magicDefense =
            source.magicDefense;

        copy.abilityPower =
            source.abilityPower;

        copy.critChance =
            source.critChance;

        copy.evade =
            source.evade;

        return copy;
    }
}