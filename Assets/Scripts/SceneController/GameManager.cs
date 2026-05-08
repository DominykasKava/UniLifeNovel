using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentChapter = 1;

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

    public void ResetGame()
    {
        currentChapter = 1;

        // čia gali resetinti:
        // dialogo manager
        // objectives
        // variables
    }

    public void GoToMainMenu()
    {
        SceneLoader.Instance.LoadMainMenu();
    }
}