using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdventureUI : MonoBehaviour
{
    [Header("Stages")]
    public List<StageData> stages =
        new List<StageData>();

    public int selectedStageIndex;

    [Header("Stage List UI")]
    public Transform stageListContent;

    public StageItemUI stageItemPrefab;

    private void Start()
    {
        SelectDefaultStage();

        RefreshStageList();
    }

    private void SelectDefaultStage()
    {
        if (stages.Count == 0)
            return;

        int unlockedStage =
            GetUnlockedStage();

        int index =
            Mathf.Clamp(
                unlockedStage - 1,
                0,
                stages.Count - 1);

        SelectStage(index);
    }

    public void SelectStage(
        int index)
    {
        if (index < 0 ||
            index >= stages.Count)
        {
            Debug.LogWarning(
                "Invalid stage index: "
                + index);

            return;
        }

        if (!IsStageUnlocked(index))
        {
            Debug.Log(
                "Stage locked: "
                + (index + 1));

            return;
        }

        selectedStageIndex = index;

        BattleStageSelection.SelectStage(
            stages[selectedStageIndex],
            selectedStageIndex);

        RefreshStageList();
    }

    public void EnterBattle()
    {
        if (UnitManager.Instance != null)
        {
            UnitManager.Instance.LoadHeroes();
        }

        if (BattleTeamManager.Instance != null)
        {
            BattleTeamManager.Instance
                .SyncWithUnitManager();
        }

        if (BattleTeamManager.Instance.selectedHeroes.Count == 0)
        {
            Debug.Log("Chua chon hero!");
            return;
        }

        if (stages.Count == 0)
        {
            Debug.LogWarning(
                "No stages assigned in AdventureUI!");

            return;
        }

        if (!IsStageUnlocked(selectedStageIndex))
        {
            Debug.Log(
                "Selected stage is locked!");

            return;
        }

        BattleStageSelection.SelectStage(
            stages[selectedStageIndex],
            selectedStageIndex);

        SceneManager.LoadScene(
            "BattleScene");
    }

    private bool IsStageUnlocked(
        int index)
    {
        return index <
            GetUnlockedStage();
    }

    private bool IsStageCleared(
        int index)
    {
        return index <
            GetUnlockedStage() - 1;
    }

    public void RefreshStageList()
    {
        if (stageListContent == null ||
            stageItemPrefab == null)
        {
            return;
        }

        bool prefabIsTemplateInContent =
            stageItemPrefab.transform.parent ==
            stageListContent;

        foreach (Transform child
            in stageListContent)
        {
            if (prefabIsTemplateInContent &&
                child == stageItemPrefab.transform)
            {
                continue;
            }

            Destroy(child.gameObject);
        }

        int visibleStageCount =
            Mathf.Min(
                GetUnlockedStage(),
                stages.Count);

        for (int i = 0;
            i < visibleStageCount;
            i++)
        {
            StageItemUI item =
                Instantiate(
                    stageItemPrefab,
                    stageListContent);

            item.gameObject.SetActive(true);

            item.Setup(
                this,
                stages[i],
                i,
                IsStageUnlocked(i),
                IsStageCleared(i),
                i == selectedStageIndex);
        }

        if (prefabIsTemplateInContent)
        {
            stageItemPrefab
                .gameObject
                .SetActive(false);
        }
    }

    private int GetUnlockedStage()
    {
        if (GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null)
        {
            return 1;
        }

        return Mathf.Max(
            1,
            GameDataManager.Instance
            .Data
            .currentStage);
    }
}
