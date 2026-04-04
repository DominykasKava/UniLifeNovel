using UnityEngine;

public class SlotsSpawner : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotsContainer;
    public int slotCount = 3;

    private void Start()
    {
        SpawnSlots();
    }

    public void SpawnSlots()
    {
        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObject = Instantiate(slotPrefab, slotsContainer);
            SaveSlotUI slotUI = slotObject.GetComponent<SaveSlotUI>();

            if (slotUI != null)
            {
                slotUI.Init(i);
            }
        }
    }
}