using System;
using UnityEngine;

public class SkipFoward : MonoBehaviour
{
    // Event'ai UI sistemai
    public event Action<string> OnLineDisplayed;
    public event Action OnDialogueFinished;

    // Dialogo duomenys
    [TextArea]
    public string[] lines;

    private int currentIndex = 0;
    private bool active = false;

    void Update()
    {
        if (!active) return;

        // FAST FORWARD – kita dialogo eilutė
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }

        // SKIP – baigti dialogą
        if (Input.GetKeyDown(KeyCode.S))
        {
            FinishDialogue();
        }
    }

    public void StartDialogue()
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogError("Nėra dialogo eilučių! (error handling)");
            return;
        }

        active = true;
        currentIndex = 0;
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentIndex >= lines.Length)
        {
            FinishDialogue();
            return;
        }

        string line = lines[currentIndex];
        currentIndex++;

        Debug.Log("Rodoma eilutė: " + line);

        // Pranešame UI
        OnLineDisplayed?.Invoke(line);
    }

    private void FinishDialogue()
    {
        active = false;
        Debug.Log("Dialogas baigtas.");

        // Pranešame UI
        OnDialogueFinished?.Invoke();
    }
}

