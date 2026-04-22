using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniObjectiveItemUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text progressText;

    [Header("Progress")]
    [SerializeField] private Slider progressBar;

    [Header("Visual feedback")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject completedIcon;

    [Header("Colors")]
    [SerializeField] private Color normalBackgroundColor = Color.white;
    [SerializeField] private Color completedBackgroundColor = new Color(0.8f, 1f, 0.8f);
    [SerializeField] private Color activeStatusColor = Color.yellow;
    [SerializeField] private Color completedStatusColor = Color.green;

    private bool isCompleted;

    public void Setup(string title, string description, float progress, bool completed)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        UpdateProgress(progress);
        SetCompletedState(completed);
    }

    public void UpdateProgress(float progress)
    {
        float clamped = Mathf.Clamp01(progress);

        if (progressText != null)
            progressText.text = Mathf.RoundToInt(clamped * 100f) + "%";

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.interactable = false;
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = clamped;
        }
    }

    public void MarkCompleted()
    {
        isCompleted = true;
        UpdateProgress(1f);
        SetCompletedState(true);
    }

    private void SetCompletedState(bool completed)
    {
        isCompleted = completed;

        if (statusText != null)
        {
            statusText.text = isCompleted ? "Įvykdyta" : "Vykdoma";
            statusText.color = isCompleted ? completedStatusColor : activeStatusColor;
        }

        if (backgroundImage != null)
            backgroundImage.color = isCompleted ? completedBackgroundColor : normalBackgroundColor;

        if (completedIcon != null)
            completedIcon.SetActive(isCompleted);
    }
}