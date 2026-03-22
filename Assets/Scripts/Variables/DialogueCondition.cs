using UnityEngine;

[System.Serializable]
public class DialogueCondition
{
    public string variableName;
    public int requiredValue;
    public ComparisonType comparison;

    public enum ComparisonType
    {
        Equal,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual
    }

    public bool Evaluate()
    {
        int value = GameVariables.Instance.GetInt(variableName);

        return comparison switch
        {
            ComparisonType.Equal => value == requiredValue,
            ComparisonType.Greater => value > requiredValue,
            ComparisonType.Less => value < requiredValue,
            ComparisonType.GreaterOrEqual => value >= requiredValue,
            ComparisonType.LessOrEqual => value <= requiredValue,
            _ => false
        };
    }
}