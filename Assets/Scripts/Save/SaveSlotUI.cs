using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotText;
    public Button button;

    private int slotIndex;

    public void Init(int index)
    {
        slotIndex = index;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (SaveSystem.HasSave(slotIndex))
        {
            slotText.text = "Save " + slotIndex;
        }
        else
        {
            slotText.text = "Empty";
        }
    }

    public void OnClick()
    {
        SaveLoadMenu.Instance.OnSlotClicked(slotIndex);
    }
}