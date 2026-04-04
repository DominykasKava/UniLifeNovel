using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string currentNodeID;
    public int dialogueIndex;
    public Dictionary<string, int> variables;

}
