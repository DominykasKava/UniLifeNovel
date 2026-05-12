using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // čia gali resetinti state jei turi singletonus
        GameManager.Instance.ResetGame();

        SceneLoader.Instance.LoadGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}