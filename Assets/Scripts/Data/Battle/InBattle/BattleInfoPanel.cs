using TMPro;
using UnityEngine;

public class BattleInfoPanel : MonoBehaviour
{
    public static BattleInfoPanel Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;

    [Header("Skill")]
    public TextMeshProUGUI skillNameText;

    [Header("Preview")]
    public Transform previewRoot;

    public GameObject previewCharacterPrefab;

    [Header("Preview Scale")]
    public float previewScale = 4f;

    private GameObject currentPreview;

    private bool isInitialized;

    public static BattleInfoPanel GetInstance()
    {
        if (Instance != null)
            return Instance;

        Instance =
            FindFirstObjectByType<BattleInfoPanel>(
                FindObjectsInactive.Include);

        return Instance;
    }

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        Instance = this;

        isInitialized = true;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        HidePanelObject();
    }

    // =========================
    // UPDATE
    // =========================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleCurrentUnitInfo();
        }
    }

    // =========================
    // TOGGLE
    // =========================

    public void ToggleCurrentUnitInfo()
    {
        if (GetPanelObject().activeSelf)
        {
            Hide();
            return;
        }

        BattleUnit unit =
            BattleManager.Instance.selectedUnit;

        // nếu chưa chọn thì lấy unit hover

        if (unit == null)
        {
            BattleUnit[] allUnits =
                FindObjectsByType<BattleUnit>(
                    FindObjectsSortMode.None);

            foreach (BattleUnit u in allUnits)
            {
                if (u.isMouseOver)
                {
                    unit = u;
                    break;
                }
            }
        }

        if (unit == null)
            return;

        Show(unit);
    }

    // =========================
    // SHOW
    // =========================

    public void Show(BattleUnit unit)
    {
        if (!isInitialized)
        {
            Instance = this;

            isInitialized = true;
        }

        gameObject.SetActive(true);

        if (panel != null)
        {
            panel.SetActive(true);
        }

        transform.SetAsLastSibling();

        SetupInfo(unit);

        CreatePreview(unit);

        BattleUI.Instance.ShowActionMenu(false);
        BattleUI.Instance.ShowSkillPanel(false);
    }

    // =========================
    // HIDE
    // =========================

    public void Hide()
    {
        Hide(false);
    }

    public void HideAndReturnToActionMenu()
    {
        Hide(true);
    }

    public void OnClickClose()
    {
        HideAndReturnToActionMenu();
    }

    private void Hide(
        bool returnToActionMenu)
    {
        HidePanelObject();

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        if (BattleCharacterPreview.Instance != null)
        {
            BattleCharacterPreview.Instance
                .ClearPreview();
        }

        if (returnToActionMenu &&
            BattleManager.Instance != null)
        {
            BattleManager.Instance
                .ReturnToActionMenuFromInfo();
        }
    }

    public bool IsVisible()
    {
        if (panel != null)
        {
            return panel.activeInHierarchy;
        }

        return gameObject.activeInHierarchy;
    }

    // =========================
    // SETUP INFO
    // =========================

    private void SetupInfo(BattleUnit unit)
    {
        // PLAYER

        if (unit.heroData != null &&
            unit.heroData.unitData != null)
        {
            HeroData hero =
                unit.heroData;

            nameText.text =
                hero.unitData.unitName;

            classText.text =
                hero.unitData.unitClass.ToString();
        }

        // ENEMY

        else if (unit.enemyData != null)
        {
            EnemyData enemy =
                unit.enemyData;

            nameText.text =
                enemy.enemyName;

            classText.text =
                enemy.unitClass.ToString();
        }

        else
        {
            nameText.text = "Unknown";
            classText.text = "Unknown";
        }

        hpText.text =
            unit.currentHP + " / " + unit.runtimeStats.hp;

        atkText.text =
            unit.runtimeStats.attack.ToString();

        defText.text =
            unit.runtimeStats.defense.ToString();

        if (unit.skillData != null)
        {
            skillNameText.text =
                unit.skillData.skillName;
        }
        else
        {
            skillNameText.text =
                "None";
        }
    }

    // =========================
    // CREATE PREVIEW
    // =========================

    private void CreatePreview(BattleUnit unit)
    {
        if (BattleCharacterPreview.Instance != null)
        {
            if (unit.heroData != null)
            {
                BattleCharacterPreview.Instance
                    .ShowHero(unit.heroData);

                return;
            }

            if (unit.enemyData != null)
            {
                BattleCharacterPreview.Instance
                    .ShowEnemy(unit.enemyData);

                return;
            }
        }

        if (previewCharacterPrefab == null ||
            previewRoot == null)
        {
            return;
        }

        // XÓA PREVIEW CŨ

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        // SPAWN VÀO UI ROOT

        currentPreview = Instantiate(
            previewCharacterPrefab,
            previewRoot);

        // RESET UI TRANSFORM

        RectTransform rect =
            currentPreview.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchorMin =
                new Vector2(0.5f, 0.5f);

            rect.anchorMax =
                new Vector2(0.5f, 0.5f);

            rect.pivot =
                new Vector2(0.5f, 0.5f);

            rect.anchoredPosition =
                Vector2.zero;

            rect.localRotation =
                Quaternion.identity;

            rect.localScale =
                Vector3.one * previewScale;
        }

        // CHARACTER VISUAL

        CharacterVisual visual =
            currentPreview
            .GetComponentInChildren<CharacterVisual>();

        if (visual == null)
        {
            Debug.LogError(
                "Missing CharacterVisual!");

            return;
        }

        // TẮT ANIMATION

        Animator anim =
            currentPreview
            .GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.enabled = false;
        }

        // APPLY APPEARANCE

        if (unit.heroData != null)
        {
            visual.ApplyHero(
                unit.heroData);
        }
        else if (unit.enemyData != null)
        {
            visual.ApplyAppearance(
                unit.enemyData.appearance);
        }

        // ĐƯA LÊN TRÊN CÙNG

        currentPreview.transform
            .SetAsLastSibling();
    }

    private GameObject GetPanelObject()
    {
        if (panel != null)
            return panel;

        return gameObject;
    }

    private void HidePanelObject()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
