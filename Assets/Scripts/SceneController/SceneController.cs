using UnityEngine;

public class SceneController : MonoBehaviour
{
    public DialogueManager dialogueSystem;

    // Ar dialogas šiuo metu aktyvus
    private bool dialogueActive = false;

    private void Start()
    {
        StartDialogue();
    }

    void Update()
    {
        // FAST FORWARD (pereiti prie kitos eilutės)
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            dialogueSystem.Next();
        }

        // SKIP (išjungti dialogą)
        if (dialogueActive && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("SKIP paspaustas.");
            StopDialogue();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("Restart dialoga");
            StartDialogue();
        }
    }

    /// <summary>
    /// Paleidžia dialogą (kol kas tik indikatoriai)
    /// </summary>
    public void StartDialogue()
    {
        if (GameVariables.Instance != null)
        {
            GameVariables.Instance.SetInt("trust", 0);
        }

        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue("start");
            dialogueActive = true;
        }
        else
        {
            Debug.LogError("DialogueSystem nepriskirtas.");
        }
    }

    /// <summary>
    /// Sustabdo dialogą
    /// </summary>
    public void StopDialogue()
    {
        dialogueActive = false;
        Debug.Log("Dialogas sustabdytas.");
    }
}

