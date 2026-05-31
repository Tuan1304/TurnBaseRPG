using System;

[Serializable]
public class HeroSaveData
{
    public string heroID;

    public string unitID;

    public string appearanceID;

    public WeaponType weaponType;

    public int level;

    public int currentExp;

    public HeroStats stats =
        new HeroStats();

    public int currentHP;

    public bool isActive;

    public HeroEquipmentSaveData equipment =
        new HeroEquipmentSaveData();
}
