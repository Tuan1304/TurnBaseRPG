using System;

[Serializable]
public class StatusEffect
{
    public EffectType effectType;

    public int duration;

    // =========================
    // STAT
    // =========================

    public int attackBuff;

    public int defenseBuff;

    // =========================
    // DAMAGE OVER TIME
    // =========================

    public int tickDamage;

    // =========================
    // CONTROL
    // =========================

    public bool preventAction;
}