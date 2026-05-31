using System;
using UnityEngine;

[Serializable]
public class HeroData
{
    [Header("Base Unit")]
    public UnitData unitData;

        [Header("Save")]
    public string heroID;

    [Header("Database")]
    public string unitID;

    [Header("Appearance")]
    public CharacterAppearanceData appearanceData;
    public WeaponType weaponType;

    [Header("Portrait")]
    public Sprite portrait;
    public RenderTexture portraitTexture;

    [Header("Stats")]
    public HeroStats stats =
        new HeroStats();

    public int currentHP;

    public int level;

    public int currentExp;

    [Header("State")]
    public HeroState heroState;

    [Header("Equipment")]
    public HeroEquipmentSaveData equipment =
        new HeroEquipmentSaveData();
}

public enum HeroState
{
    Waiting,
    Active
}
