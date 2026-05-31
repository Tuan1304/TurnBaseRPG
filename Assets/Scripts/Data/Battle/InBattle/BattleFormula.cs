using UnityEngine;

public static class BattleFormula
{
    // =========================
    // ATTACK DAMAGE
    // =========================

    public static int CalculateAttackDamage(
        BattleUnit attacker,
        BattleUnit target,
        out bool isCrit)
    {
        isCrit = false;

        int damage =
            Mathf.Max(
                1,
                attacker.runtimeStats.attack -
                target.runtimeStats.defense);

        // =====================
        // CRIT
        // =====================

        float critChance =
            attacker.runtimeStats.critChance;

        int roll =
            Random.Range(0, 100);

        if (roll < critChance)
        {
            isCrit = true;

            damage *= 2;
        }

        return damage;
    }

    // =========================
    // EVADE
    // =========================

    public static bool RollEvade(
        BattleUnit target)
    {
        float evade =
            target.runtimeStats.evade;

        int roll =
            Random.Range(0, 100);

        return roll < evade;
    }

    // =========================
    // SKILL DAMAGE
    // =========================

    public static int CalculateSkillDamage(
        BattleUnit caster,
        BattleUnit target,
        SkillData skill)
    {
        int finalDamage = 0;

        switch (skill.damageType)
        {
            // =====================
            // PHYSICAL
            // =====================

            case DamageType.Physical:

                finalDamage =
                    Mathf.Max(
                        1,
                        caster.runtimeStats.attack +
                        skill.power -
                        target.runtimeStats.defense);

                break;

            // =====================
            // MAGICAL
            // =====================

            case DamageType.Magical:

                finalDamage =
                    Mathf.Max(
                        1,
                        caster.runtimeStats.abilityPower +
                        skill.power -
                        target.runtimeStats.magicDefense);

                break;

            // =====================
            // PURE
            // =====================

            case DamageType.Pure:

                finalDamage =
                    skill.power;

                break;
        }

        return finalDamage;
    }
}