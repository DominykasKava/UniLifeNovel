using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ChoiceClickTrigger : MonoBehaviour
{
    [SerializeField] private DialogueUIController uiController;

    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
        if (uiController == null)
            uiController = Object.FindAnyObjectByType<DialogueUIController>();
    }

    private void OnEnable()
    {
        _btn.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        if (uiController != null)
            uiController.PlayChoiceSound();
    }
}
