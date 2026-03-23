using UnityEngine;

public class ChoiceAnimation : MonoBehaviour
{
    [Header("Animacijos")]
    [SerializeField] private Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Paleidžia SHOW animaciją (kai pasirinkimas atsiranda).
    /// </summary>
    public void PlayShow()
    {
        if (animator != null)
            animator.SetTrigger("Show");
    }

    /// <summary>
    /// Paleidžia HIDE animaciją (kai pasirinkimas dingsta).
    /// </summary>
    public void PlayHide()
    {
        if (animator != null)
            animator.SetTrigger("Hide");
    }
}