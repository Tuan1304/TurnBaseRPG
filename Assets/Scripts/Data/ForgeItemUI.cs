using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeItemUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;

    public TextMeshProUGUI nameText;

    public TextMeshProUGUI statsText;

    public TextMeshProUGUI goldCostText;

    public TextMeshProUGUI woodCostText;

    public TextMeshProUGUI stoneCostText;

    public TextMeshProUGUI foodCostText;

    public Button craftButton;

    private ForgeUI forgeUI;

    private ItemData item;

    public void Setup(
        ForgeUI owner,
        ItemData itemData)
    {
        forgeUI = owner;

        item = itemData;

        Refresh();
    }

    public void Craft()
    {
        if (forgeUI == null ||
            item == null)
        {
            return;
        }

        forgeUI.Craft(item);
    }

    private void Refresh()
    {
        if (item == null)
            return;

        if (iconImage != null)
        {
            iconImage.sprite =
                item.icon;

            iconImage.enabled =
                item.icon != null;
        }

        if (nameText != null)
        {
            nameText.text =
                item.itemName;
        }

        if (statsText != null)
        {
            statsText.text =
                GetStatsText(item.bonusStats);
        }

        SetCostText(
            goldCostText,
            item.goldCost);

        SetCostText(
            woodCostText,
            item.woodCost);

        SetCostText(
            stoneCostText,
            item.stoneCost);

        SetCostText(
            foodCostText,
            item.foodCost);

        if (craftButton != null)
        {
            craftButton.interactable =
                CanCraft();
        }
    }

    private bool CanCraft()
    {
        if (GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null ||
            item == null)
        {
            return false;
        }

        GameData data =
            GameDataManager.Instance.Data;

        return
            data.gold >= item.goldCost &&
            data.wood >= item.woodCost &&
            data.stone >= item.stoneCost &&
            data.food >= item.foodCost;
    }

    private void SetCostText(
        TextMeshProUGUI text,
        int cost)
    {
        if (text == null)
            return;

        text.text =
            cost.ToString();
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
            text += "\n";

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
            text += "\n";

        text +=
            label +
            " +" +
            value.ToString("0.##") +
            "%";
    }
}
