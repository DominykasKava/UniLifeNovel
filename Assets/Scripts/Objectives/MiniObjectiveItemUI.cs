using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniObjectiveItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Slider progressBar;

    public void Setup(string title, string description, int currentValue, int targetValue, bool isCompleted)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (statusText != null)
            statusText.text = isCompleted ? "Įvykdyta" : "Vykdoma";

        if (progressText != null)
            progressText.text = currentValue + "/" + targetValue;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.interactable = false;
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;

            float progress = 0f;

            if (targetValue > 0)
                progress = Mathf.Clamp01((float)currentValue / targetValue);

            progressBar.value = progress;
        }
    }
}