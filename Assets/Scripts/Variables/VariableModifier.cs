using UnityEngine;

[System.Serializable]
public class VariableModifier
{
    public string variableName;
    public int amount;
    public bool isBool;
    public bool boolValue;

    public void Apply()
    {
        if (isBool)
        {
            GameVariables.Instance.SetBool(variableName, boolValue);
        }
        else
        {
            GameVariables.Instance.AddInt(variableName, amount);
        }
    }
}