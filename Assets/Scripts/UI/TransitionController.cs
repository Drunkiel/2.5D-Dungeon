using System.Collections;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public static TransitionController instance;

    [SerializeField] private GameObject UI;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void StartTransition(float delay)
    {
        int animationIndex = Random.Range(0, Mathf.FloorToInt(animator.runtimeAnimatorController.animationClips.Length / 2));
        UI.SetActive(true);
        animator.Play($"Close_{animationIndex}");
        StartCoroutine(WaitAndOpen(animator.GetCurrentAnimatorClipInfo(0).Length + delay, animationIndex));
    }

    private IEnumerator WaitAndOpen(float waitTime, int animationIndex)
    {
        yield return new WaitForSeconds(waitTime);

        animator.Play($"Open_{animationIndex}");
    }
}
