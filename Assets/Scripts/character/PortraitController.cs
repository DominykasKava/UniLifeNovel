using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour
{
    [SerializeField] private Image portraitImage;

    public void SetPortrait(string characterName, string expression)
    {
        string path = $"Portraits/{characterName}_{expression}";
        Sprite portrait = Resources.Load<Sprite>(path);

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
        }
        else
        {
            Debug.LogWarning("Portrait not found at: " + path);
        }
    }
}