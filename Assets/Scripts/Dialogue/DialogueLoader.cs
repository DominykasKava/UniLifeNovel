using UnityEngine;
using System.Collections.Generic;

public class DialogueLoader
{
    private Dictionary<string, DialogueNode> nodeDict;

    public void Load(string fileName)
    {
        nodeDict = new Dictionary<string, DialogueNode>();
        TextAsset jsonFile = Resources.Load<TextAsset>("Dialogue/" + fileName);

        if (jsonFile == null)
        {
            Debug.LogError("Dialogo failas nerastas: " + fileName);
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(jsonFile.text);

        foreach (var node in data.nodes)
        {
            if (nodeDict.ContainsKey(node.id))
            {
                Debug.LogError("Duplicate id: " + node.id);
                continue;
            }
            nodeDict.Add(node.id, node);
        }
    }

    public DialogueNode GetNode(string id)
    {
        if (nodeDict == null)
        {
            Debug.LogError("Dialogas neužkrautas");
            return null;
        }

        if (!nodeDict.ContainsKey(id))
        {
            Debug.LogError("Node nerastas: " + id);
        }

        return nodeDict[id];
    }
}
