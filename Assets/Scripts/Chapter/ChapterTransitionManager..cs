using UnityEngine;

public class ChapterTransitionManager : MonoBehaviour
{
    public GameObject[] chapterRoots; // kiekvienas chapter UI / scene root

    public void SwitchChapter(int chapterIndex)
    {
        for (int i = 0; i < chapterRoots.Length; i++)
        {
            chapterRoots[i].SetActive(i == chapterIndex);
        }

        ChapterManager.Instance.SetChapter(chapterIndex + 1);
    }
}