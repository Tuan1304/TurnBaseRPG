using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // =========================
    // WAITING UI
    // =========================

    [Header("Waiting UI")]
    public Transform content;

    public GameObject heroItemPrefab;

    // =========================
    // DETAIL PANEL
    // =========================

    [Header("Detail Panel")]
    public GameObject detailPanel;

    public TextMeshProUGUI detailName;

    [Header("Detail Action Buttons")]
    public GameObject addButton;

    public GameObject removeButton;

    // =========================
    // HERO INFO
    // =========================

    [Header("Hero Info")]
    public TextMeshProUGUI rarityText;

    public TextMeshProUGUI classText;

    public TextMeshProUGUI levelText;

    // =========================
    // MAIN STATS
    // =========================

    [Header("Main Stats")]
    public TextMeshProUGUI hpText;

    public TextMeshProUGUI attackText;

    public TextMeshProUGUI defText;

    // =========================
    // EXTRA STATS
    // =========================

    [Header("Extra Stats")]
    public TextMeshProUGUI magicDefText;

    public TextMeshProUGUI apText;

    public TextMeshProUGUI critText;

    public TextMeshProUGUI dodgeText;

    // =========================
    // HP BAR
    // =========================

    [Header("HP Bar")]
    public Image hpFill;

    [Header("EXP Bar")]
    public Image expFill;

    public TextMeshProUGUI expText;

    // =========================
    // EQUIPMENT UI
    // =========================

    [Header("Equipment UI")]
    public EquipmentSlotUI weaponSlotUI;

    public EquipmentSlotUI armorSlotUI;

    public EquipmentSlotUI helmetSlotUI;

    public EquipmentSlotUI pantsSlotUI;

    public GameObject equipmentSelectionPanel;

    public Transform equipmentListContent;

    public EquipmentItemUI equipmentItemPrefab;

    public TextMeshProUGUI equipmentSelectionTitle;

    private EquipmentSlot selectedEquipmentSlot;

    // =========================
    // CURRENT HERO
    // =========================

    private HeroData currentHero;

    private bool currentDetailShowsRecruitActions;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance
                .RefreshHomeScene();
        }
    }

    private void OnEnable()
    {
        RefreshWaitingUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // =========================
    // REFRESH WAITING UI
    // =========================

    public void RefreshWaitingUI()
    {
        if (content == null ||
            heroItemPrefab == null ||
            UnitManager.Instance == null)
        {
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (HeroData hero
            in UnitManager.Instance.waitingHeroes)
        {
            GameObject go =
                Instantiate(
                    heroItemPrefab,
                    content);

            go.GetComponent<HeroItemUI>()
                .Setup(hero);
        }
    }

    // =========================
    // OPEN DETAIL
    // =========================

    public void OpenDetail(HeroData hero)
    {
        OpenDetail(
            hero,
            true);
    }

    public void OpenOwnedHeroDetail(
        HeroData hero)
    {
        OpenDetail(
            hero,
            false);
    }

    private void OpenDetail(
        HeroData hero,
        bool showRecruitActions)
    {
        currentHero = hero;

        currentDetailShowsRecruitActions =
            showRecruitActions;

        detailPanel.SetActive(true);

        detailPanel.transform
            .SetAsLastSibling();

        SetRecruitActionButtons(
            showRecruitActions);

        // =====================
        // BASIC INFO
        // =====================

        detailName.text =
            hero.unitData.unitName;

        HeroPreviewSpawner.Instance
            .ShowPreview(hero);

        classText.text =
            hero.unitData.unitClass
            .ToString();

        rarityText.text =
            hero.unitData.rarity
            .ToString();

        levelText.text =
            "LV. " +
            hero.level;

        RefreshExpUI(hero);

        HeroStats displayStats =
            EquipmentUtility.GetTotalStats(
                hero,
                InventoryManager.Instance);

        if (hero.currentHP == hero.stats.hp &&
            displayStats.hp > hero.currentHP)
        {
            hero.currentHP = displayStats.hp;

            UnitManager.Instance.SaveHeroes();
        }

        // =====================
        // MAIN STATS
        // =====================

        hpText.text =
            hero.currentHP +
            "/" +
            displayStats.hp;

        attackText.text =
            displayStats.attack
            .ToString();

        defText.text =
            displayStats.defense
            .ToString();

        // =====================
        // EXTRA STATS
        // =====================

        magicDefText.text =
            displayStats.magicDefense
            .ToString();

        apText.text =
            displayStats.abilityPower
            .ToString();

        critText.text =
            displayStats.critChance
            + "%";

        dodgeText.text =
            displayStats.evade
            + "%";

        // =====================
        // HP FILL
        // =====================

        hpFill.fillAmount =
            (float)hero.currentHP /
            displayStats.hp;

        // =====================
        // RARITY COLOR
        // =====================

        SetupRarityColor(
            hero.unitData.rarity);

        RefreshEquipmentSlots();
    }

    private void RefreshExpUI(
        HeroData hero)
    {
        AutoBindExpUI();

        if (hero == null)
            return;

        if (hero.level >= HeroLevelUtility.MaxLevel)
        {
            if (expFill != null)
            {
                expFill.fillAmount = 1f;
            }

            if (expText != null)
            {
                expText.text = "MAX";
            }

            return;
        }

        int requiredExp =
            HeroLevelUtility.GetRequiredExp(
                hero.level);

        float expRatio =
            requiredExp > 0
            ? Mathf.Clamp01(
                (float)hero.currentExp /
                requiredExp)
            : 0f;

        if (expFill != null)
        {
            expFill.fillAmount = expRatio;
        }

        if (expText != null)
        {
            expText.text =
                Mathf.FloorToInt(
                    expRatio * 100f)
                + "%";
        }
    }

    private void AutoBindExpUI()
    {
        if (detailPanel == null)
            return;

        Transform expRoot =
            FindDeepChild(
                detailPanel.transform,
                "EXP");

        if (expRoot == null)
            return;

        if (expFill == null)
        {
            Transform fill =
                FindDeepChild(
                    expRoot,
                    "Fill");

            if (fill != null)
            {
                expFill =
                    fill.GetComponent<Image>();
            }
        }

        if (expText == null)
        {
            Transform text =
                FindDeepChild(
                    expRoot,
                    "EXPText");

            if (text != null)
            {
                expText =
                    text.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private Transform FindDeepChild(
        Transform parent,
        string childName)
    {
        if (parent == null)
            return null;

        foreach (Transform child
            in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform result =
                FindDeepChild(
                    child,
                    childName);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private void SetRecruitActionButtons(
        bool visible)
    {
        if (addButton != null)
        {
            addButton.SetActive(
                visible);
        }

        if (removeButton != null)
        {
            removeButton.SetActive(
                visible);
        }
    }

    public void OpenEquipmentSelection(
        EquipmentSlot slot)
    {
        selectedEquipmentSlot = slot;

        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(true);

            equipmentSelectionPanel
                .transform
                .SetAsLastSibling();
        }

        if (equipmentSelectionTitle != null)
        {
            equipmentSelectionTitle.text =
                slot.ToString();
        }

        RefreshEquipmentList();
    }

    public void EquipSelectedItem(
        InventoryItemSaveData item)
    {
        if (currentHero == null ||
            item == null ||
            InventoryManager.Instance == null)
        {
            return;
        }

        if (!InventoryManager.Instance
            .EquipItem(
                currentHero,
                item))
        {
            return;
        }

        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }

        UnitManager.Instance
            .RefreshActiveHeroVisuals();

        StartCoroutine(
            RefreshCurrentHeroAfterEquipmentChange());
    }

    public void CloseEquipmentSelection()
    {
        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }
    }

    public void UnequipSelectedSlot()
    {
        if (currentHero == null ||
            InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance
            .UnequipItem(
                currentHero,
                selectedEquipmentSlot);

        if (equipmentSelectionPanel != null)
        {
            equipmentSelectionPanel.SetActive(false);
        }

        StartCoroutine(
            RefreshCurrentHeroAfterEquipmentChange());
    }

    private IEnumerator RefreshCurrentHeroAfterEquipmentChange()
    {
        if (currentHero == null)
            yield break;

        if (UnitManager.Instance != null)
        {
            UnitManager.Instance
                .RefreshActiveHeroVisuals();
        }

        if (HeroPortraitGenerator.Instance != null)
        {
            yield return StartCoroutine(
                HeroPortraitGenerator
                .Instance
                .GeneratePortrait(currentHero));
        }

        RefreshWaitingUI();

        HeroRosterUI rosterUI =
            FindFirstObjectByType<HeroRosterUI>();

        if (rosterUI != null)
        {
            rosterUI.Refresh();
        }

        TeamSelectionUI teamSelectionUI =
            FindFirstObjectByType<TeamSelectionUI>();

        if (teamSelectionUI != null)
        {
            teamSelectionUI.RefreshUI();
        }

        OpenDetail(
            currentHero,
            currentDetailShowsRecruitActions);
    }

    private void RefreshEquipmentSlots()
    {
        InventoryManager inventoryManager =
            InventoryManager.Instance;

        if (inventoryManager == null ||
            currentHero == null)
        {
            return;
        }

        if (weaponSlotUI != null)
        {
            weaponSlotUI.Refresh(
                currentHero,
                inventoryManager);
        }

        if (armorSlotUI != null)
        {
            armorSlotUI.Refresh(
                currentHero,
                inventoryManager);
        }

        if (helmetSlotUI != null)
        {
            helmetSlotUI.Refresh(
                currentHero,
                inventoryManager);
        }

        if (pantsSlotUI != null)
        {
            pantsSlotUI.Refresh(
                currentHero,
                inventoryManager);
        }
    }

    private void RefreshEquipmentList()
    {
        if (equipmentListContent == null ||
            equipmentItemPrefab == null ||
            InventoryManager.Instance == null)
        {
            return;
        }

        foreach (Transform child
            in equipmentListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItemSaveData item
            in InventoryManager
                .Instance
                .GetItemsBySlot(
                selectedEquipmentSlot))
        {
            if (InventoryManager.Instance
                .IsEquippedByOtherHero(
                    item.instanceID,
                    currentHero))
            {
                continue;
            }

            ItemData itemData =
                InventoryManager.Instance
                .GetItemData(item);

            if (!InventoryManager.Instance
                .CanHeroEquip(
                    currentHero,
                    itemData))
            {
                continue;
            }

            EquipmentItemUI itemUI =
                Instantiate(
                    equipmentItemPrefab,
                    equipmentListContent);

            itemUI.Setup(
                item,
                InventoryManager.Instance);
        }
    }

    // =========================
    // RARITY COLOR
    // =========================

    private void SetupRarityColor(
        Rarity rarity)
    {
        Color rarityColor =
            Color.white;

        switch (rarity)
        {
            case Rarity.Common:

                rarityColor =
                    Color.gray;

                break;

            case Rarity.Rare:

                rarityColor =
                    Color.blue;

                break;

            case Rarity.Epic:

                rarityColor =
                    new Color(
                        0.7f,
                        0.3f,
                        1f);

                break;

            case Rarity.Legendary:

                rarityColor =
                    new Color(
                        1f,
                        0.7f,
                        0f);

                break;
        }

        rarityText.color =
            rarityColor;
    }

    private ItemDatabase GetItemDatabase()
    {
        if (InventoryManager.Instance == null)
            return null;

        return InventoryManager.Instance.itemDatabase;
    }

    // =========================
    // CLOSE DETAIL
    // =========================

    public void CloseDetail()
    {
        detailPanel.SetActive(false);
    }

    // =========================
    // ADD HERO
    // =========================

    public void OnAdd()
    {
        if (currentHero == null ||
            UnitManager.Instance == null ||
            !UnitManager.Instance.waitingHeroes.Contains(currentHero) ||
            UnitManager.Instance.activeHeroes.Count >=
            UnitManager.Instance.maxActiveUnit)
        {
            return;
        }

        UnitManager.Instance
            .AddToVillage(currentHero);

        if (HeroVillageSpawner.Instance != null)
        {
            HeroVillageSpawner.Instance
                .SpawnHero(currentHero);
        }

        CloseDetail();
    }

    // =========================
    // REMOVE HERO
    // =========================

    public void OnRemove()
    {
        if (currentHero == null)
            return;

        UnitManager.Instance
            .RemoveHero(currentHero);

        CloseDetail();
    }
}
