using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public BattleState state;

    private List<BattleUnit> playerUnits =
        new List<BattleUnit>();

    private List<BattleUnit> enemyUnits =
        new List<BattleUnit>();

    private bool isResolvingBattleResult;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // REGISTER
    // =========================

    public void RegisterPlayerUnit(
        BattleUnit unit)
    {
        if (!playerUnits.Contains(unit))
        {
            playerUnits.Add(unit);
        }
    }

    public void RegisterEnemyUnit(
        BattleUnit unit)
    {
        if (!enemyUnits.Contains(unit))
        {
            enemyUnits.Add(unit);
        }
    }

    // =========================
    // PLAYER TURN
    // =========================

    public void StartPlayerTurn()
    {
        if (state == BattleState.Win
            || state == BattleState.Lose)
        {
            return;
        }

        isResolvingBattleResult = false;

        RestoreAllUnitVisuals();

        state =
            BattleState.PlayerTurn;

        foreach (BattleUnit unit
            in playerUnits)
        {
            if (unit == null)
                continue;

            // cooldown

            if (unit.currentCooldown > 0)
            {
                unit.currentCooldown--;
            }

            // reset state

            unit.hasMoved =
                false;

            unit.hasAttacked =
                false;

            unit.state =
                BattleUnitState.Waiting;

            // status effect

            unit.UpdateStatusEffects();
        }

        // refresh ui

        if (TurnOrderUI.Instance != null)
        {
            TurnOrderUI.Instance
                .Refresh();
        }

        Debug.Log("PLAYER TURN");
    }

    // =========================
    // CHECK END PLAYER TURN
    // =========================

    public void CheckPlayerTurnEnd()
    {
        if (state == BattleState.Win
            || state == BattleState.Lose)
        {
            return;
        }

        if (isResolvingBattleResult)
        {
            return;
        }

        foreach (BattleUnit unit
            in playerUnits)
        {
            if (unit == null)
                continue;

            if (!unit.hasMoved
                || !unit.hasAttacked)
            {
                return;
            }
        }

        StartCoroutine(
            EnemyTurnRoutine());
    }

    // =========================
    // ENEMY TURN
    // =========================

    private IEnumerator EnemyTurnRoutine()
    {
        if (state == BattleState.Win
            || state == BattleState.Lose)
        {
            yield break;
        }

        state =
            BattleState.EnemyTurn;

        RestoreAllUnitVisuals();

        // =====================
        // SHOW ENEMY TURN
        // =====================

        yield return StartCoroutine(
            BattleUI.Instance
            .ShowTurnRoutine(
                "ENEMY TURN"));

        // =====================
        // ENEMY ACTION
        // =====================

        foreach (BattleUnit enemy
            in enemyUnits)
        {
            if (enemy == null)
                continue;

            yield return StartCoroutine(
                enemy.MoveEnemyTurnCoroutine());

            if (enemy != null)
            {
                enemy.SetTurnEndedVisual(true);
            }

            if (TurnOrderUI.Instance != null)
            {
                TurnOrderUI.Instance
                    .Refresh();
            }

            if (state == BattleState.Win
                || state == BattleState.Lose)
            {
                yield break;
            }
        }

        yield return new WaitForSeconds(1f);

        // =====================
        // NEXT WAVE CHECK
        // =====================

        enemyUnits.RemoveAll(
            unit => unit == null);

        if (enemyUnits.Count == 0)
        {
            StartCoroutine(
                NextWaveRoutine());

            yield break;
        }

        // =====================
        // PLAYER TURN
        // =====================

        yield return StartCoroutine(
            BattleUI.Instance
            .ShowTurnRoutine(
                "PLAYER TURN"));

        StartPlayerTurn();
    }

    // =========================
    // CHECK RESULT
    // =========================

    public void CheckBattleResult()
    {
        playerUnits.RemoveAll(
            unit => unit == null);

        enemyUnits.RemoveAll(
            unit => unit == null);

        Debug.Log(
            "Players: "
            + playerUnits.Count);

        Debug.Log(
            "Enemies: "
            + enemyUnits.Count);

        if (TurnOrderUI.Instance != null)
        {
            TurnOrderUI.Instance
                .Refresh();
        }

        // =====================
        // LOSE
        // =====================

        if (playerUnits.Count == 0)
        {
            isResolvingBattleResult = true;

            state =
                BattleState.Lose;

            BattleUI.Instance
                .ShowDefeatResult();

            return;
        }

        // =====================
        // NEXT WAVE / WIN
        // =====================

        if (enemyUnits.Count == 0)
        {
            isResolvingBattleResult = true;

            StartCoroutine(
                NextWaveRoutine());
        }
    }

    // =========================
    // NEXT WAVE
    // =========================

    private IEnumerator NextWaveRoutine()
    {
        BattleManager.Instance
            .SetBusy(true);

        yield return new WaitForSeconds(1f);

        bool hasNextWave =
            BattleManager.Instance
            .SpawnNextWave();

        if (hasNextWave)
        {
            isResolvingBattleResult = false;

            yield break;
        }

        // =====================
        // WIN
        // =====================

        state =
            BattleState.Win;

        StageData stage =
            BattleManager.Instance
            .currentStage;

        if (stage != null)
        {
            BattleUI.Instance
                .ShowVictoryResult(
                    stage.reward);
        }

        Debug.Log("VICTORY");
    }

    // =========================
    // GETTERS
    // =========================

    public List<BattleUnit>
        GetPlayerUnits()
    {
        return playerUnits;
    }

    public List<BattleUnit>
        GetEnemyUnits()
    {
        return enemyUnits;
    }

    private void RestoreAllUnitVisuals()
    {
        foreach (BattleUnit unit
            in playerUnits)
        {
            if (unit != null)
            {
                unit.SetTurnEndedVisual(false);
            }
        }

        foreach (BattleUnit unit
            in enemyUnits)
        {
            if (unit != null)
            {
                unit.SetTurnEndedVisual(false);
            }
        }
    }
}
