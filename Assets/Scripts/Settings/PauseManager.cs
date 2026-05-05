using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public static PauseManager Instance { get; private set; }

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }


    public void TogglePause()
    {
        SetPause(!IsPaused);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();

            // Parodom arba paslepiam PauseMenu
            PauseMenuUI menu = Object.FindObjectOfType<PauseMenuUI>();
            if (menu != null)
            {
                if (IsPaused) menu.Show();
                else menu.Hide();
            }
        }
    }
}
