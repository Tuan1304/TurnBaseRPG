using UnityEngine;

public class CharacterVisual : MonoBehaviour
{
    [Header("Head")]
    public SpriteRenderer hairRenderer;

    [Header("Body")]
    public SpriteRenderer bodyRenderer;

    [Header("Arms")]
    public SpriteRenderer leftArmRenderer;
    public SpriteRenderer rightArmRenderer;

    [Header("Legs")]
    public SpriteRenderer leftLegRenderer;
    public SpriteRenderer rightLegRenderer;

    [Header("Armor")]
    public SpriteRenderer armorBodyRenderer;
    public SpriteRenderer armorLeftArmRenderer;
    public SpriteRenderer armorRightArmRenderer;
    public SpriteRenderer armorLeftLegRenderer;
    public SpriteRenderer armorRightLegRenderer;

    [Header("Weapon")]
    public SpriteRenderer weaponRenderer;

    public void ApplyHero(
        HeroData hero)
    {
        ApplyHero(
            hero,
            GetItemDatabase());
    }

    public void ApplyHero(
        HeroData hero,
        ItemDatabase itemDatabase)
    {
        if (hero == null)
            return;

        ApplyAppearance(
            hero.appearanceData);

        ApplyEquipment(
            hero.equipment,
            itemDatabase);
    }

    private ItemDatabase GetItemDatabase()
    {
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.itemDatabase != null)
        {
            return InventoryManager.Instance
                .itemDatabase;
        }

        return InventoryManager.CachedItemDatabase;
    }

    public void ApplyAppearance(CharacterAppearanceData data)
    {
        if (data == null)
        {
            Debug.LogWarning(
                "Missing appearance data!");

            return;
        }

        hairRenderer.sprite = data.hair;

        bodyRenderer.sprite = data.body;

        leftArmRenderer.sprite = data.leftArm;
        rightArmRenderer.sprite = data.rightArm;

        leftLegRenderer.sprite = data.leftLeg;
        rightLegRenderer.sprite = data.rightLeg;

        armorBodyRenderer.sprite = data.armorBody;

        armorLeftArmRenderer.sprite = data.armorLeftArm;
        armorRightArmRenderer.sprite = data.armorRightArm;

        armorLeftLegRenderer.sprite = data.armorLeftLeg;
        armorRightLegRenderer.sprite = data.armorRightLeg;

        weaponRenderer.sprite = data.weapon;
    }

    public void ApplyEquipment(
        HeroEquipmentSaveData equipment,
        InventoryManager inventoryManager)
    {
        ApplyEquipment(
            equipment,
            GetItemDatabase());
    }

    public void ApplyEquipment(
        HeroEquipmentSaveData equipment,
        ItemDatabase itemDatabase)
    {
        if (equipment == null)
        {
            return;
        }

        ApplyHelmet(
            GetEquippedItemData(
                equipment,
                EquipmentSlot.Helmet,
                itemDatabase));

        ApplyWeapon(
            GetEquippedItemData(
                equipment,
                EquipmentSlot.Weapon,
                itemDatabase));

        ApplyArmor(
            GetEquippedItemData(
                equipment,
                EquipmentSlot.Armor,
                itemDatabase));

        ApplyPants(
            GetEquippedItemData(
                equipment,
                EquipmentSlot.Pants,
                itemDatabase));
    }

    private ItemData GetEquippedItemData(
        HeroEquipmentSaveData equipment,
        EquipmentSlot slot,
        ItemDatabase itemDatabase)
    {
        InventoryItemSaveData itemSave =
            EquipmentUtility.GetEquippedItem(
                equipment,
                slot,
                InventoryManager.Instance);

        if (itemSave == null)
            return null;

        if (InventoryManager.Instance != null)
        {
            ItemData itemData =
                InventoryManager.Instance
                .GetItemData(itemSave);

            if (itemData != null)
                return itemData;
        }

        if (itemDatabase != null)
        {
            return itemDatabase.GetItem(
                itemSave.itemID);
        }

        return InventoryManager
            .GetCachedItemData(itemSave);
    }

    private void ApplyHelmet(
        ItemData item)
    {
        if (item == null ||
            item.helmetSprite == null)
        {
            return;
        }

        hairRenderer.sprite =
            item.helmetSprite;
    }

    private void ApplyWeapon(
        ItemData item)
    {
        if (item == null ||
            item.weaponSprite == null)
        {
            return;
        }

        weaponRenderer.sprite =
            item.weaponSprite;
    }

    private void ApplyArmor(
        ItemData item)
    {
        if (item == null)
            return;

        if (item.armorBodySprite != null)
        {
            armorBodyRenderer.sprite =
                item.armorBodySprite;
        }

        if (item.armorLeftArmSprite != null)
        {
            armorLeftArmRenderer.sprite =
                item.armorLeftArmSprite;
        }

        if (item.armorRightArmSprite != null)
        {
            armorRightArmRenderer.sprite =
                item.armorRightArmSprite;
        }
    }

    private void ApplyPants(
        ItemData item)
    {
        if (item == null)
            return;

        if (item.pantsLeftLegSprite != null)
        {
            armorLeftLegRenderer.sprite =
                item.pantsLeftLegSprite;
        }

        if (item.pantsRightLegSprite != null)
        {
            armorRightLegRenderer.sprite =
                item.pantsRightLegSprite;
        }
    }

    public Sprite GetPortraitSprite()
    {
        return bodyRenderer.sprite;
    }
}
