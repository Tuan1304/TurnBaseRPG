using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    // =========================
    // ACTION MENU
    // =========================

    [Header("Action Menu")]
    public GameObject actionMenuPanel;

    // =========================
    // SKILL PANEL
    // =========================

    [Header("Skill Panel")]
    public GameObject skillPanel;

    // =========================
    // BUTTONS
    // =========================

    [Header("Buttons")]
    public SkillButtonUI basicAttackButton;

    public SkillButtonUI skillButton;

    [Header("Action Buttons")]
    public GameObject attackButtonObject;

    // =========================
    // TURN UI
    // =========================

    [Header("Turn UI")]
    public GameObject turnPanel;

    public TextMeshProUGUI turnText;

    // =========================
    // WAVE UI
    // =========================

    [Header("Wave UI")]
    // public GameObject wavePanel;
    public GameObject wavePanel;

    public TextMeshProUGUI waveText;

    // =========================
    // BATTLE END
    // =========================

    [Header("Battle End")]
    public GameObject battleEndPanel;

    [Header("Result Buttons")]
    public GameObject nextStageButtonObject;

    // =========================
    // REWARD UI
    // =========================

    [Header("Reward UI")]
    public GameObject rewardPanel;

    public TextMeshProUGUI goldText;

    public TextMeshProUGUI woodText;

    public TextMeshProUGUI stoneText;

    public TextMeshProUGUI foodText;

    public TextMeshProUGUI expText;

    public TextMeshProUGUI resultText;

    public TextMeshProUGUI rewardText;

    private bool rewardClaimed;

    private bool isVictoryResult;

    private bool stageCompleted;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        actionMenuPanel.SetActive(false);

        skillPanel.SetActive(false);

        rewardPanel.SetActive(false);

        battleEndPanel.SetActive(false);

        turnPanel.SetActive(false);

        wavePanel.SetActive(false);
    }

    // =========================
    // ACTION MENU
    // =========================

    public void ShowActionMenu(
        bool value,
        bool canAttack = true)
    {
        actionMenuPanel.SetActive(value);

        attackButtonObject.SetActive(
            canAttack);
    }

    // =========================
    // SKILL PANEL
    // =========================

    public void ShowSkillPanel(
        bool value)
    {
        skillPanel.SetActive(value);
    }

    public void SetupSkillPanel(
        BattleUnit unit)
    {
        if (unit == null)
            return;

        // basic attack
        basicAttackButton
            .SetupBasicAttack(unit);

        // skill
        skillButton
            .SetupSkill(unit);
    }

    // =========================
    // TURN TEXT
    // =========================

    public IEnumerator ShowTurnRoutine(
        string text)
    {
        BattleManager.Instance
            .SetBusy(true);

        turnPanel.SetActive(true);

        turnText.text = text;

        yield return new WaitForSeconds(1.5f);

        turnPanel.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        BattleManager.Instance
            .SetBusy(false);
    }

    // =========================
    // WAVE TEXT
    // =========================

    public IEnumerator ShowWaveRoutine(int wave)
    {
        BattleManager.Instance
            .SetBusy(true);

        wavePanel.SetActive(true);

        waveText.text =
            "WAVE " + wave;

        yield return new WaitForSeconds(2f);

        wavePanel.SetActive(false);

        yield return new WaitForSeconds(0.2f);

        BattleManager.Instance
            .SetBusy(false);
    }

    // =========================
    // BATTLE RESULT
    // =========================

    public void ShowBattleResult(
        string result)
    {
        battleEndPanel.SetActive(true);

        SetResultText(result);
    }

    private void SetResultText(
        string result)
    {
        if (resultText != null)
        {
            resultText.text = result;
        }

        if (battleEndPanel == null)
            return;

        TextMeshProUGUI[] texts =
            battleEndPanel.GetComponentsInChildren
                <TextMeshProUGUI>(
                    true);

        foreach (TextMeshProUGUI text
            in texts)
        {
            if (text == null)
                continue;

            if (text.name == "EndText" ||
                text.name == "ResultText")
            {
                text.text = result;
            }
        }
    }

    public void ShowVictoryResult(
        StageReward reward)
    {
        rewardClaimed = false;

        isVictoryResult = true;

        stageCompleted = false;

        ShowBattleResult("VICTORY");

        ShowReward(reward);

        if (nextStageButtonObject != null)
        {
            nextStageButtonObject
                .SetActive(true);
        }
    }

    public void ShowDefeatResult()
    {
        rewardClaimed = true;

        isVictoryResult = false;

        stageCompleted = true;

        ShowBattleResult("DEFEAT");

        rewardPanel.SetActive(true);

        goldText.text = "0";

        woodText.text = "0";

        stoneText.text = "0";

        foodText.text = "0";

        expText.text = "0";

        if (nextStageButtonObject != null)
        {
            nextStageButtonObject
                .SetActive(false);
        }
    }

    // =========================
    // REWARD
    // =========================

    public void ShowReward(
        StageReward reward)
    {
        rewardPanel.SetActive(true);

        goldText.text =
            reward.gold.ToString();

        woodText.text =
            reward.wood.ToString();

        stoneText.text =
            reward.stone.ToString();

        foodText.text =
            reward.food.ToString();

        expText.text =
            reward.exp.ToString();
    }

    // =========================
    // RESULT BUTTONS
    // =========================

    public void OnClickConfirmResult()
    {
        ClaimRewardIfNeeded();

        CompleteStageIfNeeded();

        BattleStageSelection.ShowAdventureAfterReturn =
            false;

        ClearBattleTeamSelection();

        SceneManager.LoadScene(
            "HomeScene");
    }

    public void OnClickNextStage()
    {
        ClaimRewardIfNeeded();

        CompleteStageIfNeeded();

        BattleStageSelection.ShowAdventureAfterReturn =
            true;

        ClearBattleTeamSelection();

        SceneManager.LoadScene(
            "HomeScene");
    }

    private void ClearBattleTeamSelection()
    {
        if (BattleTeamManager.Instance == null)
            return;

        BattleTeamManager.Instance
            .ClearTeam();
    }

    private void CompleteStageIfNeeded()
    {
        if (!isVictoryResult)
            return;

        if (stageCompleted)
            return;

        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.Data != null)
        {
            int nextUnlockedStage =
                BattleStageSelection.SelectedStageIndex >= 0
                ? BattleStageSelection.SelectedStageIndex + 2
                : GameDataManager.Instance.Data.currentStage + 1;

            GameDataManager.Instance.Data.currentStage =
                Mathf.Max(
                    GameDataManager.Instance.Data.currentStage,
                    nextUnlockedStage);

            GameDataManager.Instance
                .SaveGame();
        }

        stageCompleted = true;
    }

    private void ClaimRewardIfNeeded()
    {
        if (!isVictoryResult)
            return;

        if (rewardClaimed)
            return;

        StageData stage =
            BattleManager.Instance
            .currentStage;

        if (stage != null &&
            GameDataManager.Instance != null &&
            GameDataManager.Instance.Data != null)
        {
            GameDataManager
                .Instance
                .Data
                .gold += stage.reward.gold;

            GameDataManager
                .Instance
                .Data
                .wood += stage.reward.wood;

            GameDataManager
                .Instance
                .Data
                .stone += stage.reward.stone;

            GameDataManager
                .Instance
                .Data
                .food += stage.reward.food;

            AddExpToSelectedHeroes(
                stage.reward.exp);

            GameDataManager
                .Instance
                .SaveGame();
        }

        rewardClaimed = true;
    }

    private void AddExpToSelectedHeroes(
        int totalExp)
    {
        if (totalExp <= 0 ||
            BattleTeamManager.Instance == null ||
            BattleTeamManager.Instance.selectedHeroes == null ||
            BattleTeamManager.Instance.selectedHeroes.Count == 0 ||
            GameDataManager.Instance == null ||
            GameDataManager.Instance.Data == null ||
            GameDataManager.Instance.Data.heroes == null)
        {
            return;
        }

        int expPerHero =
            Mathf.Max(
                1,
                totalExp /
                BattleTeamManager
                .Instance
                .selectedHeroes
                .Count);

        foreach (HeroData hero
            in BattleTeamManager.Instance.selectedHeroes)
        {
            if (hero == null)
                continue;

            HeroLevelUtility.AddExp(
                hero,
                expPerHero);

            SaveLeveledHero(
                hero);
        }
    }

    private void SaveLeveledHero(
        HeroData hero)
    {
        if (hero == null ||
            string.IsNullOrEmpty(hero.heroID))
        {
            return;
        }

        for (int i = 0;
            i < GameDataManager.Instance.Data.heroes.Count;
            i++)
        {
            HeroSaveData savedHero =
                GameDataManager.Instance.Data.heroes[i];

            if (savedHero == null ||
                savedHero.heroID != hero.heroID)
            {
                continue;
            }

            GameDataManager.Instance.Data.heroes[i] =
                HeroSerializer.ToSaveData(hero);

            return;
        }
    }


}
