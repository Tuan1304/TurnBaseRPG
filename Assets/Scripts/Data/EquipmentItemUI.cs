using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentItemUI : MonoBehaviour
{
    public Image backgroundImage;

    public Image iconImage;

    public TextMeshProUGUI nameText;

    public TextMeshProUGUI rarityText;

    public TextMeshProUGUI statsText;

    private InventoryItemSaveData item;

    public void Setup(
        InventoryItemSaveData itemSave,
        InventoryManager inventoryManager)
    {
        item = itemSave;

        if (backgroundImage == null)
        {
            backgroundImage =
                GetComponent<Image>();
        }

        ItemData itemData =
            inventoryManager.GetItemData(
                itemSave);

        if (itemData == null)
            return;

        if (iconImage != null)
        {
            iconImage.sprite =
                itemData.icon;

            iconImage.enabled =
                itemData.icon != null;
        }

        if (nameText != null)
        {
            nameText.text =
                itemData.itemName;
        }

        if (rarityText != null)
        {
            rarityText.text = "";
        }

        if (backgroundImage != null)
        {
            backgroundImage.color =
                GetRarityBackgroundColor(
                    itemSave.rarity);
        }

        if (statsText != null)
        {
            statsText.text =
                GetStatsText(
                    itemSave.bonusStats);
        }
    }

    public void OnClick()
    {
        UIManager.Instance
            .EquipSelectedItem(item);
    }

    private string GetStatsText(
        EquipmentStats stats)
    {
        if (stats == null)
            return "";

        string text = "";

        AddStat(ref text, "HP", stats.hp);
        AddStat(ref text, "ATK", stats.attack);
        AddStat(ref text, "DEF", stats.defense);
        AddStat(ref text, "MDEF", stats.magicDefense);
        AddStat(ref text, "AP", stats.abilityPower);
        AddStat(ref text, "CRIT", stats.critChance);
        AddStat(ref text, "EVA", stats.evade);

        return text;
    }

    private void AddStat(
        ref string text,
        string label,
        int value)
    {
        if (value == 0)
            return;

        if (text.Length > 0)
            text += "  ";

        text += label + " +" + value;
    }

    private void AddStat(
        ref string text,
        string label,
        float value)
    {
        if (Mathf.Abs(value) < 0.01f)
            return;

        if (text.Length > 0)
            text += "  ";

        text +=
            label +
            " +" +
            value.ToString("0.##") +
            "%";
    }

    private Color GetRarityColor(
        Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Rare:

                return Color.blue;

            case Rarity.Epic:

                return new Color(0.6f, 0.2f, 1f);

            case Rarity.Legendary:

                return new Color(1f, 0.7f, 0f);
        }

        return Color.gray;
    }

    private Color GetRarityBackgroundColor(
        Rarity rarity)
    {
        Color color =
            GetRarityColor(rarity);

        return new Color(
            color.r,
            color.g,
            color.b,
            0.75f);
    }
}
