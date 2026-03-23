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

        // jei nepriskirta Inspector'iuje – susirandame automatiškai
        if (uiController == null)
        {
#if UNITY_2023_1_OR_NEWER
            uiController = Object.FindFirstObjectByType<DialogueUIController>();
#else
            uiController = Object.FindObjectOfType<DialogueUIController>();
#endif
        }
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