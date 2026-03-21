using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUIManager : MonoBehaviour
{
    public GameObject choiceButtonPrefab;
    public Transform choicesParent;

    public void ShowChoices(List<Choices> choices)
    {
        ClearChoices();

        foreach (Choices choice in choices)
        {
            Choices currentChoice = choice;

            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesParent);

            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            text.text = choice.text;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                DialogueManager.Instance.Choose(currentChoice);
            });
        }
    }

    public void ClearChoices()
    {
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }
    }
}