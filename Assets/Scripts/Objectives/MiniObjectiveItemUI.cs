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
    [SerializeField] private Color failedBackgroundColor = new Color(1f, 0.8f, 0.8f);
    [SerializeField] private Color activeStatusColor = Color.yellow;
    [SerializeField] private Color completedStatusColor = Color.green;
    [SerializeField] private Color failedStatusColor = Color.red;

    private bool isCompleted;

    public void Setup(string title, string description, float progress, ObjectiveState state)
    {
        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        UpdateProgress(progress);
        SetState(state);
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

    public void SetState(ObjectiveState state)
    {
        if (statusText != null)
        {
            {
                switch (state)
                {
                    case ObjectiveState.Active:
                        statusText.text = "Vykdoma";
                        statusText.color = activeStatusColor;
                        break;

                    case ObjectiveState.Completed:
                        statusText.text = "Ivykdyta";
                        statusText.color = completedStatusColor;
                        break;

                    case ObjectiveState.Failed:
                        statusText.text = "Neivykdyta";
                        statusText.color = failedStatusColor;
                        break;

                    default:
                        statusText.text = "Užrakinta";
                        statusText.color = Color.gray;
                        break;
                }
            }
        }

        if (backgroundImage != null)
        {
            switch (state)
            {
                case ObjectiveState.Completed:
                    backgroundImage.color = completedBackgroundColor;
                    break;

                case ObjectiveState.Failed:
                    backgroundImage.color = failedBackgroundColor;
                    break;

                default:
                    backgroundImage.color = normalBackgroundColor;
                    break;
            }
        }

        if (completedIcon != null)
            completedIcon.SetActive(state == ObjectiveState.Completed);
    }
}