using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public static TransitionController instance;

    public string[] animationNames;
    [SerializeField] private GameObject UI;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        instance = this;
    }

    public void StartTransition()
    {
        int randomAnimationIndex = Random.Range(0, animationNames.Length);

        UI.SetActive(true);
        animator.Play(animationNames[randomAnimationIndex]);
    }
}
