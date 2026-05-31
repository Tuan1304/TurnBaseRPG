using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public GameData Data;

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DetachSceneManagers();

        DontDestroyOnLoad(gameObject);

        LoadGame();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void DetachSceneManagers()
    {
        while (transform.childCount > 0)
        {
            transform.GetChild(0)
                .SetParent(null);
        }
    }

    // =========================
    // LOAD
    // =========================

    public void LoadGame()
    {
        Data = SaveSystem.Load();

        if (Data == null)
        {
            NewGame();
        }
    }

    // =========================
    // NEW GAME
    // =========================

    public void NewGame()
    {
        Data = CreateNewGameData();

        SaveGame();
    }

    public static GameData CreateNewGameData()
    {
        GameData data =
            new GameData();

        data.gold = 1000;

        data.wood = 30;

        data.stone = 30;

        data.food = 30;

        data.currentStage = 1;

        return data;
    }

    // =========================
    // SAVE
    // =========================

    public void SaveGame()
    {
        SaveSystem.Save(Data);
    }
}
