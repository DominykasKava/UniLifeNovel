using UnityEngine;

public class DialoguePauseBlocker : MonoBehaviour
{

    void Update()
    {
        if (PauseManager.Instance == null) return;

        // Jei žaidimas sustabdytas – neleisti dialogo input
        if (PauseManager.Instance.IsPaused)
        {
            enabled = false;
            return;
        }

        // Jei pauzė atjungta – vėl leidžiame veikti
        if (!enabled)
        {
            enabled = true;
        }
    }

}
