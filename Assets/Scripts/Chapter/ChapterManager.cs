using System.Collections.Generic;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance;

    public int currentChapter = 1;
    public List<ChapterDefinition> chapters = new List<ChapterDefinition>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetChapter(int chapter)
    {
        currentChapter = chapter;
        Debug.Log("Chapter changed to: " + currentChapter);
    }

    public int GetChapter()
    {
        return currentChapter;
    }

    public void CompleteObjective(string objectiveId)
    {
        if (string.IsNullOrEmpty(objectiveId))
            return;

        Debug.Log("Objective completed: " + objectiveId);
    }

}