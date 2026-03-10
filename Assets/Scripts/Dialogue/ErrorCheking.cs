using System;
using UnityEngine;

public class ErrorCheking : MonoBehaviour
{

    // Event'ai kitiems (UI ir pan.)
    public event Action<string> OnLineDisplayed;
    public event Action OnDialogueFinished;

    private string[] _lines;
    private int _index;

    /// <summary>
    /// Ar dialogas šiuo metu aktyvus.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Paleidžia dialogą iš string masyvo.
    /// </summary>
    public void Begin(string[] lines)
    {
        if (lines == null)
        {
            Debug.LogError("DialogueManager.Begin: lines == null");
            OnDialogueFinished?.Invoke();
            return;
        }
        if (lines.Length == 0)
        {
            Debug.LogError("DialogueManager.Begin: lines is empty");
            OnDialogueFinished?.Invoke();
            return;
        }

        _lines = lines;
        _index = 0;
        IsActive = true;
        ShowNextLineInternal();
    }

    /// <summary>
    /// Pereina į kitą eilutę (FAST-FORWARD).
    /// </summary>
    public void NextLine()
    {
        if (!IsActive)
        {
            Debug.LogWarning("DialogueManager.NextLine: dialogas neaktyvus");
            return;
        }
        ShowNextLineInternal();
    }

    /// <summary>
    /// Praleidžia dialogą (SKIP).
    /// </summary>
    public void Skip()
    {
        if (!IsActive) return;
        FinishInternal();
    }

    private void ShowNextLineInternal()
    {
        if (_lines == null || _lines.Length == 0)
        {
            Debug.LogError("DialogueManager: nėra eilučių rodyti");
            FinishInternal();
            return;
        }

        if (_index >= _lines.Length)
        {
            FinishInternal();
            return;
        }

        string line = _lines[_index++];
        Debug.Log("Rodoma eilutė: " + line);
        OnLineDisplayed?.Invoke(line);
    }

    private void FinishInternal()
    {
        IsActive = false;
        _lines = null;
        _index = 0;
        Debug.Log("Dialogas baigtas.");
        OnDialogueFinished?.Invoke();
    }
}