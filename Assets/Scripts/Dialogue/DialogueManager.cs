using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public CharacterDisplay characterDisplay;
    public PortraitController portraitController;
    public BackgroundLoader backgroundLoader;
    public DialogueUI dialogueUI;
    private DialogueLoader loader;
    private DialogueNode currentNode;
    public event Action<string> OnLineDisplayed;
    public event Action OnDialogueFinished;

    [Header("Dialogue file name (be .json)")]
    public string dialogueFile = "test_dialogue";

    //NAUJA: apsauga nuo nekontroliuojamo Jump ciklo
    private const int MaxAutoJumps = 32;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        loader = new DialogueLoader();
        loader.Load(dialogueFile);
    }

    public void StartDialogue(string startID)
    {
        currentNode = loader.GetNode(startID);
        UpdateUI();
    }

    public void Next()
    {
        if (currentNode == null) return;

        // Jei dabartinis mazgas neturi "next" – dialogas baigtas
        if (string.IsNullOrEmpty(currentNode.next))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Dialogas pasibaigė");
            return;
        }

        // 1 žingsnis į "next"
        currentNode = loader.GetNode(currentNode.next);

        // Auto-sekimas per visus Jump mazgus:
        int hops = 0;
        while (currentNode != null &&
               !string.IsNullOrEmpty(currentNode.jumpTo) &&
               hops++ < MaxAutoJumps)
        {
            var targetId = currentNode.jumpTo;
            var target = loader.GetNode(targetId);
            if (target == null)
            {
                Debug.LogError($"DialogueManager: Jump tikslas nerastas: '{targetId}' (iš '{currentNode.id}')");
                break;
            }
            currentNode = target; // persokam
            // tęsiam ciklą — jei dar vienas Jump, šoksime toliau
        }

        if (hops >= MaxAutoJumps)
        {
            Debug.LogError("DialogueManager: per daug automatiných Jump perėjimų (galimas begalinis ciklas).");
        }

        UpdateUI();
    }

    public DialogueNode GetCurrentNode() => currentNode;

    private void UpdateUI()
    {
        if (currentNode == null) return;

        if (dialogueUI != null)
        {
            // Pastaba: vėliau, kai integruosi "choices", čia perduosi juos trečiu parametru vietoje null
            dialogueUI.DisplayDialogue(currentNode.speaker, currentNode.text, null);
        }

        if (characterDisplay != null)
        {
            characterDisplay.SetSpeakerName(currentNode.speaker);
        }

        if (portraitController != null && !string.IsNullOrEmpty(currentNode.portrait))
        {
            string[] parts = currentNode.portrait.Split('_');
            string characterName = parts[0];
            string expressions = parts.Length > 1 ? parts[1] : "default";
            portraitController.SetPortrait(characterName, expressions);
        }

        if (backgroundLoader != null && !string.IsNullOrEmpty(currentNode.background))
        {
            backgroundLoader.SetBackground(currentNode.background);
        }

        OnLineDisplayed?.Invoke("");
    }
}