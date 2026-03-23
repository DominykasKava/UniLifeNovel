using UnityEngine;
using System;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public CharacterDisplay characterDisplay;
    public PortraitController portraitController;
    public BackgroundLoader backgroundLoader;
    public DialogueUI dialogueUI;
    private DialogueLoader loader;
    private DialogueNode currentNode;
    public ChoiceUIManager choiceUI;
    public event Action<string> OnLineDisplayed;
    public event Action OnDialogueFinished;

    [Header("Dialogue file name (be .json)")]
    public string dialogueFile = "test_dialogue";

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

        if (currentNode.choices != null && currentNode.choices.Length > 0)
        {
            UpdateUI();
            return;
        }

        if (string.IsNullOrEmpty(currentNode.next))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Dialogas pasibaigė");
            return;
        }

        currentNode = loader.GetNode(currentNode.next);

        int hops = 0;
        while (currentNode != null && !string.IsNullOrEmpty(currentNode.jumpTo) && hops++ < MaxAutoJumps)
        {
            var targetId = currentNode.jumpTo;
            var target = loader.GetNode(targetId);
            if (target == null)
            {
                Debug.LogError($"DialogueManager: Jump tikslas nerastas: '{targetId}' (iš '{currentNode.id}')");
                break;
            }
            currentNode = target; 

            if (target.conditions != null && target.conditions.Length > 0)
            {
                bool ok = true;
                foreach (var condition in target.conditions )
                {
                    if (!condition.Evaluate())
                    {
                        ok = false;
                        break;
                    }
                }
                if (!ok)
                {
                    currentNode = string.IsNullOrEmpty(target.next) ? null : loader.GetNode(target.next);
                    continue;
                }
            }
        }

        if (hops >= MaxAutoJumps)
        {
            Debug.LogError("DialogueManager: per daug automatiniu Jump perėjimų (galimas begalinis ciklas).");
        }

        UpdateUI();
    }

    public DialogueNode GetCurrentNode() => currentNode;

    public void Choose(Choices choices)
    {
        if (choices == null) return;

        if (!string.IsNullOrEmpty(choices.callback))
        {
            HandleCallBack(choices.callback);
        }

        if (!string.IsNullOrEmpty(choices.next))
        {
            currentNode = loader.GetNode(choices.next);
        }
        else
        {
            return;
        }

        UpdateUI();
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

        if (choiceUI != null)
        {
            if (currentNode.choices != null && currentNode.choices.Length > 0)
            {
                choiceUI.ShowChoices(new List<Choices>(currentNode.choices));
            }
            else
            {
                choiceUI.ClearChoices();
            }
        }

        OnLineDisplayed?.Invoke("");
    }

    private void HandleCallBack(string callBack)
    {
        switch (callBack)
        {
            case "GainTrust":
                GameVariables.Instance.AddInt("trust", 10);
                Debug.Log("Trust +10");
                break;

            case "LoseTrust":
                GameVariables.Instance.AddInt("trust", -10);
                Debug.Log("Trust -10");
                break;
        }
    }
}