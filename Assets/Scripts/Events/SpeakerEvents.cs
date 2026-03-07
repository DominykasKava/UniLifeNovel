using System;

public static class SpeakerEvents
{
    public static Action<string> OnSpeakerChanged;

    public static void TriggerSpeakerChange(string speakerName)
    {
        OnSpeakerChanged?.Invoke(speakerName);
    }
}