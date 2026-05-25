using UnityEngine;

public class SaveLoadTestRunner : MonoBehaviour
{
    [Header("Refs")]
    public DialogueManager dialogueManager;

    private void Update()
    {
        // TEST 1: Save
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("TEST: Save");
            dialogueManager.SaveToFile();
        }

        // TEST 2: Load
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("TEST: Load");
            dialogueManager.LoadFromFile();
        }

        // TEST 3: Chapter keitimas
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChapterManager.Instance.SetChapter(1);
            Debug.Log("TEST: Chapter -> 1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChapterManager.Instance.SetChapter(2);
            Debug.Log("TEST: Chapter -> 2");
        }
    }
}