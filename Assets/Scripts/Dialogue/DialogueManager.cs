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

    [Header("Dialogue file name (be .json)")]
    public string dialogueFile = "test_dialogue";

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
        if (currentNode == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(currentNode.next))
        {
            Debug.Log("Dialogas pasibaigė");
            return;
        }

        currentNode = loader.GetNode(currentNode.next);
        UpdateUI();
    }

    public DialogueNode GetCurrentNode()
    {
        return currentNode;
    }

    private void UpdateUI()
    {
        if (currentNode == null) return;

        if (dialogueUI != null)
        {
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
    }
}