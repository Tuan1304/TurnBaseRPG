using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Game/ Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string unitName;
    public Sprite avatar;

    [Header("Base Stats")]
    public HeroStats baseStats;

    [Header("Type")]
    public UnitClass unitClass;
    public Rarity rarity;

    [Header("Skills")]
    public SkillData skill;

    [Header("Attack Projectile")]
    public Projectile attackProjectile;

    private void OnValidate()
    {
        id = name;
    }
} 

public enum UnitClass
{
    Warrior,
    Archer,
    Mage
}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
