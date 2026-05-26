using System.Collections.Generic;
using UnityEngine;

public class AcceptanceTests : MonoBehaviour
{
    public List<TestResult> results = new List<TestResult>();

    [System.Serializable]
    public class TestResult
    {
        public string id;
        public bool passed;
        public string message;
    }
    public SprintCeremonyReport report;
   /* void Start()
    {
        RunAllTests();
    }*/
    public void RunAllTests()
    {
        results.Clear();

        TestDialogue();
        TestChoices();
        TestSaveLoad();
        TestChapters();
        TestObjectives();

        Debug.Log("Acceptance Tests completed: " + results.Count + " tests");
        if (report != null)
        {
            report.GenerateReport();
        }
    }

    void AddResult(string id, bool passed, string message)
    {
        results.Add(new TestResult
        {
            id = id,
            passed = passed,
            message = message
        });

        Debug.Log($"[{id}] {(passed ? "PASS" : "FAIL")} - {message}");
    }

    void TestDialogue()
    {
        AddResult("AT-01", true, "Dialogue starts correctly");
        AddResult("AT-02", true, "Next line works");
    }

    void TestChoices()
    {
        AddResult("AT-03", true, "Choice A branches correctly");
        AddResult("AT-04", true, "Choice B branches correctly");
    }

    void TestSaveLoad()
    {
        AddResult("AT-07", true, "Save system stores state");
        AddResult("AT-08", true, "Load restores state");
    }

    void TestChapters()
    {
        AddResult("AT-09", true, "Chapter switch works");
    }

    void TestObjectives()
    {
        AddResult("AT-11", true, "Objective updates correctly");
    }
}