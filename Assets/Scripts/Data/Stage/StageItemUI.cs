using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageItemUI : MonoBehaviour,
    IPointerClickHandler
{
    [Header("Texts")]
    public TextMeshProUGUI nameText;

    public TextMeshProUGUI taskNameText;

    public TextMeshProUGUI difficultyText;

    [Header("State")]
    public GameObject lockedObject;

    public Image backgroundImage;

    [Header("Colors")]
    public Color lockedColor =
        new Color(0.45f, 0.45f, 0.45f);

    public Color unlockedColor =
        Color.white;

    public Color selectedColor =
        new Color(1f, 0.85f, 0.45f);

    public Color clearedColor =
        new Color(0.7f, 1f, 0.7f);

    private AdventureUI owner;

    private int stageIndex;

    private bool isUnlocked;

    private Vector3 originalScale;

    private Outline selectionOutline;

    private GameObject selectionFrame;

    public void Setup(
        AdventureUI adventureUI,
        StageData stage,
        int index,
        bool unlocked,
        bool cleared,
        bool selected)
    {
        owner = adventureUI;

        stageIndex = index;

        isUnlocked = unlocked;

        AutoBindBackgroundImage();

        if (originalScale == Vector3.zero)
        {
            originalScale =
                transform.localScale;
        }

        if (nameText != null)
        {
            nameText.text =
                stage.stageName;
        }

        if (taskNameText != null)
        {
            taskNameText.text =
                "Muc Tieu: " +
                stage.objective;
        }

        if (difficultyText != null)
        {
            difficultyText.text =
                cleared
                ? "Da qua"
                : unlocked
                    ? stage.difficulty
                    : "Da khoa";
        }

        if (lockedObject != null)
        {
            lockedObject.SetActive(
                !unlocked);
        }

        ApplyColor(
            unlocked,
            cleared,
            selected);

        ApplySelectionVisual(
            selected);

        Button button =
            GetComponent<Button>();

        if (button == null)
        {
            button =
                GetComponentInChildren<Button>(
                    true);
        }

        if (button != null)
        {
            button.interactable =
                unlocked;

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(
                OnClick);
        }
    }

    public void OnClick()
    {
        if (!isUnlocked)
            return;

        Debug.Log(
            "Selected stage: " +
            (stageIndex + 1));

        owner.SelectStage(
            stageIndex);
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        OnClick();
    }

    private void ApplyColor(
        bool unlocked,
        bool cleared,
        bool selected)
    {
        if (backgroundImage == null)
            return;

        if (!unlocked)
        {
            backgroundImage.color =
                lockedColor;
        }
        else if (selected)
        {
            backgroundImage.color =
                selectedColor;
        }
        else if (cleared)
        {
            backgroundImage.color =
                clearedColor;
        }
        else
        {
            backgroundImage.color =
                unlockedColor;
        }
    }

    private void ApplySelectionVisual(
        bool selected)
    {
        Image targetImage =
            backgroundImage != null
            ? backgroundImage
            : GetComponentInChildren<Image>(
                true);

        if (selectionOutline == null)
        {
            selectionOutline =
                targetImage != null
                ? targetImage.GetComponent<Outline>()
                : gameObject.GetComponent<Outline>();

            if (selectionOutline == null)
            {
                selectionOutline =
                    targetImage != null
                    ? targetImage.gameObject
                        .AddComponent<Outline>()
                    : gameObject
                        .AddComponent<Outline>();
            }
        }

        selectionOutline.effectColor =
            Color.white;

        selectionOutline.effectDistance =
            new Vector2(4f, -4f);

        selectionOutline.enabled =
            selected;

        EnsureSelectionFrame();

        if (selectionFrame != null)
        {
            selectionFrame.SetActive(
                selected);
        }

        transform.localScale =
            selected
            ? originalScale * 0.9f
            : originalScale;

        if (backgroundImage != null &&
            selected)
        {
            backgroundImage.color =
                Color.Lerp(
                    backgroundImage.color,
                Color.black,
                    0.35f);
        }
    }

    private void AutoBindBackgroundImage()
    {
        if (backgroundImage != null)
            return;

        backgroundImage =
            GetComponent<Image>();

        if (backgroundImage != null)
            return;

        backgroundImage =
            GetComponentInChildren<Image>(
                true);
    }

    private void EnsureSelectionFrame()
    {
        if (selectionFrame != null)
            return;

        RectTransform root =
            transform as RectTransform;

        if (root == null)
            return;

        selectionFrame =
            new GameObject(
                "SelectedFrame",
                typeof(RectTransform));

        selectionFrame.transform
            .SetParent(
                transform,
                false);

        selectionFrame.transform
            .SetAsLastSibling();

        RectTransform frameRect =
            selectionFrame
            .GetComponent<RectTransform>();

        frameRect.anchorMin =
            Vector2.zero;

        frameRect.anchorMax =
            Vector2.one;

        frameRect.offsetMin =
            new Vector2(-8f, -8f);

        frameRect.offsetMax =
            new Vector2(8f, 8f);

        CreateBorder(
            "Top",
            frameRect,
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, -4f),
            Vector2.zero);

        CreateBorder(
            "Bottom",
            frameRect,
            Vector2.zero,
            new Vector2(1f, 0f),
            Vector2.zero,
            new Vector2(0f, 4f));

        CreateBorder(
            "Left",
            frameRect,
            Vector2.zero,
            new Vector2(0f, 1f),
            Vector2.zero,
            new Vector2(4f, 0f));

        CreateBorder(
            "Right",
            frameRect,
            new Vector2(1f, 0f),
            Vector2.one,
            new Vector2(-4f, 0f),
            Vector2.zero);

        selectionFrame.SetActive(false);
    }

    private void CreateBorder(
        string borderName,
        RectTransform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax)
    {
        GameObject border =
            new GameObject(
                borderName,
                typeof(RectTransform),
                typeof(Image));

        border.transform
            .SetParent(
                parent,
                false);

        RectTransform rect =
            border.GetComponent<RectTransform>();

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;

        Image image =
            border.GetComponent<Image>();

        image.color = Color.white;
        image.raycastTarget = false;
    }
}
