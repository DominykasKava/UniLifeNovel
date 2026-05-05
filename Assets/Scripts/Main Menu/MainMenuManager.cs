using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public string gameSceneName = "GameScene";

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
        StartCoroutine(LoadAfterScene());
    }

    private IEnumerator LoadAfterScene()
    {
        yield return null;
        DialogueManager.Instance.LoadFromFile();
    }

    public void OpenSettings(GameObject settingsPanel)
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings(GameObject settingsPanel)
    {
        settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
