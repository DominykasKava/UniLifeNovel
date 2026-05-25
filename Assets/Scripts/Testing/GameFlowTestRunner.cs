using UnityEngine;
using System.Collections;

public class GameFlowTestRunner : MonoBehaviour
{
    public DialogueManager dialogueManager;

    private bool isRunning = false;

    private void Update()
    {
        // Paleidžia visą testą
        if (Input.GetKeyDown(KeyCode.T) && !isRunning)
        {
            StartCoroutine(RunFullGameFlowTest());
        }
    }

    private IEnumerator RunFullGameFlowTest()
    {
        isRunning = true;

        Debug.Log("=== TEST START ===");

        // 1. Start dialogo
        Debug.Log("Step 1: Start dialogue");
        dialogueManager.StartDialogue("start");
        yield return new WaitForSeconds(2f);

        // 2. Pereinam prie kito node
        Debug.Log("Step 2: Next");
        dialogueManager.Next();
        yield return new WaitForSeconds(2f);

        // 3. Keičiam chapter
        Debug.Log("Step 3: Change chapter");
        ChapterManager.Instance.SetChapter(2);
        yield return new WaitForSeconds(2f);

        // 4. Save
        Debug.Log("Step 4: Save");
        dialogueManager.SaveToFile();
        yield return new WaitForSeconds(2f);

        // 5. Pakeičiam state, kad matytum skirtumą
        Debug.Log("Step 5: Change state");
        ChapterManager.Instance.SetChapter(1);
        yield return new WaitForSeconds(2f);

        // 6. Load
        Debug.Log("Step 6: Load");
        dialogueManager.LoadFromFile();
        yield return new WaitForSeconds(2f);

        Debug.Log("=== TEST END ===");

        isRunning = false;
    }
}