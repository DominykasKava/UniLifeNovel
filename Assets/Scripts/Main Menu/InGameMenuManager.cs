using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject saveLoadPanel;

    public GameObject dialogueBox;
    public GameObject choicesPanel;
    public GameObject openSaveMenuButton;
    public GameObject openLoadMenuButton;
    public GameObject miniObjectivesWindow;

    public void StartGameUI()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        saveLoadPanel.SetActive(false);

        dialogueBox.SetActive(true);
        choicesPanel.SetActive(true);
        openSaveMenuButton.SetActive(true);
        openLoadMenuButton.SetActive(true);

        if (miniObjectivesWindow != null)
            miniObjectivesWindow.SetActive(true);
    }

    public void OpenLoadPanel()
    {
        mainMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(true);
    }

    public void CloseLoadPanel()
    {
        saveLoadPanel.SetActive(false);

        if (!dialogueBox.activeSelf)
            mainMenuPanel.SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button pressed");
        Application.Quit();
    }
}