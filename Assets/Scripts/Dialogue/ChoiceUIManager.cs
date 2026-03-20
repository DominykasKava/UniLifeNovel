using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject choiceButtonPrefab;
    public Transform choicesParent;

    public void ShowChoices(List<Choices> choices)
    {
        ClearChoices();

        foreach (Choices choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesParent);

            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            text.text = choice.text;

            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                Debug.Log("Selected: " + choice.text);
            });
        }
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }
    }
}