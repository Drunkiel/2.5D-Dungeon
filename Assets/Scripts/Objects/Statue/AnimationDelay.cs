using System.Collections;
using UnityEngine;

public class AnimationDelay : MonoBehaviour
{
    public string animationName;
    public float delay = 0.4f;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(DelayAnimation());
    }

    private IEnumerator DelayAnimation()
    {
        yield return new WaitForSeconds(Random.Range(0f, delay));
        anim.Play(animationName);
    }
}
