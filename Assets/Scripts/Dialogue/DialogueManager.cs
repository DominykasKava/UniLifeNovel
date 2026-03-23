using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI/visual refs (optional)")]
    public CharacterDisplay characterDisplay;
    public PortraitController portraitController;
    public BackgroundLoader backgroundLoader;
    public DialogueUI dialogueUI;

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

        // Jei nėra „next“ – dialogas pasibaigė
        if (string.IsNullOrEmpty(currentNode.next))
        {
            OnDialogueFinished?.Invoke();
            Debug.Log("Dialogas pasibaigė");
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

        var nextId = currentNode.choices[index].next;
        if (string.IsNullOrWhiteSpace(nextId))
        {
            Debug.LogWarning("DialogueManager.Choose: pasirinkimas neturi next ID.");
            return;
        }

        GoTo(nextId);

        // Po perėjimo – jei sutinkame Jump, „praslystame“
        ResolveAutoJumps();

        UpdateUI();
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

        while (currentNode != null &&
               !string.IsNullOrWhiteSpace(currentNode.jumpTo) &&
               hops++ < MaxAutoJumps)
        {
            var targetId = currentNode.jumpTo;
            var target = loader.GetNode(targetId);
            if (target == null)
            {
                Debug.LogError($"DialogueManager: Jump tikslas nerastas: '{targetId}' (šaltinis: '{currentNode.id}').");
                break;
            }
            currentNode = target;
            // ciklas tęsis – jei ir naujasis mazgas yra Jump, persoksime toliau
        }

        if (hops >= MaxAutoJumps)
        {
            Debug.LogError("DialogueManager: per daug automatinų Jump perėjimų (galimas begalinis ciklas).");
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
    }
}