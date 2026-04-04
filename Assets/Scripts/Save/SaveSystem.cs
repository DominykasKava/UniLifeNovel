using UnityEngine;

public static class SaveSystem
{
    public static bool HasSave(int index)
    {
        return PlayerPrefs.HasKey("save_" + index);
    }
}