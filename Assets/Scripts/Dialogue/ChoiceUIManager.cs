using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class ChoiceUIManager : MonoBehaviour
{
    public GameObject choiceButtonPrefab;
    public Transform choicesParent;
    public List<Button> buttons = new List<Button>();
    public int cnt;

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
            buttons.Add(btn);
            btn.onClick.AddListener(() =>
            {
                DialogueManager.Instance.Choose(currentChoice);
            });
        }
        if(buttons.Count > 0)
        {
            cnt = 0;
        }
        UpdateSelectionVisual();
    }

    public void ClearChoices()
    {
        foreach (Transform child in choicesParent)
        {
            Destroy(child.gameObject);
        }
        buttons.Clear();
        cnt = 0;
    }

    private void UpdateSelectionVisual()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == cnt)
            {
                buttons[i].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            }
            else
            {
                buttons[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    void Update()
    {
        if (buttons.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (cnt < buttons.Count - 1)
            {
                cnt++;
                UpdateSelectionVisual();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (cnt > 0)
            {
                cnt--;
                UpdateSelectionVisual();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            buttons[cnt].onClick.Invoke();
        }
    }
}