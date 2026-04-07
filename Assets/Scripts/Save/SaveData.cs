using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string currentNodeID;
    public int dialogueIndex;
    public List<IntVariableData> intVariables = new List<IntVariableData>();
    public List<BoolVariableData> boolVariables = new List<BoolVariableData>();
}
