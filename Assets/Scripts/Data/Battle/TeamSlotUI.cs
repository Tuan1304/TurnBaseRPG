using UnityEngine;
using UnityEngine.UI;

public class TeamSlotUI : MonoBehaviour
{
    public Image portrait;
    public Image frame;

    public GameObject plusIcon;

    private HeroData heroData;

    public void SetHero(HeroData hero)
    {
        heroData = hero;

        portrait.sprite = hero.portrait;

        portrait.gameObject.SetActive(true);

        plusIcon.SetActive(false);

        SetupRarity(hero.unitData.rarity);
    }

    public void ClearSlot()
    {
        heroData = null;

        portrait.sprite = null;

        portrait.gameObject.SetActive(false);

        plusIcon.SetActive(true);
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

        frame.color = rarityColor;
    }

    public void OnClickSlot()
    {
        TeamSelectionUI teamUI =
            FindFirstObjectByType<TeamSelectionUI>();

        if (teamUI != null)
        {
            teamUI.OpenSelectPanel(this);
        }
    }
}