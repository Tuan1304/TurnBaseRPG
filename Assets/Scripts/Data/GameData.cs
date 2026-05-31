using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public List<HeroSaveData> heroes = new List<HeroSaveData>();

    public List<InventoryItemSaveData> inventory =
        new List<InventoryItemSaveData>();

    // =========================
    // RESOURCES
    // =========================

    public int gold;
    public int wood;
    public int stone;
    public int food;


    // =========================
    // PROGRESS
    // =========================

    public int currentStage;

    // =========================
    // HEROES
    // =========================

    public List<string> unlockedHeroes =
        new List<string>();
}
