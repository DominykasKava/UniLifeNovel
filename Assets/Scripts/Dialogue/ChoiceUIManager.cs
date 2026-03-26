using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUIManager : MonoBehaviour
{
    [Header("References")]
    public GameObject choiceButtonPrefab;
    public Transform choicesParent;

    [Header("Sound")]
    public DialogueUIController uiController; // reikalingas tik garsui!

    private List<Button> buttons = new List<Button>();
    private int cnt;

    public bool HasActiveChoices => buttons.Count > 0;

    public void ShowChoices(List<Choices> choices)
    {
        ClearChoices();

        Button btnn = choiceButtonPrefab.GetComponent<Button>();

        if (btnn != null)
        {
            buttons.Add(btnn);
        }

        foreach (var choice in choices)
        {
            // 1) Instantiate button
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesParent);

            // 2) Set text
            TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
            text.text = choice.text;

            // 3) Register button and assign index
            int choiceIndex = buttons.Count - 1;

            Button btn = buttonObj.GetComponent<Button>();
            buttons.Add(btn);

            // 4) Add listener (LOCALIZED COPY of “choiceIndex”)
            btn.onClick.AddListener(() =>
            {
                // Garsas
                if (uiController != null)
                    uiController.PlayChoiceSound();

                // Dialogo logika
                DialogueManager.Instance.Choose(choiceIndex);
            });

            ChoiceAnimation anim = buttonObj.GetComponent<ChoiceAnimation>();
            if (anim != null)
            {
                anim.PlayShow();
            }
        }

        cnt = buttons.Count > 0 ? 0 : -1;
        UpdateSelectionVisual();
    }

    public void ClearChoices()
    {
        foreach (Transform child in choicesParent)
        {
            ChoiceAnimation anim = child.GetComponent<ChoiceAnimation>();
            if (anim != null)
            {
                anim.PlayHide();
                Destroy(child.gameObject, 0.3f);
            }
            else
            {
                Destroy(child.gameObject);
            }
        }

        buttons.Clear();
        cnt = -1;
    }

    private void UpdateSelectionVisual()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var colors = buttons[i].colors;
            if (i == cnt)
            {
                colors.normalColor = new Color(1f, 0.95f, 0.7f);
            }
            else
            {
                colors.normalColor = Color.white; // 
            }
            buttons[i].colors = colors;
        }
    }

    private void Update()
    {
        if (buttons.Count == 0) return;

        // Down
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (cnt < buttons.Count - 1)
            {
                cnt++;
                UpdateSelectionVisual();
            }
        }

        // Up
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (cnt > 0)
            {
                cnt--;
                UpdateSelectionVisual();
            }
        }

        // Press Enter or Space → Activate selected button
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (cnt >= 0 && cnt < buttons.Count)
            {
                // Grojame garsą IR čia (klaviatūros atvejui)
                if (uiController != null)
                    uiController.PlayChoiceSound();

                buttons[cnt].onClick.Invoke();
            }
        }
    }
}