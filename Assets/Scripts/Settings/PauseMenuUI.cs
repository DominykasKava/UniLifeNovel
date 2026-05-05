using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenuRoot;

    public void Show()
    {
        pauseMenuRoot.SetActive(true);
    }

    public void Hide()
    {
        pauseMenuRoot.SetActive(false);
    }

}
