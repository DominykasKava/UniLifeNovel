using UnityEngine;
using UnityEngine.UI;

public class BackgroundLoader : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    public void SetBackground(string backgroundName)
    {
        Sprite bg = Resources.Load<Sprite>("Backgrounds/" + backgroundName);

        if (bg != null)
        {
            backgroundImage.sprite = bg;
        }
        else
        {
            Debug.LogWarning("Background not found: " + backgroundName);
        }
    }
}