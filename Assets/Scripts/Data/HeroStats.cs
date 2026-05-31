using System;
using UnityEngine;

[Serializable]
public class HeroStats
{
    [Header("Basic")]
    public int hp = 100;

    public int attack = 20;

    public int defense = 5;

    // =========================
    // MAGIC
    // =========================

    public int magicDefense = 0;

    public int abilityPower = 0;

    // =========================
    // SPECIAL
    // =========================

    [Range(0f, 100f)]
    public float critChance = 5f;

    [Range(0f, 100f)]
    public float evade = 0f;
}