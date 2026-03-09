using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
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
        if (currentNode == null || dialogueUI == null) return;

        Sprite portraitSprite = null;
        if (!string.IsNullOrEmpty(currentNode.portrait))
        {
            portraitSprite = Resources.Load<Sprite>("Portraits/" + currentNode.portrait);
        }
        dialogueUI.DisplayDialogue(currentNode.speaker, currentNode.text, portraitSprite);
    }
}