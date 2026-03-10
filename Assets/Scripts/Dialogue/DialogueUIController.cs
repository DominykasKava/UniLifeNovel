using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{

    [Header("References")]
    public DialogueManager dialogueManager;
    public TextMeshProUGUI dialogueText; // TMP tekstas dialogui
    public GameObject container;         // panelė ar UI blokas, kur yra tekstas (paslepiama)

    private void Awake()
    {
        // Jei container nenustatytas – bandome pasiimti parent objektą
        if (container == null && dialogueText != null)
            container = dialogueText.transform.parent.gameObject;
    }

    private void OnEnable()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueUIController: dialogueManager nepriskirtas!");
            return;
        }
        if (dialogueText == null)
        {
            Debug.LogError("DialogueUIController: dialogueText nepriskirtas!");
            return;
        }

        // Prisijungiame prie event'ų
        dialogueManager.OnLineDisplayed += UpdateDialogueText;
        dialogueManager.OnDialogueFinished += HideDialogueUI;
    }

    private void OnDisable()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnLineDisplayed -= UpdateDialogueText;
            dialogueManager.OnDialogueFinished -= HideDialogueUI;
        }
    }

    /// <summary>
    /// Atnaujina tekstą UI'e, kai DialogueManager parodo naują eilutę
    /// </summary>
    private void UpdateDialogueText(string line)
    {
        dialogueText.text = line;

        if (container != null)
            container.SetActive(true);
    }

    /// <summary>
    /// Paslepia dialogo UI, kai dialogas baigiasi
    /// </summary>
    private void HideDialogueUI()
    {
        if (container != null)
            container.SetActive(false);

        dialogueText.text = "";
    }
}