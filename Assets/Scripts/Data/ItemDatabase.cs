using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items =
        new List<ItemData>();

    public ItemData GetItem(
        string itemID)
    {
        return items.Find(
            item =>
                item != null &&
                item.id == itemID);
    }

    public List<ItemData> GetEquipmentBySlot(
        EquipmentSlot slot)
    {
        return items.FindAll(
            item =>
                item != null &&
                item.itemType == ItemType.Equipment &&
                item.equipmentSlot == slot);
    }
}
