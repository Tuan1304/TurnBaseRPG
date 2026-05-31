using TMPro;
using UnityEngine;

public class ForgeUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject forgePanel;

    [Header("List")]
    public Transform content;

    public ForgeItemUI forgeItemPrefab;

    [Header("Title")]
    public TextMeshProUGUI titleText;

    [Header("Resources")]
    public TextMeshProUGUI goldText;

    public TextMeshProUGUI woodText;

    public TextMeshProUGUI stoneText;

    public TextMeshProUGUI foodText;

    private EquipmentSlot currentSlot =
        EquipmentSlot.Weapon;

    private void OnEnable()
    {
        Refresh();
    }

    public void Open()
    {
        if (forgePanel != null)
        {
            forgePanel.SetActive(true);
        }

        Refresh();
    }

    public void Close()
    {
        if (forgePanel != null)
        {
            forgePanel.SetActive(false);
        }
    }

    public void ShowWeapons()
    {
        ShowSlot(
            EquipmentSlot.Weapon);
    }

    public void ShowArmor()
    {
        ShowSlot(
            EquipmentSlot.Armor);
    }

    public void ShowHelmet()
    {
        ShowSlot(
            EquipmentSlot.Helmet);
    }

    public void ShowPants()
    {
        ShowSlot(
            EquipmentSlot.Pants);
    }

    public void ShowSlot(
        EquipmentSlot slot)
    {
        currentSlot = slot;

        Refresh();
    }

    public void Craft(
        ItemData item)
    {
        if (InventoryManager.Instance == null ||
            item == null)
        {
            return;
        }

        if (InventoryManager.Instance.CraftItem(item) == null)
        {
            Debug.Log(
                "Not enough resources to craft: "
                + item.itemName);

            return;
        }

        Refresh();
    }

    private void Refresh()
    {
        RefreshTitle();

        RefreshResources();

        RefreshItemList();
    }

    private void RefreshTitle()
    {
        if (titleText == null)
            return;

        titleText.text =
            currentSlot.ToString();
    }

    private void RefreshResources()
    {
        if (GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null)
        {
            return;
        }

        GameData data =
            GameDataManager.Instance.Data;

        if (goldText != null)
        {
            goldText.text =
                data.gold.ToString();
        }

        if (woodText != null)
        {
            woodText.text =
                data.wood.ToString();
        }

        if (stoneText != null)
        {
            stoneText.text =
                data.stone.ToString();
        }

        if (foodText != null)
        {
            foodText.text =
                data.food.ToString();
        }
    }

    private void RefreshItemList()
    {
        if (content == null ||
            forgeItemPrefab == null ||
            InventoryManager.Instance == null ||
            InventoryManager.Instance.itemDatabase == null)
        {
            return;
        }

        foreach (Transform child
            in content)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item
            in InventoryManager
                .Instance
                .itemDatabase
                .GetEquipmentBySlot(currentSlot))
        {
            ForgeItemUI itemUI =
                Instantiate(
                    forgeItemPrefab,
                    content);

            itemUI.Setup(
                this,
                item);
        }
    }
}
