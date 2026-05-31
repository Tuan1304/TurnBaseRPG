using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderUI : MonoBehaviour
{
    public static TurnOrderUI Instance;

    public Transform container;

    public GameObject iconPrefab;

    private List<GameObject> icons =
        new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // REFRESH
    // =========================

    public void Refresh()
    {
        ClearIcons();

        CreateIcons(
            TurnManager.Instance
            .GetPlayerUnits());

        CreateIcons(
            TurnManager.Instance
            .GetEnemyUnits());
    }

    // =========================
    // CREATE ICONS
    // =========================

    private void CreateIcons(
        List<BattleUnit> units)
    {
        foreach (BattleUnit unit in units)
        {
            if (unit == null)
                continue;

            GameObject obj =
                Instantiate(
                    iconPrefab,
                    container);

            icons.Add(obj);

            Image img =
                obj.GetComponent<Image>();

            CanvasGroup group =
                obj.GetComponent<CanvasGroup>();

            // =====================
            // ICON
            // =====================

            if (unit.heroData != null)
            {
                img.sprite =
                    unit.heroData
                    .portrait;
            }
            else if (unit.enemyData != null)
            {
                img.sprite =
                    unit.visual.GetPortraitSprite();
            }

            // =====================
            // TURN STATE
            // =====================

            group.alpha =
                unit.hasAttacked
                ? 0.4f
                : 1f;
        }
    }

    // =========================
    // CLEAR
    // =========================

    private void ClearIcons()
    {
        foreach (GameObject obj
            in icons)
        {
            Destroy(obj);
        }

        icons.Clear();
    }
}