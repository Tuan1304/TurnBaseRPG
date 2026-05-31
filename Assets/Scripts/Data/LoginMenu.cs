using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button continueButton;

    [Header("Scenes")]
    [SerializeField]
    private string homeSceneName = "HomeScene";

    private void Start()
    {
        RefreshContinueButton();
    }

    public void NewGame()
    {
        SaveSystem.DeleteSave();

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.NewGame();
        }
        else
        {
            SaveSystem.Save(
                GameDataManager.CreateNewGameData());
        }

        LoadHomeScene();
    }

    public void ContinueGame()
    {
        if (!SaveSystem.HasSave())
            return;

        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.LoadGame();
        }

        LoadHomeScene();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null)
            return;

        continueButton.interactable =
            SaveSystem.HasSave();
    }

    private void LoadHomeScene()
    {
        SceneManager.LoadScene(
            homeSceneName);
    }
}
