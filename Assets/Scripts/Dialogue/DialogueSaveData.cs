using System;
using System.Collections.Generic;

[Serializable]
public class DialogueSaveData
{
    public string nodeId;
    public string chapterId;
    public List<ObjectiveSaveEntry> objectives = new List<ObjectiveSaveEntry>();
}

public class ObjectiveSaveEntry
{
    public string id;
    public bool done;
}