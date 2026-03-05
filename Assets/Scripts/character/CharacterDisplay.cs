/*using UnityEngine;
using TMPro;

public class CharacterDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    private void OnEnable()
    {
        SpeakerEvents.OnSpeakerChanged += SetSpeakerName;
    }

    private void OnDisable()
    {
        SpeakerEvents.OnSpeakerChanged -= SetSpeakerName;
    }

    public void SetSpeakerName(string speakerName)
    {
        nameText.text = speakerName;
    }
}*/