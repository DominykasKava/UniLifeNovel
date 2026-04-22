using UnityEngine;

public class ObjectiveDebugInput : MonoBehaviour
{
    [SerializeField] private string objectiveId = "obj_1";

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ObjectiveTracker.Instance.ActivateObjective(objectiveId);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ObjectiveTracker.Instance.SetProgress(objectiveId, 0.5f);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ObjectiveTracker.Instance.CompleteObjective(objectiveId);

    }
}