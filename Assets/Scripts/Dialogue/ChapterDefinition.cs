using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter", menuName = "Dialogue/Chapter Definition")]

public class ChapterDefinition : MonoBehaviour
{
    public int chapterNumber;
    public string chapterId;          // pvz. "chapter1"
    public string title;              // pvz. "1 skyrius"
    public string startNodeId;        // nuo kurio dialogo mazgo pradėti

    [Header("Mini objectives")]
    public List<ObjectiveDefinition> objectives = new List<ObjectiveDefinition>();
}

[Serializable]
public class ObjectiveDefinition
{
    public string objectiveId;        // pvz. "talk_to_teacher"
    public string title;              // pvz. "Paklausk mokytojo"
}