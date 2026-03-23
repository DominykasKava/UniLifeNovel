using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUIController : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public TextMeshProUGUI dialogueText; // TMP tekstas dialogui
    public GameObject container;         // panelė ar UI blokas, kur yra tekstas (paslepiama)

    [Header("Choice UI")]
    [SerializeField] private Transform choicesContainer;     // TU: priskirk tuščią VerticalLayout (ar pan.)
    [SerializeField] private Button choiceButtonPrefab;      // TU: priskirk mygtuko prefab’ą su TMP_Text vaikui

    [Header("Choice Sound")]
    [SerializeField] private AudioSource sfxSource;          // <- AudioSource (išjunk PlayOnAwake)
    [SerializeField] private AudioClip choiceClickSound;     // <- click garsas

    private readonly List<Button> _spawnedButtons = new();

    private void Awake()
    {
        // Jei container nenustatytas – bandome pasiimti parent objektą
        if (container == null && dialogueText != null)
            container = dialogueText.transform.parent.gameObject;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
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
        if (dialogueText != null)
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

        if (dialogueText != null)
            dialogueText.text = "";

        ClearChoices();
    }

    // -------------------------
    //  PASIRINKIMŲ UI
    // -------------------------

    public void ShowChoices(Choices[] choices)
    {
        ClearChoices();

        if (choicesContainer == null || choiceButtonPrefab == null)
        {
            Debug.LogWarning("DialogueUIController: choicesContainer arba choiceButtonPrefab nepriskirti.");
            return;
        }

        if (choices == null || choices.Length == 0) return;

        for (int i = 0; i < choices.Length; i++)
        {
            var data = choices[i];
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            _spawnedButtons.Add(btn);

            // surandam TMP_Text mygtuke (gali būti ant to pačio arba vaikui)
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = data.text;

            int capturedIndex = i;
            btn.onClick.AddListener(() => OnChoiceClicked(capturedIndex));
        }

        // Jei turi atskirą „choices panel“ – įjunk ją čia:
        if (!choicesContainer.gameObject.activeSelf)
            choicesContainer.gameObject.SetActive(true);
    }

    public void ClearChoices()
    {
        if (choicesContainer == null) return;

        foreach (var b in _spawnedButtons)
            if (b != null) Destroy(b.gameObject);

        _spawnedButtons.Clear();

        // Jei turi atskirą panelę ir ją slepi – padaryk:
        // choicesContainer.gameObject.SetActive(false);
    }

    public void OnChoiceClicked(int index)
    {
        PlayChoiceSound();
        DialogueManager.Instance.Choose(index);
    }

    public void PlayChoiceSound()
    {
        if (sfxSource != null && choiceClickSound != null)
            sfxSource.PlayOneShot(choiceClickSound);
    }
}