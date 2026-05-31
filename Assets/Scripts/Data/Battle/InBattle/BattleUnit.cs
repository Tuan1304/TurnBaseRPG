using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public HeroData heroData;

    public CharacterVisual visual;

    public BattleTile currentTile;

    public Vector2Int gridPos;

    public TeamType teamType;

    public bool isMouseOver;

    private Vector3 originalVisualScale;
    // =========================
    // HIGHLIGHT
    // =========================

    private bool isHighlighted;
    [Header("Target")]
    public GameObject targetIndicator;

    // =========================
    // VISUAL
    // =========================

    [Header("Visual")]
    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private Animator[] visualAnimators;
    private float[] originalAnimatorSpeeds;
    private bool isTurnEndedVisual;
    private Collider2D rootClickCollider;

    public GameObject hitEffectPrefab;

    [Header("Visual Root")]
    [SerializeField]
    private Transform visualRoot;

    // =========================
    // ANIMATOR
    // =========================

    [Header("Animator")]
    public RuntimeAnimatorController warriorAnimator;

    public RuntimeAnimatorController archerAnimator;

    public RuntimeAnimatorController mageAnimator;

    // =========================
    // ENEMY
    // =========================

    [Header("Enemy")]
    public EnemyData enemyData;

    // =========================
    // STATUS EFFECT
    // =========================

    [Header("Status Effects")]
    public List<StatusEffect> activeEffects =
        new List<StatusEffect>();

    [Header("Modifiers")]
    public List<StatModifier> modifiers =
        new List<StatModifier>();

    // =========================
    // PROJECTILE
    // =========================

    [Header("Projectile")]
    public Transform projectileSpawnPoint;

    // =========================
    // MOVEMENT
    // =========================

    [Header("Movement")]
    public float moveSpeed = 3f;

    public bool IsMoving
    {
        get;
        private set;
    }

    // =========================
    // STATE
    // =========================

    [Header("State")]
    public BattleUnitState state =
        BattleUnitState.Waiting;

    public bool hasMoved;

    public bool hasAttacked;

    private bool isDead;

    // =========================
    // STATS
    // =========================

    [Header("Stats")]
    public HeroStats baseStats;

    public HeroStats runtimeStats;

    public int currentHP;

    // =========================
    // COMBAT
    // =========================

    [Header("Combat")]
    public int attackRange = 1;

    [Header("Battle")]
    public int moveRange = 5;

    // =========================
    // SKILL
    // =========================

    [Header("Skill")]
    public SkillData skillData;

    private SkillData pendingSkill;

    private List<BattleUnit> pendingSkillTargets;

    public int currentCooldown;

    // =========================
    // UI
    // =========================

    [Header("UI")]
    public BattleHPBar hpBarPrefab;

    public DamagePopup damagePopupPrefab;

    private BattleHPBar hpBar;

    // =========================
    // COMBAT CACHE
    // =========================

    private BattleUnit pendingTarget;

    private int pendingDamage;

    private bool pendingCrit;

    // =========================
    // START
    // =========================

    private void Start()
    {
        originalVisualScale = visualRoot.localScale;

        Canvas canvas =
            FindFirstObjectByType<Canvas>();

        hpBar =
            Instantiate(
                hpBarPrefab,
                canvas.transform);

        hpBar.Setup(this);

        CacheVisualState();

        SetupRootClickCollider();
    }

    private void CacheVisualState()
    {
        renderers =
            GetComponentsInChildren<SpriteRenderer>();

        originalColors =
            new Color[renderers.Length];

        for (int i = 0;
            i < renderers.Length;
            i++)
        {
            originalColors[i] =
                renderers[i].color;
        }

        visualAnimators =
            GetComponentsInChildren<Animator>();

        originalAnimatorSpeeds =
            new float[visualAnimators.Length];

        for (int i = 0;
            i < visualAnimators.Length;
            i++)
        {
            originalAnimatorSpeeds[i] =
                visualAnimators[i].speed;
        }
    }

    private void SetupRootClickCollider()
    {
        rootClickCollider =
            GetComponent<Collider2D>();

        if (rootClickCollider == null)
        {
            rootClickCollider =
                gameObject.AddComponent<BoxCollider2D>();
        }

        Collider2D[] childColliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D childCollider
            in childColliders)
        {
            if (childCollider != null &&
                childCollider != rootClickCollider)
            {
                childCollider.enabled = false;
            }
        }

        BoxCollider2D boxCollider =
            rootClickCollider as BoxCollider2D;

        if (boxCollider == null ||
            renderers == null ||
            renderers.Length == 0)
        {
            return;
        }

        Bounds bounds =
            renderers[0].bounds;

        for (int i = 1;
            i < renderers.Length;
            i++)
        {
            if (renderers[i] != null)
            {
                bounds.Encapsulate(
                    renderers[i].bounds);
            }
        }

        Vector3 localCenter =
            transform.InverseTransformPoint(
                bounds.center);

        boxCollider.offset =
            new Vector2(
                localCenter.x,
                localCenter.y);

        boxCollider.size =
            new Vector2(
                Mathf.Max(0.6f, bounds.size.x),
                Mathf.Max(0.8f, bounds.size.y));
    }

    private void TryOpenInfo()
    {
        if (!isMouseOver)
            return;

        BattleInfoPanel.Instance
            .Show(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryOpenInfo();
        }
    }

    public void OnMouseEnter()
    {
        isMouseOver = true;
    }

    public void OnMouseExit()
    {
        isMouseOver = false;
    }

    // =========================
    // SETUP PLAYER
    // =========================

    public void Setup(HeroData hero)
    {
        heroData = hero;

        visual.ApplyHero(hero);

        baseStats =
            HeroStatsUtility.Clone(
                EquipmentUtility.GetTotalStats(
                    hero,
                    InventoryManager.Instance));

        runtimeStats =
            HeroStatsUtility.Clone(
                baseStats);

        currentHP =
            runtimeStats.hp;

        Debug.Log(
            "Battle setup hero " +
            hero.unitData.unitName +
            " | base hp: " +
            hero.stats.hp +
            " | saved current hp: " +
            hero.currentHP +
            " | runtime hp: " +
            runtimeStats.hp +
            " | weapon: " +
            hero.equipment.weaponInstanceID +
            " | armor: " +
            hero.equipment.armorInstanceID +
            " | helmet: " +
            hero.equipment.helmetInstanceID +
            " | pants: " +
            hero.equipment.pantsInstanceID);

        switch (hero.unitData.unitClass)
        {
            case UnitClass.Warrior:

                attackRange = 1;

                break;

            case UnitClass.Archer:

                attackRange = 4;

                break;

            case UnitClass.Mage:

                attackRange = 3;

                break;
        }

        skillData =
            hero.unitData.skill;

        SetupAnimator(
            hero.unitData.unitClass);
    }

    // =========================
    // SETUP ENEMY
    // =========================

    public void SetupEnemy(EnemyData data)
    {
        enemyData = data;

        visual.ApplyAppearance(
            data.appearance);

        baseStats =
            HeroStatsUtility.Clone(
                data.stats);

        runtimeStats =
            HeroStatsUtility.Clone(
                data.stats);

        currentHP =
            runtimeStats.hp;

        skillData = data.skill;

        switch (data.unitClass)
        {
            case UnitClass.Warrior:

                attackRange = 1;

                break;

            case UnitClass.Archer:

                attackRange = 4;

                break;

            case UnitClass.Mage:

                attackRange = 3;

                break;
        }

        SetupAnimator(
            data.unitClass);
    }

    // =========================
    // SETUP ANIMATOR
    // =========================

    private void SetupAnimator(
        UnitClass unitClass)
    {
        Animator anim =
            GetComponentInChildren<Animator>();

        if (anim == null)
        {
            Debug.LogError(
                "Missing Animator!");

            return;
        }

        switch (unitClass)
        {
            case UnitClass.Warrior:

                anim.runtimeAnimatorController =
                    warriorAnimator;

                break;

            case UnitClass.Archer:

                anim.runtimeAnimatorController =
                    archerAnimator;

                break;

            case UnitClass.Mage:

                anim.runtimeAnimatorController =
                    mageAnimator;

                break;
        }
    }

    // =========================
    // CLICK
    // =========================

    private void OnMouseDown()
    {
        if (isDead)
            return;

        if (BattleManager.Instance.IsBusy)
            return;

        if (TurnManager.Instance.state
            != BattleState.PlayerTurn)
        {
            return;
        }

        if (HasControlBlockEffect())
        {
            return;
        }

        if (BattleManager.Instance.phase
            == BattlePhase.AttackSelection)
        {
            BattleManager.Instance
                .TryAttack(this);

            return;
        }

        // =====================
        // PLAYER CLICK
        // =====================

        if (state ==
            BattleUnitState.Ended)
        {
            return;
        }

        if (hasMoved)
            return;

        BattleManager.Instance
            .SelectUnit(this);
    }

    // =========================
    // MOVE
    // =========================

    public void MoveTo(BattleTile tile, System.Action onComplete)
    {
        BattleTile oldTile = currentTile;

        List<BattleTile> path =
            BattlePathfinder.Instance
            .FindPath(
                gridPos,
                tile.gridPos);

        StartCoroutine(
            MovePathRoutine(
                path,
                () =>
                {
                    if (oldTile != null)
                    {
                        oldTile.occupiedUnit =
                            null;
                    }

                    currentTile = tile;

                    currentTile.occupiedUnit =
                        this;

                    onComplete?.Invoke();
                }));
    }

    public IEnumerator MovePathRoutine(
        List<BattleTile> path,
        System.Action onComplete)
    {
        if (path == null ||
            path.Count == 0)
        {
            yield break;
        }

        IsMoving = true;
        

        // =====================
        // CAMERA FOLLOW
        // =====================

        if (BattleCamera.Instance != null)
        {
            BattleCamera.Instance
                .StartFollow(transform);

            BattleCamera.Instance
                .SetLocked(true);
        }

        foreach (BattleTile tile in path)
        {
            Vector3 targetPos =
                tile.GetWorldPosition();

            Vector3 dir =
                targetPos - transform.position;

            if (visualRoot != null)
            {
                Vector3 scale = originalVisualScale;

                if (dir.x > 0.05f)
                {
                    scale.x =
                        -Mathf.Abs(
                            originalVisualScale.x);
                }
                else if (dir.x < -0.05f)
                {
                    scale.x =
                        Mathf.Abs(
                            originalVisualScale.x);
                }

                visualRoot.rotation =
                    Quaternion.identity;

                visualRoot.localScale = scale;
            }

            Debug.Log("DIR X = " + dir.x);

            while (Vector3.Distance(
                transform.position,
                targetPos) > 0.05f)
            {
                transform.position =
                    Vector3.MoveTowards(
                        transform.position,
                        targetPos,
                        moveSpeed *
                        Time.deltaTime);

                yield return null;
            }

            transform.position =
                targetPos;
        }

        gridPos =
            path[path.Count - 1]
            .gridPos;

        IsMoving = false;

        hasMoved = true;

        // =====================
        // RELEASE CAMERA
        // =====================

        if (BattleCamera.Instance != null)
        {
            BattleCamera.Instance
                .StopFollow();

            BattleCamera.Instance
                .SetLocked(false);
        }

        onComplete?.Invoke();
    }

    // =========================
    // COMBAT ROLL
    // =========================

    private bool RollCrit()
    {
        if (heroData == null)
            return false;

        int roll =
            Random.Range(0, 100);

        return roll <
            heroData.stats.critChance;
    }

    private bool RollEvade(BattleUnit attacker)
    {
        float evadeChance = 0;

        // PLAYER

        if (heroData != null)
        {
            evadeChance =
                heroData.stats.evade;
        }

        // ENEMY

        else if (enemyData != null)
        {
            evadeChance = 0;
        }

        int roll =
            Random.Range(0, 100);

        return roll < evadeChance;
    }

    // =========================
    // ATTACK
    // =========================

    public void Attack(
        BattleUnit target,
        System.Action onComplete)
    {
        StartCoroutine(
            AttackRoutine(
                target,
                onComplete));
    }

    private IEnumerator AttackRoutine(
        BattleUnit target,
        System.Action onComplete)
    {
        // int damage =
        //     Mathf.Max(
        //         1,
        //         stats.attack - target.stats.defense);

        // bool isCrit =
        //     RollCrit();

        // if (isCrit)
        // {
        //     damage *= 2;
        // }

        bool isCrit;

        int damage =
            BattleFormula
            .CalculateAttackDamage(
                this,
                target,
                out isCrit);

        pendingTarget =
            target;

        pendingDamage =
            damage;

        pendingCrit =
            isCrit;

        // =====================
        // LOCK CAMERA
        // =====================

        if (BattleCamera.Instance != null)
        {
            BattleCamera.Instance
                .SetLocked(true);

            yield return StartCoroutine(
                BattleCamera.Instance
                .FocusTarget(transform));

            yield return StartCoroutine(
                BattleCamera.Instance
                .ZoomIn());
        }

        // =====================
        // PLAY ATTACK
        // =====================

        UnitAnimator anim =
            GetComponentInChildren<UnitAnimator>();

        if (anim != null)
        {
            anim.PlayAttack();
        }
        else
        {
            DealAttackDamage();
        }

        // =====================
        // CHỜ PROJECTILE / HIT
        // =====================

        yield return new WaitForSeconds(1.2f);

        // =====================
        // RESET CAMERA
        // =====================

        if (BattleCamera.Instance != null)
        {
            yield return StartCoroutine(
                BattleCamera.Instance
                .ResetCamera());

            BattleCamera.Instance
                .SetLocked(false);
        }

        // =====================
        // DELAY NHẸ
        // =====================

        yield return new WaitForSeconds(0.3f);

        // =====================
        // COMPLETE
        // =====================

        onComplete?.Invoke();
    }

    // =========================
    // DEAL ATTACK DAMAGE
    // =========================

    public void DealAttackDamage()
    {
        if (pendingTarget == null)
            return;

        // EVADE

        if (BattleFormula.RollEvade(pendingTarget))
        {
            Debug.Log("MISS!");
            return;
        }

        UnitClass unitClass =
            GetUnitClass();

        switch (unitClass)
        {
            case UnitClass.Archer:
            case UnitClass.Mage:

                ShootProjectile(
                    pendingTarget,
                    pendingDamage);

                return;
        }

        pendingTarget.TakeDamage(
            pendingDamage);
    }

    private UnitClass GetUnitClass()
    {
        if (heroData != null &&
            heroData.unitData != null)
        {
            return heroData.unitData.unitClass;
        }

        if (enemyData != null)
        {
            return enemyData.unitClass;
        }

        return UnitClass.Warrior;
    }

    // =========================
    // USE SKILL
    // =========================

    public void UseSkill(
        BattleUnit target)
    {
        if (isDead)
            return;

        if (skillData == null)
            return;

        pendingSkill =
            skillData;

        pendingSkillTargets =
            SkillResolver.GetTargets(
                skillData,
                this,
                target);

        currentCooldown =
            skillData.cooldown;

        UnitAnimator anim =
            GetComponentInChildren<UnitAnimator>();

        if (skillData.castEffectPrefab
            != null)
        {
            Instantiate(
                skillData.castEffectPrefab,
                transform.position,
                Quaternion.identity);
        }

        if (anim != null)
        {
            anim.PlaySkill();
        }
        else
        {
            DealSkillEffect();
        }
    }

    // =========================
    // DEAL SKILL EFFECT
    // =========================

    public void DealSkillEffect()
    {
        if (pendingSkill == null)
            return;

        foreach (BattleUnit t
            in pendingSkillTargets)
        {
            if (t == null)
                continue;

            ApplySkillEffect(
                pendingSkill,
                t);
        }
    }

    public bool HasControlBlockEffect()
    {
        foreach (StatusEffect effect
            in activeEffects)
        {
            if (effect.preventAction)
            {
                return true;
            }
        }

        return false;
    }

    // =========================
    // APPLY SKILL
    // =========================

    private void ApplySkillEffect(
        SkillData skill,
        BattleUnit target)
    {
        if (skill.hitEffectPrefab
            != null)
        {
            Instantiate(
                skill.hitEffectPrefab,
                target.transform.position,
                Quaternion.identity);
        }

        int finalDamage =
            BattleFormula
            .CalculateSkillDamage(
                this,
                target,
                skill);

        if (finalDamage > 0)
        {
            target.TakeDamage(finalDamage);
        }

        // HEAL

        if (skill.healAmount > 0)
        {
            target.currentHP =
                Mathf.Min(
                    target.runtimeStats.hp,
                    target.currentHP +
                    skill.healAmount);

            if (target.hpBar != null)
            {
                target.hpBar.Refresh();
            }
        }

        // BUFF

        if (skill.buffAttack > 0)
        {
            StatusEffect effect =
                new StatusEffect();

            effect.attackBuff =
                skill.buffAttack;

            effect.duration =
                skill.buffDuration;

            target.AddStatusEffect(effect);
        }

        if (skill.applyPoison)
        {
            StatusEffect effect =
                new StatusEffect();

            effect.effectType =
                EffectType.Poison;

            effect.tickDamage =
                skill.poisonDamage;

            effect.duration =
                skill.poisonDuration;

            target.AddStatusEffect(effect);
        }

        if (skill.applyStun)
        {
            StatusEffect effect =
                new StatusEffect();

            effect.effectType =
                EffectType.Stun;

            effect.preventAction =
                true;

            effect.duration =
                skill.stunDuration;

            target.AddStatusEffect(effect);
        }
    }

    public void RecalculateStats()
    {
        runtimeStats =
            HeroStatsUtility.Clone(
                baseStats);

        foreach (StatModifier mod
            in modifiers)
        {
            ApplyModifier(mod);
        }
    }

    private void ApplyModifier(
        StatModifier mod)
    {
        switch (mod.statType)
        {
            // =====================
            // ATTACK
            // =====================

            case StatType.Attack:

                if (mod.modifierType ==
                    ModifierType.Flat)
                {
                    runtimeStats.attack +=
                        mod.value;
                }
                else
                {
                    runtimeStats.attack +=
                        Mathf.RoundToInt(
                            baseStats.attack *
                            (mod.value / 100f));
                }

                break;

            // =====================
            // DEFENSE
            // =====================

            case StatType.Defense:

                if (mod.modifierType ==
                    ModifierType.Flat)
                {
                    runtimeStats.defense +=
                        mod.value;
                }
                else
                {
                    runtimeStats.defense +=
                        Mathf.RoundToInt(
                            baseStats.defense *
                            (mod.value / 100f));
                }

                break;

            // =====================
            // ABILITY POWER
            // =====================

            case StatType.AbilityPower:

                if (mod.modifierType ==
                    ModifierType.Flat)
                {
                    runtimeStats.abilityPower +=
                        mod.value;
                }
                else
                {
                    runtimeStats.abilityPower +=
                        Mathf.RoundToInt(
                            baseStats.abilityPower *
                            (mod.value / 100f));
                }

                break;
        }
}

    // =========================
    // STATUS EFFECT
    // =========================

    public void AddStatusEffect(
        StatusEffect effect)
    {
        activeEffects.Add(effect);

        if (effect.attackBuff != 0)
        {
            StatModifier mod =
                new StatModifier();

            mod.statType =
                StatType.Attack;

            mod.modifierType =
                ModifierType.Flat;

            mod.value =
                effect.attackBuff;

            mod.duration =
                effect.duration;

            modifiers.Add(mod);
        }

        if (effect.defenseBuff != 0)
        {
            StatModifier mod =
                new StatModifier();

            mod.statType =
                StatType.Defense;

            mod.modifierType =
                ModifierType.Flat;

            mod.value =
                effect.defenseBuff;

            mod.duration =
                effect.duration;

            modifiers.Add(mod);
        }

        RecalculateStats();
    }

    public void UpdateStatusEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i]
                .duration--;

            ApplyEffectTick(activeEffects[i]);

            if (activeEffects[i]
                .duration <= 0)
            {
                RemoveEffect(
                    activeEffects[i]);

                activeEffects
                    .RemoveAt(i);
            }
        }

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            modifiers[i].duration--;

            if (modifiers[i].duration <= 0)
            {
                modifiers.RemoveAt(i);
            }
        }

        RecalculateStats();
    }

    private void ApplyEffectTick(StatusEffect effect)
    {
        if (effect.tickDamage > 0)
        {
            TakeDamage(
                effect.tickDamage);
        }
    }

    private void RemoveEffect(StatusEffect effect)
    {
        modifiers.RemoveAll(
            mod => mod.duration <= 0);

        RecalculateStats();
    }

    // =========================
    // TAKE DAMAGE
    // =========================

    public void TakeDamage(
        int damage)
    {
        if (isDead)
            return;

        currentHP -= damage;

        UnitAnimator anim =
            GetComponentInChildren<UnitAnimator>();

        if (anim != null &&
            currentHP > 0)
        {
            anim.PlayHurt();
        }

        Canvas canvas =
            FindFirstObjectByType<Canvas>();

        DamagePopup popup =
            Instantiate(
                damagePopupPrefab,
                canvas.transform);

        popup.transform.position =
            Camera.main.WorldToScreenPoint(
                transform.position +
                Vector3.up * 1.5f);

        popup.Setup(damage);

        if (hitEffectPrefab != null)
        {
            Instantiate(
                hitEffectPrefab,
                transform.position,
                Quaternion.identity);
        }

        if (hpBar != null)
        {
            hpBar.Refresh();
        }

        StartCoroutine(
            FlashRoutine());

        if (BattleCamera.Instance != null)
        {
            StartCoroutine(
                BattleCamera.Instance
                .Shake());
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // =========================
    // DIE
    // =========================

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        state =
            BattleUnitState.Ended;

        if (currentTile != null)
        {
            currentTile.occupiedUnit = null;
        }

        if (teamType == TeamType.Player)
        {
            TurnManager.Instance
                .GetPlayerUnits()
                .Remove(this);
        }
        else
        {
            TurnManager.Instance
                .GetEnemyUnits()
                .Remove(this);
        }

        TurnManager.Instance
            .CheckBattleResult();

        UnitAnimator anim =
            GetComponentInChildren<UnitAnimator>();

        if (anim != null)
        {
            anim.PlayDie();
        }
        else
        {
            DestroyUnit();
        }
    }

    public void DestroyUnit()
    {
        if (hpBar != null)
        {
            Destroy(
                hpBar.gameObject);
        }

        Destroy(gameObject);
    }

    // =========================
    // ENEMY AI
    // =========================

    public IEnumerator MoveEnemyTurnCoroutine()
    {
        yield return StartCoroutine(
            EnemyRoutine());
    }

    private IEnumerator EnemyRoutine()
    {
        if (HasControlBlockEffect())
        {
            yield return new WaitForSeconds(0.5f);

            yield break;
        }

        BattleUnit target =
            FindClosestPlayer();

        if (target == null)
            yield break;

        bool isRanged =
            enemyData != null &&
            (
                enemyData.unitClass ==
                UnitClass.Archer ||

                enemyData.unitClass ==
                UnitClass.Mage
            );

        int distance =
            Mathf.Abs(
                gridPos.x -
                target.gridPos.x)
            +
            Mathf.Abs(
                gridPos.y -
                target.gridPos.y);

        // =====================
        // BACK AWAY
        // =====================

        // bool backedAway = false;

        if (isRanged &&
            distance <= 1)
        {
            Vector2Int dir =
                gridPos -
                target.gridPos;

            dir.x =
                Mathf.Clamp(
                    dir.x,
                    -1,
                    1);

            dir.y =
                Mathf.Clamp(
                    dir.y,
                    -1,
                    1);

            Vector2Int backPos =
                gridPos + dir;

            BattleTile backTile =
                BattleGridManager.Instance
                .GetTile(backPos);

            if (backTile != null &&
                !backTile.IsOccupied())
            {
                List<BattleTile> backPath =
                    new List<BattleTile>();

                backPath.Add(currentTile);

                backPath.Add(backTile);

                currentTile.occupiedUnit =
                    null;

                currentTile =
                    backTile;

                currentTile.occupiedUnit =
                    this;

                yield return StartCoroutine(
                    MovePathRoutine(
                        backPath,
                        null));

                gridPos =
                    backTile.gridPos;

                // backedAway = true;
            }
        }

        // =====================
        // NORMAL MOVE
        // =====================

        distance =
            Mathf.Abs(
                gridPos.x -
                target.gridPos.x)
            +
            Mathf.Abs(
                gridPos.y -
                target.gridPos.y);

        bool shouldMove =
            distance > attackRange;

        if (shouldMove)
        {
            BattleTile attackTile =
                FindClosestAttackTile(target);

            if (attackTile == null)
                yield break;

            List<BattleTile> path =
                BattlePathfinder.Instance
                .FindPath(
                    gridPos,
                    attackTile.gridPos);

            if (path != null &&
                path.Count > 1)
            {
                int moveSteps =
                    Mathf.Min(
                        moveRange,
                        path.Count - 1);

                List<BattleTile> movePath =
                    new List<BattleTile>();

                movePath.Add(path[0]);

                for (int i = 1;
                    i <= moveSteps;
                    i++)
                {
                    movePath.Add(path[i]);
                }

                BattleTile destination =
                    movePath[
                        movePath.Count - 1];

                BattleTile oldTile =currentTile;

                yield return StartCoroutine(
                    MovePathRoutine(
                        movePath,
                        null));

                if (oldTile != null)
                {
                    oldTile.occupiedUnit = null;
                }

                currentTile =
                    destination;

                currentTile.occupiedUnit =
                    this;

                gridPos =
                    destination.gridPos;
            }
        }

        yield return new WaitForSeconds(0.5f);

        // =====================
        // RECALCULATE DISTANCE
        // =====================

        distance =
            Mathf.Abs(
                gridPos.x -
                target.gridPos.x)
            +
            Mathf.Abs(
                gridPos.y -
                target.gridPos.y);

        // =====================
        // MAGE AI
        // =====================

        if (
            enemyData != null &&
            enemyData.unitClass ==
            UnitClass.Mage)
        {
            if (
                CanUseSkill(target) &&
                CanHitMultipleTargets(target))
            {
                UseSkill(target);

                yield return new WaitForSeconds(1f);
            }
            else if (
                distance <= attackRange)
            {
                bool finished = false;

                Attack(
                    target,
                    () => finished = true);

                yield return new WaitUntil(
                    () => finished);
            }

            yield break;
        }

        // =====================
        // NORMAL AI
        // =====================

        if (CanUseSkill(target))
        {
            UseSkill(target);

            yield return new WaitForSeconds(1f);
        }
        else if (
            distance <= attackRange)
        {
            bool finished = false;

            Attack(
                target,
                () => finished = true);

            yield return new WaitUntil(
                () => finished);
        }
    }

    // =========================
    // FIND TARGET
    // =========================

    private BattleUnit FindClosestPlayer()
    {
        List<BattleUnit> players =
            TurnManager.Instance
            .GetPlayerUnits();

        BattleUnit bestTarget = null;

        int bestScore = -999999;

        foreach (BattleUnit unit in players)
        {
            if (unit == null)
                continue;

            int distance =
                Mathf.Abs(
                    gridPos.x -
                    unit.gridPos.x)
                +
                Mathf.Abs(
                    gridPos.y -
                    unit.gridPos.y);

            int score = 0;

            // =====================
            // WARRIOR
            // ƯU TIÊN GẦN
            // =====================

            if (enemyData != null &&
                enemyData.unitClass ==
                UnitClass.Warrior)
            {
                score -= distance * 20;

                score -= unit.currentHP;
            }

            // =====================
            // ARCHER
            // ƯU TIÊN MÁU THẤP
            // =====================

            else if (enemyData != null &&
                    enemyData.unitClass ==
                    UnitClass.Archer)
            {
                score -= unit.currentHP * 5;

                score -= distance * 3;
            }

            // =====================
            // MAGE
            // ƯU TIÊN HIT NHIỀU
            // =====================

            else if (enemyData != null &&
                    enemyData.unitClass ==
                    UnitClass.Mage)
            {
                int nearbyCount =
                    CountNearbyPlayers(unit);

                score += nearbyCount * 100;

                score -= distance * 2;
            }

            // =====================
            // DEFAULT
            // =====================

            else
            {
                score -= distance * 10;
            }

            // =====================
            // PICK BEST
            // =====================

            if (score > bestScore)
            {
                bestScore = score;

                bestTarget = unit;
            }
        }

        return bestTarget;
    }

    private BattleTile FindClosestAttackTile(BattleUnit target)
    {
        List<BattleTile> possibleTiles =
            new List<BattleTile>();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int pos =
                target.gridPos + dir;

            BattleTile tile =
                BattleGridManager.Instance
                .GetTile(pos);

            if (tile == null)
                continue;

            if (tile.IsOccupied())
                continue;

            possibleTiles.Add(tile);
        }

        BattleTile bestTile = null;

        int bestDistance = 9999;

        foreach (BattleTile tile in possibleTiles)
        {
            int distance =
                Mathf.Abs(
                    gridPos.x -
                    tile.gridPos.x)
                +
                Mathf.Abs(
                    gridPos.y -
                    tile.gridPos.y);

            if (distance < bestDistance)
            {
                bestDistance = distance;

                bestTile = tile;
            }
        }

        return bestTile;
    }

    private int CountNearbyPlayers(
        BattleUnit centerTarget)
    {
        int count = 0;

        List<BattleUnit> players =
            TurnManager.Instance
            .GetPlayerUnits();

        foreach (BattleUnit unit in players)
        {
            if (unit == null)
                continue;

            int distance =
                Mathf.Abs(
                    centerTarget.gridPos.x -
                    unit.gridPos.x)
                +
                Mathf.Abs(
                    centerTarget.gridPos.y -
                    unit.gridPos.y);

            // phạm vi AoE nhỏ
            if (distance <= 1)
            {
                count++;
            }
        }

        return count;
    }

    private bool CanHitMultipleTargets(
        BattleUnit centerTarget)
    {
        int count =
            CountNearbyPlayers(
                centerTarget);

        return count >= 2;
    }

    // =========================
    // PROJECTILE
    // =========================

    private void ShootProjectile(
        BattleUnit target,
        int damage)
    {
        Projectile projectilePrefab =
            null;

        // PLAYER

        if (heroData != null &&
            heroData.unitData != null)
        {
            projectilePrefab =
                heroData.unitData
                .attackProjectile;
        }

        // ENEMY

        else if (enemyData != null)
        {
            projectilePrefab =
                enemyData
                .attackProjectile;
        }

        // NO PROJECTILE

        if (projectilePrefab == null)
        {
            target.TakeDamage(damage);

            return;
        }

        Projectile projectile =
            Instantiate(
                projectilePrefab,
                projectileSpawnPoint.position,
                Quaternion.identity);

        projectile.Setup(
            target,
            damage);
    }

    // =========================
    // FLASH
    // =========================

    private IEnumerator FlashRoutine()
    {
        if (isDead)
            yield break;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                if (isHighlighted)
                {
                    renderers[i].color =
                        new Color(1f, 0.6f, 0.6f);
                }
                else if (isTurnEndedVisual)
                {
                    renderers[i].color =
                        GetEndedTurnColor(
                            originalColors[i]);
                }
                else
                {
                    renderers[i].color =
                        originalColors[i];
                }
            }
        }
}

    // =========================
    // TARGET HIGHLIGHT
    // =========================

    public void SetTurnEndedVisual(
        bool value)
    {
        if (isDead)
            return;

        if (renderers == null ||
            originalColors == null ||
            renderers.Length != originalColors.Length)
        {
            CacheVisualState();
        }

        isTurnEndedVisual = value;

        for (int i = 0;
            i < renderers.Length;
            i++)
        {
            if (renderers[i] == null)
                continue;

            renderers[i].color =
                value
                ? GetEndedTurnColor(
                    originalColors[i])
                : originalColors[i];
        }

        if (visualAnimators == null ||
            originalAnimatorSpeeds == null ||
            visualAnimators.Length !=
            originalAnimatorSpeeds.Length)
        {
            CacheVisualState();
        }

        for (int i = 0;
            i < visualAnimators.Length;
            i++)
        {
            if (visualAnimators[i] == null)
                continue;

            visualAnimators[i].speed =
                value
                ? 0f
                : originalAnimatorSpeeds[i];
        }
    }

    private Color GetEndedTurnColor(
        Color source)
    {
        Color gray =
            Color.Lerp(
                source,
                Color.gray,
                0.75f);

        gray.a = source.a;

        return gray;
    }

   public void SetHighlight(bool value)
    {
        isHighlighted = value;

        if (targetIndicator != null)
        {
            targetIndicator.SetActive(value);
        }
    }

    // =========================
    // SKILL CHECK
    // =========================

    private bool CanUseSkill(
        BattleUnit target)
    {
        if (skillData == null)
            return false;

        if (currentCooldown > 0)
            return false;

        int distance =
            Mathf.Abs(
                gridPos.x -
                target.gridPos.x)
            +
            Mathf.Abs(
                gridPos.y -
                target.gridPos.y);

        return distance <=
            skillData.range;
    }
}
