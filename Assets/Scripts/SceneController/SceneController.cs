using UnityEngine;

public class SceneController : MonoBehaviour
{
    // ŠITAS bus prijungtas kai kitas žmogus sukurs dialogo sistemą
    public MonoBehaviour dialogueSystem;

    // Ar dialogas šiuo metu aktyvus
    private bool dialogueActive = false;

    void Update()
    {
        // FAST FORWARD (pereiti prie kitos eilutės)
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("FAST-FORWARD paspaustas (kol kas tik log).");
            // ČIA vėliau prijungs call į dialogManager.NextLine()
        }

        // SKIP (išjungti dialogą)
        if (dialogueActive && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("SKIP paspaustas (kol kas tik log).");
            StopDialogue();
        }
    }

    /// <summary>
    /// Paleidžia dialogą (kol kas tik indikatoriai)
    /// </summary>
    public void StartDialogue()
    {
        if (dialogueSystem == null)
        {
            Debug.LogError("NEPASKIRTAS dialogueSystem! (čia error handling).");
            return;
        }

        dialogueActive = true;
        Debug.Log("Dialogas pradėtas.");
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

