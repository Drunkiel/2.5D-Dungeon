using System.Collections;
using UnityEngine;

public class AnimationDelay : MonoBehaviour
{
    public string animationName;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(DelayAnimation());
    }

    private IEnumerator DelayAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.4f));
        anim.Play(animationName);
    }
}
