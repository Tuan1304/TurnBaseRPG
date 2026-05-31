using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Battle State")]
    public BattlePhase phase;

    private int currentWaveIndex;

    // private BattleUnit selectedUnit;

    [HideInInspector]
    public BattleUnit selectedUnit;

    private bool usingBasicAttack;

    [Header("Enemy Spawn")]
    [SerializeField]
    private BattleUnit enemyPrefab;

    [Header("Stage")]
    public StageData currentStage;

    [Header("Spawn Effect")]
    public GameObject enemySpawnEffect;

    private BattleTile previousTile;
    private Vector2Int previousGridPos;

    public bool IsBusy
    {
        get;
        private set;
    }

    private List<BattleTile> highlightedTiles =
        new List<BattleTile>();

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        Instance = this;

        if (BattleStageSelection.SelectedStage != null)
        {
            currentStage =
                BattleStageSelection.SelectedStage;
        }
    }

    private IEnumerator Start()
    {
        // đợi UI init xong
        yield return null;

        SpawnPlayerUnits();

        yield return StartCoroutine(
            SpawnStageEnemies());
    }

    public void SetBusy(bool value)
    {
        IsBusy = value;
    }

    // =========================
    // PLAYER SPAWN
    // =========================

    private void SpawnPlayerUnits()
    {
        Debug.Log("TEAM COUNT = " +
            BattleTeamManager.Instance.selectedHeroes.Count);
        // List<HeroData> heroes =
        //     UnitManager.Instance.activeHeroes;

        List<HeroData> heroes =
            BattleTeamManager.Instance.selectedHeroes;

        List<BattleTile> playerTiles =
            BattleGridManager.Instance.PlayerSpawnTiles;

        for (int i = 0;
            i < heroes.Count;
            i++)
        {
            if (i >= playerTiles.Count)
                break;

            BattleTile tile =
                playerTiles[i];

            BattleUnit prefab =
                BattleGridManager.Instance.playerPrefab;

            BattleUnit unit =
                Instantiate(
                    prefab,
                    tile.GetWorldPosition(),
                    Quaternion.identity);

            unit.teamType =
                TeamType.Player;

            unit.gridPos =
                tile.gridPos;

            unit.currentTile =
                tile;

            tile.occupiedUnit =
                unit;

            unit.Setup(
                heroes[i]);

            TurnManager.Instance
                .GetPlayerUnits()
                .Add(unit);
        }
    }

    // =========================
    // ENEMY SPAWN
    // =========================

    private IEnumerator SpawnStageEnemies()
    {
        if (currentStage == null)
        {
            Debug.LogError(
                "Missing current stage!");

            yield break;
        }

        if (currentStage.waves.Count == 0)
        {
            Debug.LogError(
                "Current stage has no waves!");

            yield break;
        }

        // HIỆN WAVE
        yield return StartCoroutine(
            BattleUI.Instance.ShowWaveRoutine(
                currentWaveIndex + 1));

        WaveData wave =
            currentStage.waves[currentWaveIndex];

        // SPAWN ENEMY
        foreach (StageEnemyData enemy in wave.enemies)
        {
            if (enemy.enemyData == null)
                continue;

            BattleTile tile =
                BattleGridManager.Instance
                .GetTileAtPosition(
                    enemy.gridPosition);

            if (tile == null)
                continue;

            if (enemySpawnEffect != null)
            {
                Instantiate(
                    enemySpawnEffect,
                    tile.GetWorldPosition(),
                    Quaternion.identity);
            }

            BattleUnit unit =
                Instantiate(
                    enemyPrefab,
                    tile.GetWorldPosition(),
                    Quaternion.identity);

            unit.teamType =
                TeamType.Enemy;

            unit.gridPos =
                enemy.gridPosition;

            unit.currentTile =
                tile;

            tile.occupiedUnit =
                unit;

            unit.SetupEnemy(
                enemy.enemyData);

            TurnManager.Instance
                .RegisterEnemyUnit(unit);

            yield return new WaitForSeconds(1f);
        }

        // ĐỢI SPAWN XONG
        yield return new WaitForSeconds(0.5f);

        // HIỆN PLAYER TURN
        yield return StartCoroutine(
            BattleUI.Instance.ShowTurnRoutine(
                "PLAYER TURN"));

        // BẮT ĐẦU PLAYER TURN
        TurnManager.Instance.StartPlayerTurn();
    }

    // =========================
    // SELECT UNIT
    // =========================

    public void SelectUnit(
        BattleUnit unit)
    {
        if (IsBusy)
            return;
        if (phase != BattlePhase.None)
            return;

        if (selectedUnit != null)
            return;

        if (unit.hasMoved)
            return;

        selectedUnit =
            unit;

        previousTile =
            unit.currentTile;

        previousGridPos =
            unit.gridPos;

        phase =
            BattlePhase.Moving;

        ShowMoveTiles(unit);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleRightClick()
    {
        BattleInfoPanel infoPanel =
            BattleInfoPanel.GetInstance();

        if (infoPanel != null &&
            infoPanel.IsVisible())
        {
            infoPanel.HideAndReturnToActionMenu();

            return;
        }

        if (phase == BattlePhase.Moving)
        {
            selectedUnit = null;

            phase = BattlePhase.None;

            ClearHighlights();

            return;
        }

        if (phase == BattlePhase.ActionSelection)
        {
            UndoMove();

            return;
        }

        if (phase == BattlePhase.SkillSelection)
        {
            phase =
                BattlePhase.ActionSelection;

            BattleUI.Instance
                .ShowSkillPanel(false);

            BattleUI.Instance
                .ShowActionMenu(true);

            return;
        }

        if (phase == BattlePhase.AttackSelection)
        {
            phase =
                BattlePhase.SkillSelection;

            ClearHighlights();

            BattleUI.Instance
                .ShowSkillPanel(true);

            return;
        }
    }

    private void UndoMove()
    {
        if (selectedUnit == null)
            return;

        // clear current
        if (selectedUnit.currentTile != null)
        {
            selectedUnit.currentTile
                .occupiedUnit = null;
        }

        // restore tile
        selectedUnit.currentTile =
            previousTile;

        selectedUnit.gridPos =
            previousGridPos;

        previousTile.occupiedUnit =
            selectedUnit;

        // restore world position
        selectedUnit.transform.position =
            previousTile.GetWorldPosition();

        selectedUnit.hasMoved = false;

        phase =
            BattlePhase.Moving;

        BattleUI.Instance
            .ShowActionMenu(false);

        ShowMoveTiles(selectedUnit);
    }

    // =========================
    // MOVE RANGE
    // =========================

    private void ShowMoveTiles(
        BattleUnit unit)
    {
        ClearHighlights();

        Vector2Int center =
            unit.gridPos;

        int moveRange =
            unit.moveRange;

        for (int x = -moveRange;
            x <= moveRange;
            x++)
        {
            for (int y = -moveRange;
                y <= moveRange;
                y++)
            {
                int distance =
                    Mathf.Abs(x)
                    + Mathf.Abs(y);

                if (distance > moveRange)
                    continue;

                Vector2Int pos =
                    center +
                    new Vector2Int(x, y);

                BattleTile tile =
                    BattleGridManager.Instance
                    .GetTile(pos);

                if (tile == null)
                    continue;

                if (tile.IsOccupied())
                    continue;

                List<BattleTile> path =
                    BattlePathfinder.Instance
                    .FindPath(
                        center,
                        pos);

                if (path == null)
                    continue;

                if (path.Count - 1 >
                    moveRange)
                    continue;

                // tile.Highlight(true);

                tile.HighlightMove(true);

                highlightedTiles
                    .Add(tile);
            }
        }
    }

    // =========================
    // TILE CLICK
    // =========================

    public void OnTileClicked(
        BattleTile tile)
    {
        if (IsBusy)
            return;
        if (phase != BattlePhase.Moving)
            return;

        if (selectedUnit == null)
            return;

        if (!highlightedTiles
            .Contains(tile))
        {
            return;
        }

        List<BattleTile> path =
            BattlePathfinder.Instance
            .FindPath(
                selectedUnit.gridPos,
                tile.gridPos);

        if (path == null)
            return;

        if (path.Count - 1 >
            selectedUnit.moveRange)
        {
            Debug.Log("Too far");

            return;
        }

        phase =
            BattlePhase.None;

        selectedUnit.MoveTo(
            tile,
            OnMoveComplete);

        ClearHighlights();
    }

    private bool CanOpenCombatMenu(BattleUnit unit)
    {
        if (unit == null)
            return false;

        // =====================
        // SELF / BUFF SKILL
        // =====================

        if (unit.skillData != null)
        {
            if (unit.skillData.targetType
                == TargetType.Self)
            {
                return true;
            }
        }

        // =====================
        // CHECK ENEMY
        // =====================

        List<BattleUnit> enemies =
            TurnManager.Instance
            .GetEnemyUnits();

        foreach (BattleUnit enemy in enemies)
        {
            if (enemy == null)
                continue;

            int distance =
                Mathf.Abs(
                    unit.gridPos.x -
                    enemy.gridPos.x)
                +
                Mathf.Abs(
                    unit.gridPos.y -
                    enemy.gridPos.y);

            // basic attack
            if (distance <= unit.attackRange)
            {
                return true;
            }

            // skill
            if (unit.skillData != null &&
                unit.skillData.targetType
                != TargetType.Self &&
                distance <= unit.skillData.range)
            {
                return true;
            }
        }

        return false;
    }

    // =========================
    // MOVE COMPLETE
    // =========================

    private void OnMoveComplete()
    {
        phase =
            BattlePhase.ActionSelection;

        bool canAttack = CanOpenCombatMenu(selectedUnit);

        BattleUI.Instance
            .ShowActionMenu(
                true,
                canAttack);
    }

    // =========================
    // ACTION MENU
    // =========================

    public void OnClickAttack()
    {
        if (selectedUnit == null)
            return;

        phase =
            BattlePhase.SkillSelection;

        BattleUI.Instance
            .ShowActionMenu(false);

        BattleUI.Instance
            .ShowSkillPanel(true);

        BattleUI.Instance
            .SetupSkillPanel(selectedUnit);
    }

    public void OnClickEndTurn()
    {
        EndSelectedUnitTurn();
    }

    public void OnClickInfo()
    {
        if (selectedUnit == null)
            return;

        BattleInfoPanel infoPanel =
            BattleInfoPanel.GetInstance();

        if (infoPanel == null)
        {
            Debug.LogWarning(
                "Missing BattleInfoPanel in scene.");

            return;
        }

        infoPanel
            .Show(selectedUnit);
    }

    public void ReturnToActionMenuFromInfo()
    {
        if (selectedUnit == null)
            return;

        phase =
            BattlePhase.ActionSelection;

        BattleUI.Instance
            .ShowSkillPanel(false);

        BattleUI.Instance
            .ShowActionMenu(
                true,
                CanOpenCombatMenu(selectedUnit));
    }

    // =========================
    // ENTER SKILL MODE
    // =========================

    public void EnterSkillMode(
        BattleUnit unit,
        bool basicAttack)
    {
        selectedUnit =
            unit;

        usingBasicAttack =
            basicAttack;

        // =====================
        // SELF TARGET SKILL
        // =====================

        if (!basicAttack &&
            unit.skillData != null &&
            unit.skillData.targetType ==
            TargetType.Self)
        {
            unit.UseSkill(unit);

            StartCoroutine(
                EndActionAfterDelay(1f));

            return;
        }

        // =====================
        // NORMAL TARGET
        // =====================

        phase =
            BattlePhase.AttackSelection;

        BattleUI.Instance
            .ShowSkillPanel(false);

        ShowAttackRange(unit);
    }

    // =========================
    // ATTACK RANGE
    // =========================

    private void ShowAttackRange(
        BattleUnit unit)
    {
        ClearHighlights();

        Vector2Int center =
            unit.gridPos;

        int range =
            usingBasicAttack
            ? unit.attackRange
            : unit.skillData.range;

        for (int x = -range;
            x <= range;
            x++)
        {
            for (int y = -range;
                y <= range;
                y++)
            {
                int distance =
                    Mathf.Abs(x)
                    + Mathf.Abs(y);

                if (distance > range)
                    continue;

                Vector2Int pos =
                    center +
                    new Vector2Int(x, y);

                BattleTile tile =
                    BattleGridManager.Instance
                    .GetTile(pos);

                if (tile == null)
                    continue;

                if (tile.occupiedUnit != null)
                {
                    BattleUnit target =
                        tile.occupiedUnit;

                    if (IsValidActionTarget(
                        unit,
                        target))
                    {
                        target.SetHighlight(true);
                    }
                }

                // tile.Highlight(true);

                tile.HighlightAttack(true);

                highlightedTiles
                    .Add(tile);

                // if (tile.occupiedUnit != null &&
                //     tile.occupiedUnit.teamType == TeamType.Enemy)
                // {
                //     tile.occupiedUnit.SetTargetHighlight(true);
                // }
            }
        }
    }

    // =========================
    // TRY ATTACK
    // =========================

    public void TryAttack(
        BattleUnit target)
    {
        if (selectedUnit == null)
            return;

        int distance =
            Mathf.Abs(
                selectedUnit.gridPos.x -
                target.gridPos.x)
            +
            Mathf.Abs(
                selectedUnit.gridPos.y -
                target.gridPos.y);

        int range =
            usingBasicAttack
            ? selectedUnit.attackRange
            : selectedUnit.skillData.range;

        if (distance > range)
        {
            Debug.Log("Out of range");

            return;
        }

        if (!IsValidActionTarget(
            selectedUnit,
            target))
        {
            Debug.Log("Invalid target");

            return;
        }

        // =====================
        // BASIC ATTACK
        // =====================

        if (usingBasicAttack)
        {
            selectedUnit.Attack(
                target,
                EndAction);

            return;
        }

        // =====================
        // SKILL
        // =====================

        else
        {
            selectedUnit.UseSkill(target);

            StartCoroutine(
                EndActionAfterDelay(1f));
        }
    }

    private IEnumerator EndActionAfterDelay(
        float delay)
    {
        yield return new WaitForSeconds(delay);

        EndAction();
    }

    private bool IsValidActionTarget(
        BattleUnit caster,
        BattleUnit target)
    {
        if (caster == null ||
            target == null)
        {
            return false;
        }

        if (usingBasicAttack)
        {
            return target.teamType !=
                caster.teamType;
        }

        if (caster.skillData == null)
            return false;

        switch (caster.skillData.targetType)
        {
            case TargetType.EnemySingle:
            case TargetType.EnemyAOE:
            case TargetType.AllEnemies:
                return target.teamType !=
                    caster.teamType;

            case TargetType.AllySingle:
            case TargetType.AllyAOE:
            case TargetType.AllAllies:
            case TargetType.Self:
                return target.teamType ==
                    caster.teamType;
        }

        return false;
    }

    // =========================
    // END ACTION
    // =========================

    private void EndAction()
    {
        if (selectedUnit == null)
            return;

        selectedUnit.hasAttacked =
            true;

        selectedUnit.state =
            BattleUnitState.Ended;

        selectedUnit.SetTurnEndedVisual(true);

        phase =
            BattlePhase.None;

        ClearHighlights();

        BattleUI.Instance
            .ShowActionMenu(false);

        BattleUI.Instance
            .ShowSkillPanel(false);

        TurnManager.Instance
            .CheckPlayerTurnEnd();

        selectedUnit = null;
    }

    // =========================
    // END TURN
    // =========================

    public void EndSelectedUnitTurn()
    {
        if (selectedUnit == null)
            return;

        selectedUnit.hasMoved =
            true;

        selectedUnit.hasAttacked =
            true;

        selectedUnit.state =
            BattleUnitState.Ended;

        selectedUnit.SetTurnEndedVisual(true);

        phase =
            BattlePhase.None;

        ClearHighlights();

        BattleUI.Instance
            .ShowActionMenu(false);

        BattleUI.Instance
            .ShowSkillPanel(false);

        TurnManager.Instance
            .CheckPlayerTurnEnd();

        selectedUnit = null;
    }

    // =========================
    // CLEAR HIGHLIGHT
    // =========================

    private void ClearHighlights()
    {
        foreach (BattleUnit unit in TurnManager.Instance.GetEnemyUnits())
        {
            if (unit != null)
            {
                unit.SetHighlight(false);
            }
        }

        foreach (BattleUnit unit in TurnManager.Instance.GetPlayerUnits())
        {
            if (unit != null)
            {
                unit.SetHighlight(false);
            }
        }

        foreach (BattleTile tile in highlightedTiles)
        {
            if (tile != null)
            {
                tile.HighlightMove(false);
                tile.HighlightAttack(false);
            }
        }

        highlightedTiles.Clear();
    }

   public void NextWave()
    {
        phase = BattlePhase.None;

        currentWaveIndex++;

        if (currentWaveIndex >=
            currentStage.waves.Count)
        {
            Debug.Log("STAGE CLEAR!");

            return;
        }

        Debug.Log(
            "Next Wave: " +
            (currentWaveIndex + 1));

        StartCoroutine(SpawnStageEnemies());
    }

    public bool SpawnNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex
            < currentStage.waves.Count)
        {
            StartCoroutine(
                SpawnStageEnemies());

            return true;
        }

        return false;
    }
}
