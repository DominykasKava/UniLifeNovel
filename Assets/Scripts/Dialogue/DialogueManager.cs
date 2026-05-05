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

    /// <summary> Iškviečiama, kai reikia atnaujinti „antrinį“ UI tekstą (jei toks naudojamas). </summary>
    public event Action<string> OnLineDisplayed;

    /// <summary> Iškviečiama, kai dialogo eiga pasibaigia. </summary>
    public event Action OnDialogueFinished;

    [Header("Dialogue file name (be .json)")]
    public string dialogueFile = "test_dialogue";

    // Apsauga nuo begalinių Jump/Goto kilpų vieno perėjimo metu
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

    /// <summary> Pradeda dialogą nuo nurodyto mazgo ID. </summary>
    public void StartDialogue(string startID)
    {
        currentNode = loader.GetNode(startID);

        // Jei startinis mazgas yra Jump – automatiškai peršokame į tikslą
        ResolveAutoJumps();

        UpdateUI();
    }

    /// <summary>
    /// Pereina prie kito mazgo pagal „next“. Jei naujasis mazgas yra „Jump“,
    /// automatiškai „praslystame“ per visus jump’us ir sustojame ties „normaliu“ mazgu.
    /// </summary>
    public void Next()
    {
        if (currentNode == null)
            return;

        if ((currentNode.choices != null && currentNode.choices.Length > 0) ||
    (choiceUI != null && choiceUI.HasActiveChoices))
        {
            return;
        }

        // Jei nėra „next“ – dialogas pasibaigė
        if (string.IsNullOrEmpty(currentNode.next))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Dialogas pasibaigė");
            return;
        }

        var nextNode = loader.GetNode(currentNode.next);

        if (!CheckCondition(nextNode))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Conditions nepasiektos");
            return;
        }

        // Žingsnis į „next“
        currentNode = loader.GetNode(currentNode.next);

        // Auto-jump’ai
        ResolveAutoJumps();

        UpdateUI();
    }

    /// <summary>
    /// Pasirinkimo apdorojimas (kai turėsi pasirinkimų UI).
    /// Iškviesk: DialogueManager.Instance.Choose(index) iš mygtuko OnClick().
    /// </summary>
    public void Choose(int index)
    {
        if (currentNode == null || currentNode.choices == null || currentNode.choices.Length == 0)
            return;

        if (index < 0 || index >= currentNode.choices.Length)
        {
            Debug.LogWarning("DialogueManager.Choose: neteisingas pasirinkimo indeksas.");
            return;
        }

        var choice = currentNode.choices[index];
        if (!string.IsNullOrEmpty(choice.callback))
        {
            HandleCallBack(choice.callback);
        }


        if (!string.IsNullOrWhiteSpace(choice.next))
        {
            GoTo(choice.next);

            // Po perėjimo – jei sutinkame Jump, „praslystame“
            ResolveAutoJumps();

            UpdateUI();
        }
    }

    /// <summary> Pereina į konkretų mazgą pagal ID. </summary>
    public void GoTo(string nodeId)
    {
        var node = loader.GetNode(nodeId);
        if (node == null)
        {
            Debug.LogError($"DialogueManager.GoTo: nerastas mazgas '{nodeId}'.");
            return;
        }
        currentNode = node;
    }

    /// <summary> Grąžina esamą mazgą (jei kam reikia). </summary>
    public DialogueNode GetCurrentNode() => currentNode;

    /// <summary>
    /// Automatiškai „praslysta“ per visus Jump/Goto mazgus:
    /// kol mazgas turi „jumpTo“, pereina į nurodytą tikslą.
    /// </summary>
    private void ResolveAutoJumps()
    {
        int hops = 0;

        while (currentNode != null && hops++ < MaxAutoJumps)
        {
            // CONDITIONS
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

            // JUMP
            if (!string.IsNullOrWhiteSpace(currentNode.jumpTo))
            {
                var target = loader.GetNode(currentNode.jumpTo);
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
            Debug.LogError("Per daug Jump – galimas loop");
        }
    }

    /// <summary> Atnaujina visus UI sluoksnius pagal esamą mazgą. </summary>
    private void UpdateUI()
    {
        if (currentNode == null) return;

        if (!string.IsNullOrWhiteSpace(currentNode.callback) && !executedNodeCallbacks.Contains(currentNode.id))
        {
            executedNodeCallbacks.Add(currentNode.id);
            HandleCallBack(currentNode.callback);
        }

        // Pirminis dialogo UI (portretas, vardas, eilutė)
        if (dialogueUI != null)
        {
            // Šiuo metu portretą paduodame kaip null; jei turi Sprite resolv’ą – integruosi vėliau
            dialogueUI.DisplayDialogue(currentNode.speaker, currentNode.text, null);
        }

        // Antrinis UI (jei naudoji DialogueUIController su atskiru TMP tekstu)
        OnLineDisplayed?.Invoke(currentNode.text ?? string.Empty);
        DialogueBacklog.AddLine(currentNode.text);

        // Vardas viršuje (jei naudoji)
        if (characterDisplay != null)
            characterDisplay.SetSpeakerName(currentNode.speaker);

        // Portretas (jei yra duomenyse)
        if (portraitController != null && !string.IsNullOrEmpty(currentNode.portrait))
        {
            var parts = currentNode.portrait.Split('_');
            var characterName = parts[0];
            var expression = parts.Length > 1 ? parts[1] : "default";
            portraitController.SetPortrait(characterName, expression);
        }

        // Foninis paveikslas (jei yra duomenyse)
        if (backgroundLoader != null && !string.IsNullOrEmpty(currentNode.background))
            backgroundLoader.SetBackground(currentNode.background);

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
        if (node.conditions == null || node.conditions.Length == 0)
        {
            return true;
        }
        foreach (var condition in node.conditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }
        return true;
    }

    private void HandleCallBack(string callBack)
    {
        if (string.IsNullOrWhiteSpace(callBack))
        {
            return;
        }

<<<<<<< HEAD
        // formatas: "CHAPTER:2"
        if (callBack.StartsWith("CHAPTER:"))
        {
            string data = callBack.Replace("CHAPTER:", "").Trim();

            if (int.TryParse(data, out int chapterNumber))
            {
                ChapterManager.Instance?.SetChapter(chapterNumber);
                Debug.Log("Chapter pakeistas į: " + chapterNumber);
            }
            return;
        }

        // formatas: "OBJ:talk_to_teacher"
        if (callBack.StartsWith("OBJ:"))
        {
            string objectiveId = callBack.Replace("OBJ:", "").Trim();
            ChapterManager.Instance?.CompleteObjective(objectiveId);
            Debug.Log("Objective atliktas: " + objectiveId);
            return;
        }

        switch (callBack)
=======
        string[] callBacks = callBack.Split('|');

        foreach (var cb in callBacks)
>>>>>>> 72ce0e4e75d9b89033dad5c30587c15e82e97922
        {
            string trimmed = cb.Trim();

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


        // Nustatome currentNode
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

        // Parodome UI
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
        data.chapter = ChapterManager.Instance.GetChapter();

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

