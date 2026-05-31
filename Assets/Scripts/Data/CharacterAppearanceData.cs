using UnityEngine;

[CreateAssetMenu(fileName = "Appearance", menuName = "Game/Appearance Data")]
public class CharacterAppearanceData : ScriptableObject
{
    [Header("ID")]
    public string id;

    [Header("Head")]
    public Sprite hair;

    [Header("Body")]
    public Sprite body;

    [Header("Left Arm")]
    public Sprite leftArm;

    [Header("Right Arm")]
    public Sprite rightArm;

    [Header("Left Leg")]
    public Sprite leftLeg;

    [Header("Right Leg")]
    public Sprite rightLeg;

    [Header("Clothers")]
    public Sprite armorBody;
    public Sprite armorLeftArm;
    public Sprite armorRightArm;
    public Sprite armorLeftLeg;
    public Sprite armorRightLeg;

    [Header("Weapon")]
    public Sprite weapon;
    public UnitClass allowedClass;

}
