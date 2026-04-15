using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniObjectiveItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Slider progressBar;

    public void Setup(string title, string description, string status)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (statusText != null)
            statusText.text = status;

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }
}