using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MiniObjectivesUI : MonoBehaviour
{
    [System.Serializable]
    public class MiniObjectiveViewData
    {
        public string title;
        public string description;
        public int currentValue;
        public int targetValue;
        public bool isCompleted;
    }

    [Header("Window")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private Transform objectivesContainer;
    [SerializeField] private GameObject objectiveItemPrefab;
    [SerializeField] private TMP_Text emptyText;

    private readonly List<GameObject> spawnedItems = new List<GameObject>();
    private bool isVisible = true;

    private void Start()
    {
        ShowDummyData();
        RefreshEmptyState();
        SetWindowVisible(isVisible);
    }

    public void ToggleWindow()
    {
        isVisible = !isVisible;
        SetWindowVisible(isVisible);
    }

    public void SetWindowVisible(bool visible)
    {
        isVisible = visible;

        if (windowRoot != null)
            windowRoot.SetActive(visible);
    }

    public void DisplayObjectives(List<MiniObjectiveViewData> objectives)
    {
        ClearObjectives();

        foreach (MiniObjectiveViewData data in objectives)
        {
            GameObject item = Instantiate(objectiveItemPrefab, objectivesContainer);
            MiniObjectiveItemUI itemUI = item.GetComponent<MiniObjectiveItemUI>();

            if (itemUI != null)
            {
                itemUI.Setup(
                    data.title,
                    data.description,
                    data.currentValue,
                    data.targetValue,
                    data.isCompleted
                );
            }

            spawnedItems.Add(item);
        }

        RefreshEmptyState();
    }

    public void ClearObjectives()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i]);
        }

        spawnedItems.Clear();
        RefreshEmptyState();
    }

    private void RefreshEmptyState()
    {
        if (emptyText != null)
            emptyText.gameObject.SetActive(spawnedItems.Count == 0);
    }

    private void ShowDummyData()
    {
        List<MiniObjectiveViewData> demoObjectives = new List<MiniObjectiveViewData>
    {
        new MiniObjectiveViewData
        {
            title = "Find the key",
            description = "Search the classroom for the missing key.",
            currentValue = 1,
            targetValue = 3,
            isCompleted = false
        },
        new MiniObjectiveViewData
        {
            title = "Talk to Emma",
            description = "Ask Emma about yesterday's event.",
            currentValue = 1,
            targetValue = 1,
            isCompleted = true
        }
    };

        DisplayObjectives(demoObjectives);
    }
}