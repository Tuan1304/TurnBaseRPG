using UnityEngine;
using UnityEngine.UI;

public class TeamSelectionUI : MonoBehaviour
{
    public TeamSlotUI[] slots;

    [Header("Select Hero UI")]
    public GameObject selectHeroPanel;

    public Transform heroListContent;

    public HeroItemUI heroItemPrefab;

    private TeamSlotUI currentSlot;


    private void Start()
    {
        RefreshUI();
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (BattleTeamManager.Instance != null)
        {
            BattleTeamManager.Instance
                .SyncWithUnitManager();
        }
        else
        {
            return;
        }

        int selectedCount =
            BattleTeamManager.Instance.selectedHeroes.Count;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < selectedCount)
            {
                slots[i].gameObject.SetActive(true);

                HeroData hero =
                    BattleTeamManager.Instance.selectedHeroes[i];

                slots[i].SetHero(hero);
            }
            else if (i == selectedCount)
            {
                slots[i].gameObject.SetActive(true);

                slots[i].ClearSlot();
            }
            else
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenSelectPanel(TeamSlotUI slot)
    {
        currentSlot = slot;

        selectHeroPanel.SetActive(true);

        RefreshHeroList();
    }

    private void RefreshHeroList()
    {
        if (UnitManager.Instance == null ||
            BattleTeamManager.Instance == null ||
            heroListContent == null ||
            heroItemPrefab == null)
        {
            return;
        }

        BattleTeamManager.Instance
            .SyncWithUnitManager();

        foreach (Transform child in heroListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (HeroData hero
            in UnitManager.Instance.activeHeroes)
        {
            HeroItemUI item =
                Instantiate(heroItemPrefab, heroListContent);

            item.Setup(hero);
            item.disableDetailClick = true;

            Button btn =
                item.GetComponent<Button>();

            btn.onClick.RemoveAllListeners();

            btn.onClick.AddListener(() =>
            {
                SelectHero(hero);
            });
        }
    }

    private void SelectHero(HeroData hero)
    {
        HeroData freshHero =
            GetFreshHero(hero);

        BattleTeamManager.Instance
            .AddHero(freshHero);

        RefreshUI();

        selectHeroPanel.SetActive(false);
    }

    private HeroData GetFreshHero(
        HeroData hero)
    {
        if (hero == null ||
            UnitManager.Instance == null)
        {
            return hero;
        }

        HeroData freshHero =
            UnitManager.Instance.activeHeroes.Find(
                activeHero =>
                    activeHero != null &&
                    activeHero.heroID ==
                    hero.heroID);

        return freshHero != null
            ? freshHero
            : hero;
    }
}
