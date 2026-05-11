using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public Slider textSpeedSlider;
    public TMP_Text textSpeedValueText;
    public Toggle fullscreenToggle;

    private void Start()
    {
        LoadValuesToUI();

        textSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    private void LoadValuesToUI()
    {
        float textSpeed = SettingsManager.instance.currentSettings.textSpeed;
        bool fullscreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        textSpeedSlider.value = textSpeed;
        textSpeedValueText.text = textSpeed.ToString("0.0");

        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;
    }

    private void OnTextSpeedChanged(float value)
    {
        SettingsManager.instance.currentSettings.textSpeed = value;
        textSpeedValueText.text = value.ToString("0.0");

        SettingsManager.instance.SaveSettings();
    }

    private void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;

        PlayerPrefs.SetInt("fullscreen", value ? 1 : 0);
        PlayerPrefs.Save();
    }
}