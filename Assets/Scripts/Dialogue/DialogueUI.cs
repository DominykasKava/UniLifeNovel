using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portrait;

    public void DisplayDialogue(string speaker, string line, Sprite portraitSprite)
    {
        nameText.text = speaker;
        dialogueText.text = line;
        portrait.sprite = portraitSprite;
    }
}