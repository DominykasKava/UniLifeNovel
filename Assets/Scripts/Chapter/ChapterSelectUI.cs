using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("UI")]
    public Transform listParent;      // VerticalLayoutGroup parent
    public Button buttonPrefab;       // Button prefab su TextMeshProUGUI

    [Header("Refs")]
    public ChapterManager chapterManager;
    public DialogueManager dialogueManager;

    public void Build()
    {
        // Išvalom senus mygtukus
        for (int i = listParent.childCount - 1; i >= 0; i--)
        {
            Destroy(listParent.GetChild(i).gameObject);
        }

        // Kuriam mygtukus pagal chapter definitions
        for (int i = 0; i < chapterManager.chapters.Count; i++)
        {
            ChapterDefinition ch = chapterManager.chapters[i];
            if (ch == null) continue;

            Button btn = Instantiate(buttonPrefab, listParent);
            TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = ch.title;

            int chapterNumber = ch.chapterNumber;
            string startNode = ch.startNodeId;

            btn.onClick.AddListener(() =>
            {
                chapterManager.SetChapter(chapterNumber);
                dialogueManager.StartDialogue(startNode);
                gameObject.SetActive(false);
            });
        }
    }
}




