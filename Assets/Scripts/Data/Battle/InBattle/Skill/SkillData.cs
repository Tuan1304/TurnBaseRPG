using UnityEngine;

[CreateAssetMenu(
    fileName = "Skill",
    menuName = "Game/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("VFX")]
    public GameObject castEffectPrefab;

    public GameObject hitEffectPrefab;

    [Header("Target")]
    public TargetType targetType;

    [Header("Range")]
    public int range = 3;

    [Header("Damage")]
    public DamageType damageType;

    public int power = 20;

    [Header("AOE")]
    public int aoeRadius = 1;

    [Header("Heal")]
    public int healAmount = 0;

    [Header("Buff")]
    public int buffAttack = 0;

    [Header("Status Effect")]
    public bool applyPoison;

    public int poisonDamage;

    public int poisonDuration;

    public bool applyStun;

    public int stunDuration;

    [Header("Cooldown")]
    public int cooldown = 2;

    [Header("Buff Duration")]
    public int buffDuration = 2;
}