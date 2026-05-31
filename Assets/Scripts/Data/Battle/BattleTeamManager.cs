using System.Collections.Generic;
using UnityEngine;

public class BattleTeamManager : MonoBehaviour
{
    public static BattleTeamManager Instance;

    public List<HeroData> selectedHeroes =
        new List<HeroData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearTeam()
    {
        selectedHeroes.Clear();
    }

    public void AddHero(HeroData hero)
    {
        if (hero == null)
            return;

        if (selectedHeroes.Exists(
            selectedHero =>
                selectedHero != null &&
                selectedHero.heroID ==
                hero.heroID))
        {
            return;
        }

        if (selectedHeroes.Count >= 5)
            return;

        selectedHeroes.Add(hero);
    }

    public void RemoveHero(HeroData hero)
    {
        if (selectedHeroes.Contains(hero))
        {
            selectedHeroes.Remove(hero);
        }
    }

    public void SyncWithAvailableHeroes(
        List<HeroData> availableHeroes)
    {
        if (availableHeroes == null)
        {
            selectedHeroes.Clear();

            return;
        }

        for (int i = selectedHeroes.Count - 1;
            i >= 0;
            i--)
        {
            HeroData selectedHero =
                selectedHeroes[i];

            if (selectedHero == null ||
                string.IsNullOrEmpty(
                    selectedHero.heroID))
            {
                selectedHeroes.RemoveAt(i);

                continue;
            }

            HeroData freshHero =
                availableHeroes.Find(
                    hero =>
                        hero != null &&
                        hero.heroID ==
                        selectedHero.heroID);

            if (freshHero == null)
            {
                selectedHeroes.RemoveAt(i);

                continue;
            }

            selectedHeroes[i] =
                freshHero;
        }
    }

    public void SyncWithUnitManager()
    {
        if (UnitManager.Instance == null)
            return;

        SyncWithAvailableHeroes(
            UnitManager.Instance.activeHeroes);
    }
}
