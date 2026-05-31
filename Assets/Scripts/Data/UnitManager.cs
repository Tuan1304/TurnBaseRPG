using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public UnitDatabase database;

    public List<HeroData> waitingHeroes = new List<HeroData>();
    public List<HeroData> activeHeroes = new List<HeroData>();
    public List<CharacterAppearanceData> appearances;

    public int maxActiveUnit  = 8;

    private bool isInitializingScene;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(this);

            return;
        }

        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(
            InitializeForCurrentScene());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode)
    {
        if (scene.name != "HomeScene")
            return;

        StartCoroutine(
            InitializeForCurrentScene());
    }

    public void RefreshHomeScene()
    {
        StartCoroutine(
            InitializeForCurrentScene());
    }

    private IEnumerator InitializeForCurrentScene()
    {
        if (isInitializingScene)
            yield break;

        isInitializingScene = true;

        yield return null;

        yield return new WaitUntil(
            () =>
                GameDataManager.Instance != null &&
                GameDataManager.Instance.Data != null &&
                InventoryManager.Instance != null &&
                UIManager.Instance != null &&
                HeroPortraitGenerator.Instance != null &&
                HeroVillageSpawner.Instance != null);

        LoadHeroes();

        Debug.Log(
            "Loaded waiting heroes: " +
            waitingHeroes.Count +
            ", active heroes: " +
            activeHeroes.Count);

        if (BattleTeamManager.Instance != null)
        {
            BattleTeamManager.Instance
                .SyncWithAvailableHeroes(
                    activeHeroes);
        }

        yield return StartCoroutine(
            GenerateLoadedHeroPortraits());

        UIManager.Instance.RefreshWaitingUI();

        SpawnActiveHeroes();

        RefreshSceneHeroLists();

        isInitializingScene = false;
    }

    private void RefreshSceneHeroLists()
    {
        ApplyAdventureReturnState();

        HeroRosterUI rosterUI =
            FindFirstObjectByType<HeroRosterUI>(
                FindObjectsInactive.Include);

        if (rosterUI != null)
        {
            rosterUI.Refresh();
        }

        TeamSelectionUI teamSelectionUI =
            FindFirstObjectByType<TeamSelectionUI>(
                FindObjectsInactive.Include);

        if (teamSelectionUI != null)
        {
            teamSelectionUI.RefreshUI();
        }
    }

    private void ApplyAdventureReturnState()
    {
        AdventureUI adventureUI =
            FindFirstObjectByType<AdventureUI>(
                FindObjectsInactive.Include);

        if (adventureUI == null)
            return;

        adventureUI.gameObject.SetActive(
            BattleStageSelection
            .ShowAdventureAfterReturn);

        BattleStageSelection.ShowAdventureAfterReturn =
            false;
    }

    public void SummonHero()
    {
        StartCoroutine(SummonRoutine());
    }

    private IEnumerator SummonRoutine()
    {
        if (database == null || database.units.Count == 0)
        {
            Debug.LogWarning("Database rỗng!");
            yield break;
        }

        UnitData randomUnit =
            database.units[
                Random.Range(0, database.units.Count)];

        HeroData hero = CreateHero(randomUnit);

        // generate portrait
        yield return StartCoroutine(
            HeroPortraitGenerator.Instance.GeneratePortrait(hero));

        waitingHeroes.Add(hero);

        SaveHeroes();

        UIManager.Instance.RefreshWaitingUI();
    }

    private HeroData CreateHero(UnitData data)
    {
        HeroData hero = new HeroData();

        hero.heroID = Guid.NewGuid().ToString();

        hero.unitID = data.name;

        hero.unitData = data;

        // =========================
        // RANDOM APPEARANCE BY CLASS
        // =========================

        List<CharacterAppearanceData> validAppearances =
            appearances.FindAll(
                a => a.allowedClass ==
                data.unitClass);

        if (validAppearances.Count > 0)
        {
            hero.appearanceData =
                validAppearances[
                    Random.Range(
                        0,
                        validAppearances.Count)];
        }
        else
        {
            Debug.LogWarning(
                "No appearance for class: "
                + data.unitClass);
        }

        hero.weaponType = GetRandomWeapon(data.unitClass);

        hero.level = 1;

        hero.currentExp = 0;

        hero.stats.hp =
            data.baseStats.hp
            + Random.Range(-5, 6);

        hero.stats.attack =
            data.baseStats.attack
            + Random.Range(-2, 3);

        hero.stats.defense =
            data.baseStats.defense
            + Random.Range(-2, 3);

        // MAGIC

        hero.stats.magicDefense =
            data.baseStats.magicDefense
            + Random.Range(-1, 2);

        hero.stats.abilityPower =
            data.baseStats.abilityPower
            + Random.Range(-1, 2);

        // PERCENT STATS

        hero.stats.critChance =
            data.baseStats.critChance;

        hero.stats.evade =
            data.baseStats.evade;

        // CURRENT HP

        hero.currentHP =
            hero.stats.hp;

        hero.heroState = HeroState.Waiting;

        return hero;
    }

    private WeaponType GetRandomWeapon(UnitClass unitClass)
    {
        switch (unitClass)
        {
            case UnitClass.Warrior:

                return WeaponType.Sword;

            case UnitClass.Archer:

                return WeaponType.Bow;

            case UnitClass.Mage:

                return WeaponType.Staff;
        }

        return WeaponType.Sword;
}

    public void AddToVillage(HeroData hero)
    {
        if (activeHeroes.Count >= maxActiveUnit )
        {
            Debug.Log("Đã đầy slot!");
            return;
        }

        if (!waitingHeroes.Contains(hero)) return;

        waitingHeroes.Remove(hero);
        hero.heroState = HeroState.Active;
        activeHeroes.Add(hero);

        SaveHeroes();

        UIManager.Instance.RefreshWaitingUI();
    }

    public void RemoveHero(HeroData hero)
    {
        if (!waitingHeroes.Contains(hero))
            return;

        waitingHeroes.Remove(hero);
        SaveHeroes();
        UIManager.Instance.RefreshWaitingUI();
    }

    private void SpawnActiveHeroes()
    {
        if (HeroVillageSpawner.Instance == null)
            return;

        HeroEntity[] existingHeroes =
            FindObjectsByType<HeroEntity>(
                FindObjectsSortMode.None);

        foreach (HeroEntity entity
            in existingHeroes)
        {
            if (entity != null)
            {
                Destroy(entity.gameObject);
            }
        }

        foreach (HeroData hero
            in activeHeroes)
        {
            HeroVillageSpawner.Instance
                .SpawnHero(hero);
        }
    }

    private IEnumerator GenerateLoadedHeroPortraits()
    {
        if (HeroPortraitGenerator.Instance == null)
            yield break;

        foreach (HeroData hero
            in waitingHeroes)
        {
            yield return StartCoroutine(
                HeroPortraitGenerator.Instance
                .GeneratePortrait(hero));
        }

        foreach (HeroData hero
            in activeHeroes)
        {
            yield return StartCoroutine(
                HeroPortraitGenerator.Instance
                .GeneratePortrait(hero));
        }
    }

    public void RefreshActiveHeroVisuals()
    {
        HeroEntity[] entities =
            FindObjectsByType<HeroEntity>(
                FindObjectsSortMode.None);

        foreach (HeroEntity entity
            in entities)
        {
            if (entity == null ||
                entity.heroData == null)
            {
                continue;
            }

            HeroData hero =
                activeHeroes.Find(
                    activeHero =>
                        activeHero.heroID ==
                        entity.heroData.heroID);

            if (hero == null)
                continue;

            entity.heroData = hero;

            entity.RefreshVisual();
        }
    }

    public void SaveHeroes()
    {
        int totalHeroCount =
            waitingHeroes.Count +
            activeHeroes.Count;

        bool hasSavedHeroes =
            GameDataManager.Instance != null &&
            GameDataManager.Instance.Data != null &&
            GameDataManager.Instance.Data.heroes != null &&
            GameDataManager.Instance.Data.heroes.Count > 0;

        if (totalHeroCount == 0 &&
            hasSavedHeroes)
        {
            Debug.LogWarning(
                "Blocked saving empty hero list over existing save.");

            return;
        }

        GameDataManager
            .Instance
            .Data
            .heroes
            .Clear();

        // waiting

        foreach (HeroData hero
            in waitingHeroes)
        {
            GameDataManager
                .Instance
                .Data
                .heroes
                .Add(
                    HeroSerializer
                    .ToSaveData(hero));
        }

        // active

        foreach (HeroData hero
            in activeHeroes)
        {
            GameDataManager
                .Instance
                .Data
                .heroes
                .Add(
                    HeroSerializer
                    .ToSaveData(hero));
        }

        GameDataManager
            .Instance
            .SaveGame();
    }

    public void LoadHeroes()
    {
        waitingHeroes.Clear();

        activeHeroes.Clear();

        foreach (HeroSaveData save
            in GameDataManager
            .Instance
            .Data
            .heroes)
        {
            HeroData hero =
                HeroSerializer
                .ToHeroData(
                    save,
                    database,
                    appearances);

            if (hero == null)
                continue;

            if (hero.heroState ==
                HeroState.Active)
            {
                activeHeroes.Add(hero);
            }
            else
            {
                waitingHeroes.Add(hero);
            }
        }
    }
}
