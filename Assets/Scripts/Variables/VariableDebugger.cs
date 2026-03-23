using UnityEngine;

public class VariableDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("=== INT VARIABLES ===");
            foreach (var v in GameVariables.Instance.GetAllInts())
            {
                Debug.Log(v.Key + " = " + v.Value);
            }

            Debug.Log("=== BOOL VARIABLES ===");
            foreach (var v in GameVariables.Instance.GetAllBools())
            {
                Debug.Log(v.Key + " = " + v.Value);
            }
        }
    }
}