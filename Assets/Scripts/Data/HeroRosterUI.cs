using UnityEngine;

public class HeroRosterUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject rosterPanel;

    [Header("List")]
    public Transform content;

    public HeroItemUI heroItemPrefab;

    private void OnEnable()
    {
        Refresh();
    }

    public void Open()
    {
        if (rosterPanel != null)
        {
            rosterPanel.SetActive(true);
        }

        Refresh();
    }

    public void Close()
    {
        if (rosterPanel != null)
        {
            rosterPanel.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (content == null ||
            heroItemPrefab == null ||
            UnitManager.Instance == null)
        {
            return;
        }

        foreach (Transform child
            in content)
        {
            Destroy(child.gameObject);
        }

        foreach (HeroData hero
            in UnitManager.Instance.activeHeroes)
        {
            HeroItemUI item =
                Instantiate(
                    heroItemPrefab,
                    content);

            item.disableDetailClick = false;

            item.Setup(
                hero,
                OpenHeroInfo);
        }
    }

    private void OpenHeroInfo(
        HeroData hero)
    {
        if (UIManager.Instance == null)
            return;

        Close();

        UIManager.Instance
            .OpenOwnedHeroDetail(hero);
    }
}
