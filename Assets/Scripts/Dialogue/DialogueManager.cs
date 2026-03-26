using UnityEngine;
using System;
using System.Collections.Generic;

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

        // Pirminis dialogo UI (portretas, vardas, eilutė)
        if (dialogueUI != null)
        {
            // Šiuo metu portretą paduodame kaip null; jei turi Sprite resolv’ą – integruosi vėliau
            dialogueUI.DisplayDialogue(currentNode.speaker, currentNode.text, null);
        }

        // Antrinis UI (jei naudoji DialogueUIController su atskiru TMP tekstu)
        OnLineDisplayed?.Invoke(currentNode.text ?? string.Empty);

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