using System.Collections.Generic;
using UnityEngine;

public static class SkillResolver
{
    public static List<BattleUnit> GetTargets(
        SkillData skill,
        BattleUnit caster,
        BattleUnit primaryTarget)
    {
        List<BattleUnit> result = new List<BattleUnit>();

        List<BattleUnit> enemies =
            GetEnemyUnits(caster);

        List<BattleUnit> allies =
            GetAllyUnits(caster);

        switch (skill.targetType)
        {
            // =====================
            // 1. SINGLE ENEMY
            // =====================
            case TargetType.EnemySingle:
                if (primaryTarget != null &&
                    primaryTarget.teamType !=
                    caster.teamType)
                {
                    result.Add(primaryTarget);
                }
                break;

            // =====================
            // 2. AOE ENEMY
            // =====================
            case TargetType.EnemyAOE:
                AddUnitsInArea(
                    result,
                    enemies,
                    primaryTarget,
                    skill.aoeRadius);
                break;

            // =====================
            // 3. SELF
            // =====================
            case TargetType.Self:
                result.Add(caster);
                break;

            // =====================
            // 4. SINGLE ALLY
            // =====================
            case TargetType.AllySingle:
                if (primaryTarget != null &&
                    primaryTarget.teamType ==
                    caster.teamType)
                {
                    result.Add(primaryTarget);
                }
                break;

            // =====================
            // 5. AOE ALLY
            // =====================
            case TargetType.AllyAOE:
                AddUnitsInArea(
                    result,
                    allies,
                    primaryTarget,
                    skill.aoeRadius);
                break;

            // =====================
            // 6. ALL ENEMIES
            // =====================
            case TargetType.AllEnemies:
                result.AddRange(enemies);
                break;

            // =====================
            // 7. ALL ALLIES
            // =====================
            case TargetType.AllAllies:
                result.AddRange(allies);
                break;
        }

        return result;
    }

    private static List<BattleUnit> GetEnemyUnits(
        BattleUnit caster)
    {
        if (caster.teamType == TeamType.Player)
        {
            return TurnManager.Instance
                .GetEnemyUnits();
        }

        return TurnManager.Instance
            .GetPlayerUnits();
    }

    private static List<BattleUnit> GetAllyUnits(
        BattleUnit caster)
    {
        if (caster.teamType == TeamType.Player)
        {
            return TurnManager.Instance
                .GetPlayerUnits();
        }

        return TurnManager.Instance
            .GetEnemyUnits();
    }

    private static void AddUnitsInArea(
        List<BattleUnit> result,
        List<BattleUnit> candidates,
        BattleUnit center,
        int radius)
    {
        if (center == null)
            return;

        foreach (BattleUnit unit in candidates)
        {
            if (unit == null)
                continue;

            int distance =
                Mathf.Abs(
                    unit.gridPos.x -
                    center.gridPos.x)
                +
                Mathf.Abs(
                    unit.gridPos.y -
                    center.gridPos.y);

            if (distance <= radius)
            {
                result.Add(unit);
            }
        }
    }
}
