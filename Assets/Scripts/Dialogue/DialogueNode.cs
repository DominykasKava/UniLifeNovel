using UnityEngine;
using System;

[Serializable]

public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public string next;

    public string jumpTo;

    public string portrait;
    public string background;

    public Choices[] choices;

    public DialogueCondition[] conditions;
}
