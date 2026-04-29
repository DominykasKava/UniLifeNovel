using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MiniObjective
{
    public string id;
    public string title;
    [TextArea] public string description;

    public ObjectiveState state = ObjectiveState.Locked;

    [Range(0f, 1f)]
    public float progress = 0f;

    public List<ObjectiveCondition> activationConditions = new();
    public List<ObjectiveCondition> completionConditions = new();
    public List<ObjectiveCondition> failConditions = new();
    public bool CanActivate()
    {
        if (activationConditions == null || activationConditions.Count == 0)
        {
            return false;
        }

        foreach (var condition in activationConditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }
        return true;
    }

    public bool CanComplete()
    {
        if (completionConditions == null || completionConditions.Count == 0)
        {
            return false;
        }

        foreach (var condition in completionConditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }
        return true;
    }

    public bool CanFail()
    {
        if (failConditions == null || failConditions.Count == 0)
        {
            return false;
        }

        foreach (var condition in failConditions)
        {
            if (!condition.Evaluate())
            {
                return false;
            }
        }
        return true;
    }
}

[Serializable]
public class ObjectiveCondition
{
    public string variableKey;
    public bool useIntCondition;
    public int requiredIntValue;
    public bool useBoolCondition;
    public bool requiredBoolValue;
    
    public bool Evaluate()
    {
        if (string.IsNullOrWhiteSpace(variableKey))
        {
            return true;
        }

        if (useIntCondition)
        {
            return GameVariables.Instance.GetInt(variableKey) >= requiredIntValue;  
        }

        if (useBoolCondition)
        {
            return GameVariables.Instance.GetBool(variableKey) == requiredBoolValue;
        }
        return true;
    }
}
