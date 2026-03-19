using System.Collections.Generic;
using UnityEngine;

public class GameVariables : MonoBehaviour
{
    public static GameVariables Instance;

    private Dictionary<string, int> intVariables = new();
    private Dictionary<string, bool> boolVariables = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // INT VARIABLES
    public int GetInt(string key)
    {
        return intVariables.ContainsKey(key) ? intVariables[key] : 0;
    }

    public void SetInt(string key, int value)
    {
        intVariables[key] = value;
    }

    public void AddInt(string key, int amount)
    {
        SetInt(key, GetInt(key) + amount);
    }

    // BOOL VARIABLES
    public bool GetBool(string key)
    {
        return boolVariables.ContainsKey(key) && boolVariables[key];
    }

    public void SetBool(string key, bool value)
    {
        boolVariables[key] = value;
    }

    // DEBUG ACCESS
    public Dictionary<string, int> GetAllInts() => intVariables;
    public Dictionary<string, bool> GetAllBools() => boolVariables;
}