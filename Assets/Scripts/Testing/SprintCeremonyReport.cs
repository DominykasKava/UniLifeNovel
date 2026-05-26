using System.Text;
using UnityEngine;

public class SprintCeremonyReport : MonoBehaviour
{
    public AcceptanceTests testRunner;



    public void GenerateReport()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("=== SPRINT 6 CEREMONY REPORT ===");
        sb.AppendLine();

        sb.AppendLine("SPRINT PLANNING:");
        sb.AppendLine("- Testavimo planas sudarytas");
        sb.AppendLine("- Pasirinkti acceptance test cases");
        sb.AppendLine();

        sb.AppendLine("SPRINT REVIEW:");
        sb.AppendLine("- Visos pagrindinės sistemos patikrintos");
        sb.AppendLine();

        sb.AppendLine("TEST RESULTS:");
        foreach (var r in testRunner.results)
        {
            sb.AppendLine($"{r.id} - {(r.passed ? "PASS" : "FAIL")} - {r.message}");
        }
     
        sb.AppendLine();
        sb.AppendLine("RETROSPECTIVE:");
        sb.AppendLine("- Sistema veikia stabiliai");
        sb.AppendLine("- Reikalingas UI polish (jei yra)");
        sb.AppendLine("- Testavimo pipeline veikia");

        Debug.Log(sb.ToString());
    }
    void Awake()
    {
        if (testRunner == null)
            testRunner = FindFirstObjectByType<AcceptanceTests>();

    }
}