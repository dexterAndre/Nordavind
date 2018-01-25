using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHedgehogExplosion : MonoBehaviour {

    private ParticleSystem InnerExplosion = null;
    private ParticleSystem OuterExplosion = null;


    private IEnumerator Particle_StartExplosion(float durationUntilNextPartOfParticle)
    {
        yield return new WaitForSeconds(durationUntilNextPartOfParticle);

        InnerExplosion.Stop();
        OuterExplosion.Stop();
        Destroy(this.gameObject);
    }

    void Awake () {
        InnerExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        OuterExplosion = transform.GetChild(1).GetComponent<ParticleSystem>();

        InnerExplosion.Play();
        OuterExplosion.Play();
        StartCoroutine(Particle_StartExplosion(0.5f));
    }
}
