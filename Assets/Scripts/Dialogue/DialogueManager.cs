using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public DialogueUI dialogueUI;
    public Sprite johnSprite;

    void Start()
    {
        ShowLine("Ben", "Hello world", johnSprite);
    }

    public void ShowLine(string speaker, string line, Sprite portraitSprite)
    {
        dialogueUI.DisplayDialogue(speaker, line, portraitSprite);
    }
}