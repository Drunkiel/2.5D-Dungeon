using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public string animationName;
    public ParticleSystem particle;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        if (!string.IsNullOrEmpty(animationName)) 
            anim.Play(animationName);
    }

    public void PlayParticle()
    {
        if (particle != null)
            particle.Play();
    }
}
