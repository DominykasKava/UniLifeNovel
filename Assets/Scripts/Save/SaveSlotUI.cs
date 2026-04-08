using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotText;
    public Button button;
    public Button deleteButton;

    private int slotIndex;

    public void Init(int index)
    {
        slotIndex = index;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(DeleteSlot);
        }

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        bool hasSave = SaveSystem.HasSave(slotIndex);

        if (slotText != null)
        {
            if (hasSave)
            {
                slotText.text = "Save " + (slotIndex + 1);
            }
            else
            {
                slotText.text = "Empty Slot " + (slotIndex + 1);
            }
        }

        if (deleteButton != null)
        {
            deleteButton.gameObject.SetActive(hasSave);
        }
    }

    public void OnClick()
    {
        SaveLoadMenu.Instance.OnSlotClicked(slotIndex);
    }

    public void DeleteSlot()
    {
        SaveSystem.DeleteSave(slotIndex);
        SaveLoadMenu.Instance.RefreshAllSlots();
    }
}