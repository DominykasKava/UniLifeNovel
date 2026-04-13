using UnityEngine;
using System;

[Serializable]
public class MiniObjective
{
    public string id;
    public string title;
    [TextArea] public string description;

    public ObjectiveState state = ObjectiveState.Locked;

    [Range(0f, 1f)]
    public float progress = 0f;
}
