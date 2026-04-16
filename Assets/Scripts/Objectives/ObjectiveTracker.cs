using UnityEngine;
using System;
using System.Collections.Generic;

public class ObjectiveTracker : MonoBehaviour
{
    public static ObjectiveTracker Instance { get; private set; }

    [SerializeField] private List<MiniObjective> objectives = new();

    private Dictionary<string, MiniObjective> objectiveMap = new();

    public event Action<MiniObjective> OnObjectiveActivated;
    public event Action<MiniObjective> OnObjectiveCompleted;
    public event Action<MiniObjective> OnObjectiveProgressChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildMap();
    }

    private void BuildMap()
    {
        objectiveMap.Clear();

        foreach (var objective in objectives)
        {
            if (objective == null || string.IsNullOrWhiteSpace(objective.id))
            {
                continue;
            }
            if (!objectiveMap.ContainsKey(objective.id))
            {
                objectiveMap.Add(objective.id, objective);
            }
        }
    }

    public List<MiniObjective> GetAllObjectives()
    {
        return objectives;
    }

    public MiniObjective GetObjective(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        objectiveMap.TryGetValue(id, out MiniObjective objective);
        return objective;
    }

    public void ActivateObjective(string id)
    {
        MiniObjective objective = GetObjective(id);
        if (objective == null)
        {
            return;
        }
        if (objective.state != ObjectiveState.Locked)
        {
            return;
        }

        objective.state = ObjectiveState.Active;
        objective.progress = Mathf.Clamp01(objective.progress);

        OnObjectiveActivated?.Invoke(objective);
    }

    public void CompleteObjective(string id)
    {
        MiniObjective objective = GetObjective(id);
        if (objective == null)
        {
            return; 
        }
        if (objective.state == ObjectiveState.Completed)
        {
            return;
        }

        objective.state = ObjectiveState.Completed;
        objective.progress = 1f;

        OnObjectiveCompleted?.Invoke(objective);
    }

    public void SetProgress(string id, float value)
    {
        MiniObjective objective = GetObjective(id);
        if (objective == null)
        {
            return;
        }
        if (objective.state == ObjectiveState.Locked)
        {
            objective.state = ObjectiveState.Active;
            OnObjectiveActivated?.Invoke(objective);
        }

        objective.progress = Mathf.Clamp01(value);

        OnObjectiveProgressChanged?.Invoke(objective);

        if (objective.progress >= 1f)
        {
            objective.state = ObjectiveState.Completed;
            OnObjectiveCompleted?.Invoke(objective);
        }
    }

    public bool IsObjectiveCompleted(string id)
    {
        MiniObjective objective = GetObjective(id);
        return objective != null && objective.state == ObjectiveState.Completed;
    }

    public bool IsObjectiveActive(string id)
    {
        MiniObjective objective = GetObjective(id);
        return objective != null && objective.state == ObjectiveState.Active;
    }

    public void EvaluateObjectives()
    {
        foreach (var objective in objectives)
        {
            if (objective == null)
            {
                continue;
            }

            if (objective.state == ObjectiveState.Locked && objective.CanActivate())
            {
                objective.state = ObjectiveState.Active;
                objective.progress = Mathf.Max(objective.progress, 0f);
                OnObjectiveActivated?.Invoke(objective);
            }

            if (objective.state == ObjectiveState.Active && objective.CanComplete())
            {
                objective.state = ObjectiveState.Completed;
                objective.progress = 1f;
                OnObjectiveCompleted?.Invoke(objective);
            }
        }
    }
}

