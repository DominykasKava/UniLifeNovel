using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI/visual refs (optional)")]
    public CharacterDisplay characterDisplay;
    public PortraitController portraitController;
    public BackgroundLoader backgroundLoader;
    public DialogueUI dialogueUI;
    public ChoiceUIManager choiceUI;

    private DialogueLoader loader;
    private DialogueNode currentNode;
    private readonly HashSet<string> executedNodeCallbacks = new();

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
        ResolveAutoJumps();
        UpdateUI();
    }

    public void Next()
    {
        if (currentNode == null)
            return;

        if ((currentNode.choices != null && currentNode.choices.Length > 0) ||
            (choiceUI != null && choiceUI.HasActiveChoices))
        {
            return;
        }

        if (string.IsNullOrEmpty(currentNode.next))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Dialogas pasibaigė");
            return;
        }

        DialogueNode nextNode = loader.GetNode(currentNode.next);

        if (!CheckCondition(nextNode))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Conditions nepasiektos");
            return;
        }

        currentNode = nextNode;

        ResolveAutoJumps();
        UpdateUI();
    }

    public void Choose(int index)
    {
        if (currentNode == null || currentNode.choices == null || currentNode.choices.Length == 0)
            return;

        if (index < 0 || index >= currentNode.choices.Length)
        {
            Debug.LogWarning("DialogueManager.Choose: neteisingas pasirinkimo indeksas.");
            return;
        }

        Choices choice = currentNode.choices[index];

        if (!string.IsNullOrEmpty(choice.callback))
        {
            HandleCallBack(choice.callback);
        }

        if (!string.IsNullOrWhiteSpace(choice.next))
        {
            GoTo(choice.next);
            ResolveAutoJumps();
            UpdateUI();
        }
    }

    public void GoTo(string nodeId)
    {
        DialogueNode node = loader.GetNode(nodeId);

        if (node == null)
        {
            Debug.LogError($"DialogueManager.GoTo: nerastas mazgas '{nodeId}'.");
            return;
        }

        currentNode = node;
    }

    public DialogueNode GetCurrentNode()
    {
        return currentNode;
    }

    private void ResolveAutoJumps()
    {
        int hops = 0;

        while (currentNode != null && hops++ < MaxAutoJumps)
        {
            if (!CheckCondition(currentNode))
            {
                Debug.Log("Conditions nepasiektos node");

                if (string.IsNullOrEmpty(currentNode.next))
                {
                    currentNode = null;
                    OnDialogueFinished?.Invoke();
                    return;
                }

                currentNode = loader.GetNode(currentNode.next);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(currentNode.jumpTo))
            {
                DialogueNode target = loader.GetNode(currentNode.jumpTo);

                if (target == null)
                {
                    Debug.LogError("Jump target nerastas: " + currentNode.jumpTo);
                    return;
                }

                currentNode = target;
                continue;
            }

            break;
        }

        if (hops >= MaxAutoJumps)
        {
            Debug.LogError("Per daug Jump, galimas loop");
        }
    }

    private void UpdateUI()
    {
        if (currentNode == null)
            return;

        if (!string.IsNullOrWhiteSpace(currentNode.callback) &&
            !executedNodeCallbacks.Contains(currentNode.id))
        {
            executedNodeCallbacks.Add(currentNode.id);
            HandleCallBack(currentNode.callback);
        }

        if (dialogueUI != null)
        {
            dialogueUI.DisplayDialogue(currentNode.speaker, currentNode.text, null);
        }

        OnLineDisplayed?.Invoke(currentNode.text ?? string.Empty);
        DialogueBacklog.AddLine(currentNode.text);

        if (characterDisplay != null)
        {
            characterDisplay.SetSpeakerName(currentNode.speaker);
        }

        if (portraitController != null && !string.IsNullOrEmpty(currentNode.portrait))
        {
            string[] parts = currentNode.portrait.Split('_');
            string characterName = parts[0];
            string expression = parts.Length > 1 ? parts[1] : "default";

            portraitController.SetPortrait(characterName, expression);
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

        ObjectiveTracker.Instance?.EvaluateObjectives();
    }

    private bool CheckCondition(DialogueNode node)
    {
        if (node == null)
            return false;

        if (node.conditions == null || node.conditions.Length == 0)
            return true;

        foreach (var condition in node.conditions)
        {
            if (!condition.Evaluate())
                return false;
        }

        return true;
    }

    private void HandleCallBack(string callBack)
    {
        if (string.IsNullOrWhiteSpace(callBack))
        {
            return;
        }

        string[] callBacks = callBack.Split('|');

        foreach (string cb in callBacks)
        {
            string trimmed = cb.Trim();

            if (trimmed.StartsWith("CHAPTER:"))
            {
                string data = trimmed.Replace("CHAPTER:", "").Trim();

                if (int.TryParse(data, out int chapterNumber))
                {
                    ChapterManager.Instance?.SetChapter(chapterNumber);
                    Debug.Log("Chapter pakeistas į: " + chapterNumber);
                }

                continue;
            }

            if (trimmed.StartsWith("OBJ:"))
            {
                string objectiveId = trimmed.Replace("OBJ:", "").Trim();
                ChapterManager.Instance?.CompleteObjective(objectiveId);
                Debug.Log("Objective atliktas: " + objectiveId);

                continue;
            }

            switch (trimmed)
            {
                case "GainTrust":
                    GameVariables.Instance.AddInt("trust", 10);
                    Debug.Log("Trust +10");
                    break;

                case "LoseTrust":
                    GameVariables.Instance.AddInt("trust", -10);
                    Debug.Log("Trust -10");
                    break;

                default:
                    HandleObjectiveAndVariableCallback(trimmed);
                    break;
            }

            ObjectiveTracker.Instance?.EvaluateObjectives();
        }
    }

    private void HandleObjectiveAndVariableCallback(string callBack)
    {
        if (callBack.StartsWith("AddInt:"))
        {
            string data = callBack.Replace("AddInt:", "").Trim();
            string[] parts = data.Split(':');

            if (parts.Length == 2 && int.TryParse(parts[1], out int amount))
            {
                string key = parts[0].Trim();
                GameVariables.Instance.AddInt(key, amount);
                Debug.Log($"{key} {(amount >= 0 ? "+" : "")}{amount}");
            }

            return;
        }

        if (callBack.StartsWith("SetInt:"))
        {
            string data = callBack.Replace("SetInt:", "").Trim();
            string[] parts = data.Split(':');

            if (parts.Length == 2 && int.TryParse(parts[1], out int value))
            {
                string key = parts[0].Trim();
                GameVariables.Instance.SetInt(key, value);
                Debug.Log($"{key} set to {value}");
            }

            return;
        }

        if (callBack.StartsWith("ActivateObjective:"))
        {
            string objectiveID = callBack.Replace("ActivateObjective:", "").Trim();
            ObjectiveTracker.Instance?.ActivateObjective(objectiveID);
            return;
        }

        if (callBack.StartsWith("CompleteObjective:"))
        {
            string objectiveID = callBack.Replace("CompleteObjective:", "").Trim();
            ObjectiveTracker.Instance?.CompleteObjective(objectiveID);
            return;
        }

        if (callBack.StartsWith("FailObjective:"))
        {
            string objectiveID = callBack.Replace("FailObjective:", "").Trim();
            ObjectiveTracker.Instance?.FailObjective(objectiveID);
            return;
        }

        if (callBack.StartsWith("SetObjectiveProgress:"))
        {
            string data = callBack.Replace("SetObjectiveProgress:", "").Trim();
            string[] parts = data.Split('|');

            if (parts.Length == 2 && float.TryParse(parts[1], out float progress))
            {
                string objectiveID = parts[0].Trim();
                ObjectiveTracker.Instance?.SetProgress(objectiveID, progress);
            }
        }
    }

    public void LoadFromFile(string fileName = "dialogue_save.json")
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning("Save failas nerastas: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null || string.IsNullOrEmpty(data.currentNodeID))
        {
            Debug.LogWarning("Neleistinas arba tuščias save failas.");
            return;
        }

        if (ChapterManager.Instance != null)
        {
            ChapterManager.Instance.SetChapter(data.chapter);
        }

        DialogueNode loadedNode = loader.GetNode(data.currentNodeID);

        if (loadedNode == null)
        {
            Debug.LogError("Dialogo mazgas nerastas pagal id iš save failo: " + data.currentNodeID);
            return;
        }

        currentNode = loadedNode;

        GameVariables.Instance.GetAllInts().Clear();
        GameVariables.Instance.GetAllBools().Clear();

        foreach (var varData in data.intVariables)
        {
            GameVariables.Instance.SetInt(varData.key, varData.value);
        }

        foreach (var varData in data.boolVariables)
        {
            GameVariables.Instance.SetBool(varData.key, varData.value);
        }

        UpdateUI();

        Debug.Log("Dialogas užkrautas nuo mazgo: " + data.currentNodeID);
    }

    public void SaveToFile(string fileName = "dialogue_save.json")
    {
        if (currentNode == null)
        {
            Debug.LogWarning("Nėra aktyvaus mazgo, kurį galima būtų išsaugoti.");
            return;
        }

        SaveData data = new SaveData();
        data.currentNodeID = currentNode.id;
        data.dialogueIndex = 0;

        if (ChapterManager.Instance != null)
        {
            data.chapter = ChapterManager.Instance.GetChapter();
        }

        foreach (var pair in GameVariables.Instance.GetAllInts())
        {
            data.intVariables.Add(new IntVariableData
            {
                key = pair.Key,
                value = pair.Value
            });
        }

        foreach (var pair in GameVariables.Instance.GetAllBools())
        {
            data.boolVariables.Add(new BoolVariableData
            {
                key = pair.Key,
                value = pair.Value
            });
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Dialogas išsaugotas į: " + path);
        Debug.Log("Save JSON: " + json);
    }

    public void QuickLoad()
    {
        LoadFromFile();
    }

    public SaveData CreateSaveData()
    {
        if (currentNode == null)
        {
            Debug.LogWarning("Nėra aktyvaus mazgo išsaugojimui.");
            return null;
        }

        SaveData data = new SaveData();
        data.currentNodeID = currentNode.id;
        data.dialogueIndex = 0;

        if (ChapterManager.Instance != null)
        {
            data.chapter = ChapterManager.Instance.GetChapter();
        }

        foreach (var pair in GameVariables.Instance.GetAllInts())
        {
            data.intVariables.Add(new IntVariableData
            {
                key = pair.Key,
                value = pair.Value
            });
        }

        foreach (var pair in GameVariables.Instance.GetAllBools())
        {
            data.boolVariables.Add(new BoolVariableData
            {
                key = pair.Key,
                value = pair.Value
            });
        }

        return data;
    }

    public void LoadFromSaveData(SaveData data)
    {
        if (data == null || string.IsNullOrEmpty(data.currentNodeID))
        {
            Debug.LogWarning("SaveData tuščias arba neteisingas.");
            return;
        }

        DialogueNode loadedNode = loader.GetNode(data.currentNodeID);

        if (loadedNode == null)
        {
            Debug.LogError("Mazgas nerastas pagal id: " + data.currentNodeID);
            return;
        }

        currentNode = loadedNode;

        if (ChapterManager.Instance != null)
        {
            ChapterManager.Instance.SetChapter(data.chapter);
        }

        GameVariables.Instance.GetAllInts().Clear();
        GameVariables.Instance.GetAllBools().Clear();

        foreach (var varData in data.intVariables)
        {
            GameVariables.Instance.SetInt(varData.key, varData.value);
        }

        foreach (var varData in data.boolVariables)
        {
            GameVariables.Instance.SetBool(varData.key, varData.value);
        }

        UpdateUI();

        Debug.Log("Dialogas užkrautas nuo mazgo: " + data.currentNodeID);
    }
}