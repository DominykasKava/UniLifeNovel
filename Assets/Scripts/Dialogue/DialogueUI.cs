using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portrait;

    public float typingSpeed = 0.03f;

    Coroutine typingCoroutine;

    public void DisplayDialogue(string speaker, string line, Sprite portraitSprite)
    {
        nameText.text = speaker;
        portrait.sprite = portraitSprite;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}