using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string id;

    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;

    public Rarity rarity;

    public ItemType itemType;

    [Header("Equipment")]
    public EquipmentSlot equipmentSlot;

    public WeaponType weaponType;

    public EquipmentStats bonusStats =
        new EquipmentStats();

    public EquipmentStats randomRange =
        new EquipmentStats();

    [Header("Equipment Sprites")]
    public Sprite helmetSprite;

    public Sprite weaponSprite;

    public Sprite armorBodySprite;

    public Sprite armorLeftArmSprite;

    public Sprite armorRightArmSprite;

    public Sprite pantsLeftLegSprite;

    public Sprite pantsRightLegSprite;

    [Header("Craft Cost")]
    public int goldCost;

    public int woodCost;

    public int stoneCost;

    public int foodCost;

    private void OnValidate()
    {
        id = name;
    }
}
