using UnityEngine;
using System.IO;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    public SettingsData currentSettings = new SettingsData();
    private string path;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        path = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(currentSettings, true);
        File.WriteAllText(path, json);
    }

    public void LoadSettings()
    {
        if (!File.Exists(path))
        {
            SaveSettings();
            return;
        }

        string json = File.ReadAllText(path);
        currentSettings = JsonUtility.FromJson<SettingsData>(json);
    }
}
