using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyData",
    menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Info")]
    public string enemyName;

    public UnitClass unitClass;

    [Header("Stats")]
    public HeroStats stats;

    [Header("Skill")]
    public SkillData skill;

    [Header("Appearance")]
    public CharacterAppearanceData appearance;

    [Header("Projectile")]
    public Projectile attackProjectile;
}