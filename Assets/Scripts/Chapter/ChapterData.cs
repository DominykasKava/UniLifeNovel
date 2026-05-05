using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChapterData", menuName = "VN/Chapter Data")]
public class ChapterData : ScriptableObject
{
    public string chapterName;
    public List<string> dialogueNodeIDs;
    public string startNodeID;
}