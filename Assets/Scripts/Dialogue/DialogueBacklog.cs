using System.Collections.Generic;

public static class DialogueBacklog
{
    public static List<string> entries = new List<string>();

    public static void AddLine(string line)
    {
        if (!string.IsNullOrEmpty(line))
            entries.Add(line);
    }
}