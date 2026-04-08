using UnityEngine;

public static class SaveSystem
{
    public static bool HasSave(int index)
    {
        return PlayerPrefs.HasKey("save_" + index);
    }

    public static void SaveGame(int index, SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("save_" + index, json);
        PlayerPrefs.Save();
    }

    public static SaveData LoadGame(int index)
    {
        if (!HasSave(index))
            return null;

        string json = PlayerPrefs.GetString("save_" + index);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave(int index)
    {
        if (HasSave(index))
        {
            PlayerPrefs.DeleteKey("save_" + index);
            PlayerPrefs.Save();
        }
    }
}