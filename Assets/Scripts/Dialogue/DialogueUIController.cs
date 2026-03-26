using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogueUIController : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public GameObject container;         // panelė ar UI blokas, kur yra tekstas (paslepiama

    [Header("Choice Sound")]
    [SerializeField] private AudioSource sfxSource;          // <- AudioSource (išjunk PlayOnAwake)
    [SerializeField] private AudioClip choiceClickSound;     // <- click garsas

    private readonly List<Button> _spawnedButtons = new();

    private void Awake()
    {
        // Jei container nenustatytas – bandome pasiimti parent objektą
        if (container == null)
            container = gameObject;

        // Jei sfxSource nenurodytas – bandome paimti iš šio GO
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

        // Prisijungiame prie event'ų
        dialogueManager.OnDialogueFinished += HideDialogueUI;
    }

    private void OnDisable()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueFinished -= HideDialogueUI;
        }
    }

    /// <summary>
    /// Paslepia dialogo UI, kai dialogas baigiasi
    /// </summary>
    private void HideDialogueUI()
    {
        if (container != null)
            container.SetActive(false);
    }

    // -------------------------
    //  PASIRINKIMO GARSAS
    // -------------------------

    public void PlayChoiceSound()
    {
        if (sfxSource != null && choiceClickSound != null)
        {
            sfxSource.PlayOneShot(choiceClickSound);
        }
        else
        {
            Debug.LogWarning("DialogueUIController: Pasirinkimo garsas neįkeltas / nėra AudioSource.");
        }
    }
}