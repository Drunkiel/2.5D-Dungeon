using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public string animationName;
    public ParticleSystem particle;
    public Animator anim;

    public void PlayParticle()
    {
        if (particle != null)
            particle.Play();
    }

    public void Ala()
    {
        transform.GetChild(0).GetComponent<Projectile>().Shoot();
    }
}
