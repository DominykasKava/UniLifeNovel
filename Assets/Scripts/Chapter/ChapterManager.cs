using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance;

    public int currentChapter = 1;

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
}