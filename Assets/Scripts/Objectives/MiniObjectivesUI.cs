using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniObjectivesUI : MonoBehaviour
{
    [Header("Window")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private Transform objectivesContainer;
    [SerializeField] private GameObject objectiveItemPrefab;
    [SerializeField] private TMP_Text emptyText;

    private readonly List<GameObject> spawnedItems = new();
    private readonly Dictionary<string, MiniObjectiveItemUI> itemMap = new();

    private bool isVisible = true;

    private void Start()
    {
        BuildObjectivesList();
        RefreshEmptyState();
        SetWindowVisible(isVisible);
    }

    private void OnEnable()
    {
        if (ObjectiveTracker.Instance != null)
        {
            ObjectiveTracker.Instance.OnObjectiveActivated += HandleObjectiveActivated;
            ObjectiveTracker.Instance.OnObjectiveCompleted += HandleObjectiveCompleted;
            ObjectiveTracker.Instance.OnObjectiveProgressChanged += HandleObjectiveProgressChanged;
        }
    }

    private void OnDisable()
    {
        if (ObjectiveTracker.Instance != null)
        {
            ObjectiveTracker.Instance.OnObjectiveActivated -= HandleObjectiveActivated;
            ObjectiveTracker.Instance.OnObjectiveCompleted -= HandleObjectiveCompleted;
            ObjectiveTracker.Instance.OnObjectiveProgressChanged -= HandleObjectiveProgressChanged;
        }
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

    private void BuildObjectivesList()
    {
        ClearSpawnedItems();

        if (ObjectiveTracker.Instance == null)
            return;

        List<MiniObjective> objectives = ObjectiveTracker.Instance.GetAllObjectives();

        foreach (MiniObjective objective in objectives)
        {
            if (objective == null)
                continue;

            if (objective.state == ObjectiveState.Locked)
                continue;

            CreateObjectiveItem(objective);
        }
    }

    private void CreateObjectiveItem(MiniObjective objective)
    {
        if (objectiveItemPrefab == null || objectivesContainer == null || objective == null)
            return;

        GameObject itemObject = Instantiate(objectiveItemPrefab, objectivesContainer);
        spawnedItems.Add(itemObject);

        MiniObjectiveItemUI itemUI = itemObject.GetComponent<MiniObjectiveItemUI>();

        if (itemUI != null)
        {
            itemUI.Setup(
                objective.title,
                objective.description,
                objective.progress,
                objective.state == ObjectiveState.Completed
            );

            if (!itemMap.ContainsKey(objective.id))
                itemMap.Add(objective.id, itemUI);
        }
    }

    private void HandleObjectiveActivated(MiniObjective objective)
    {
        if (objective == null)
            return;

        if (itemMap.ContainsKey(objective.id))
            return;

        CreateObjectiveItem(objective);
        RefreshEmptyState();
    }

    private void HandleObjectiveProgressChanged(MiniObjective objective)
    {
        if (objective == null)
            return;

        if (itemMap.TryGetValue(objective.id, out MiniObjectiveItemUI itemUI))
        {
            itemUI.UpdateProgress(objective.progress);

            if (objective.state == ObjectiveState.Completed)
                itemUI.MarkCompleted();
        }
    }

    private void HandleObjectiveCompleted(MiniObjective objective)
    {
        if (objective == null)
            return;

        if (itemMap.TryGetValue(objective.id, out MiniObjectiveItemUI itemUI))
            itemUI.MarkCompleted();
    }

    private void RefreshEmptyState()
    {
        if (emptyText != null)
            emptyText.gameObject.SetActive(spawnedItems.Count == 0);
    }

    private void ClearSpawnedItems()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }

        spawnedItems.Clear();
        itemMap.Clear();
    }
}