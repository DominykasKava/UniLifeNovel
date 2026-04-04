using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    public static SaveLoadMenu Instance;

    public GameObject slotPrefab;
    public Transform container;

    public int slotCount = 3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateSlots();
    }

    void CreateSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject obj = Instantiate(slotPrefab, container);
            SaveSlotUI slot = obj.GetComponent<SaveSlotUI>();
            slot.Init(i);
        }
    }

    public void OnSlotClicked(int index)
    {
        Debug.Log("Clicked slot: " + index);

        // Čia vėliau jungsi:
        // SaveSystem.Save(index);
        // SaveSystem.Load(index);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}