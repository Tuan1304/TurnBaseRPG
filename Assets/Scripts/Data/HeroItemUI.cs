using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI rarityText;

    public Image portraitImage;
    public bool disableDetailClick;

    public Image frame;
    public Image glow;

    private HeroData data;

    private Action<HeroData> clickAction;

    public void Setup(
        HeroData hero,
        Action<HeroData> onClick = null)
    {
        data = hero;

        clickAction = onClick;

        DisableChildRaycastTargets();

        Button button =
            GetComponent<Button>();

        if (button != null)
        {
            button.onClick.RemoveListener(
                OnClick);

            button.onClick.AddListener(
                OnClick);
        }

        nameText.text = hero.unitData.unitName;

        classText.text =
            hero.unitData.unitClass.ToString();

        rarityText.text =
            hero.unitData.rarity.ToString();

        if (portraitImage != null)
        {
            Sprite portrait =
                hero.portrait != null
                ? hero.portrait
                : hero.unitData.avatar;

            portraitImage.sprite =
                portrait;

            portraitImage.enabled = true;

            portraitImage.color =
                portrait != null
                ? Color.white
                : new Color(1f, 1f, 1f, 0f);
        }

        SetupRarity(hero.unitData.rarity);
    }

    private void DisableChildRaycastTargets()
    {
        Graphic[] graphics =
            GetComponentsInChildren<Graphic>(
                true);

        foreach (Graphic graphic
            in graphics)
        {
            graphic.raycastTarget = false;
        }

        Graphic rootGraphic =
            GetComponent<Graphic>();

        if (rootGraphic != null)
        {
            rootGraphic.raycastTarget = true;
        }
    }

    private void SetupRarity(Rarity rarity)
    {
        Color rarityColor = Color.white;

        switch (rarity)
        {
            case Rarity.Common:
                rarityColor = Color.gray;
                break;

            case Rarity.Rare:
                rarityColor = Color.blue;
                break;

            case Rarity.Epic:
                rarityColor =
                    new Color(0.6f, 0.2f, 1f);
                break;

            case Rarity.Legendary:
                rarityColor =
                    new Color(1f, 0.7f, 0f);
                break;
        }

        rarityText.color = rarityColor;

        frame.color = rarityColor;

        glow.color =
            new Color(
                rarityColor.r,
                rarityColor.g,
                rarityColor.b,
                0.3f);
    }


    public void OnClick()
    {
        if (disableDetailClick)
            return;

        if (clickAction != null)
        {
            clickAction.Invoke(data);

            return;
        }

        UIManager.Instance.OpenDetail(data);
    }

}
