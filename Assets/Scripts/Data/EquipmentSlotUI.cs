using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    public EquipmentSlot slot;

    public Image iconImage;

    public Sprite emptyIcon;

    public void Refresh(
        HeroData hero,
        InventoryManager inventoryManager)
    {
        if (iconImage == null)
            return;

        InventoryItemSaveData itemSave =
            EquipmentUtility.GetEquippedItem(
                hero.equipment,
                slot,
                inventoryManager);

        ItemData item =
            inventoryManager.GetItemData(
                itemSave);

        if (item != null &&
            item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = emptyIcon;
            iconImage.enabled = emptyIcon != null;
        }
    }

    public void OnClick()
    {
        UIManager.Instance
            .OpenEquipmentSelection(slot);
    }
}
