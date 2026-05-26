using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestCase
{
    public string id;
    public string system;
    public string description;
    public string expectedResult;
}

[CreateAssetMenu(fileName = "TestPlan", menuName = "Testing/Test Plan")]
public class TestPlan : ScriptableObject
{
    public List<TestCase> testCases = new List<TestCase>();
}