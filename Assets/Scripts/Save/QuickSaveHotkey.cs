using UnityEngine;

public class QuickSaveHotkey : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (SaveLoadMenu.Instance != null)
            {
                SaveLoadMenu.Instance.QuickSave();
            }
        }
    }
}