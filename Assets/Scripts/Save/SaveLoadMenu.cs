using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    public static SaveLoadMenu Instance;

    public enum MenuMode
    {
        Save,
        Load
    }

    public MenuMode currentMode;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenSaveMenu()
    {
        currentMode = MenuMode.Save;
        gameObject.SetActive(true);
        RefreshAllSlots();
    }

    public void OpenLoadMenu()
    {
        currentMode = MenuMode.Load;
        gameObject.SetActive(true);
        RefreshAllSlots();
    }

    public void QuickSave()
    {
        SaveData data = CreateCurrentSaveData();

        if (data != null)
        {
            SaveSystem.SaveGame(0, data);
            RefreshAllSlots();
            Debug.Log("Quick save padarytas");
        }
    }

    public void CloseSaveMenu()
    {
        gameObject.SetActive(false);
    }

    public void OnSlotClicked(int slotIndex)
    {
        if (currentMode == MenuMode.Save)
        {
            SaveData data = CreateCurrentSaveData();

            if (data != null)
            {
                SaveSystem.SaveGame(slotIndex, data);
            }
        }
        else
        {
            SaveData data = SaveSystem.LoadGame(slotIndex);

            if (data != null)
            {
                ApplyLoadedData(data);
                CloseSaveMenu();
            }
            else
            {
                Debug.Log("Šiame slote nėra save.");
            }
        }

        RefreshAllSlots();
    }

    private SaveData CreateCurrentSaveData()
    {
        return DialogueManager.Instance.CreateSaveData();
    }

    private void ApplyLoadedData(SaveData data)
    {
        DialogueManager.Instance.LoadFromSaveData(data);
    }

    public void RefreshAllSlots()
    {
        SaveSlotUI[] slots = Object.FindObjectsByType<SaveSlotUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (SaveSlotUI slot in slots)
        {
            slot.UpdateVisual();
        }
    }
}