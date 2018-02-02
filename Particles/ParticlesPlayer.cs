using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPlayer : MonoBehaviour {

    private ParticleSystem leftFoot = null;
    private ParticleSystem rightFoot = null;
    private ParticleSystem[] impact = new ParticleSystem[3];

    private float durationOfFootParticles = 0.1f;
    private float durationOfImpactParticle = 0.1f;

    private void Start()
    {
        leftFoot = transform.GetChild(0).transform.GetChild(0).GetComponent<ParticleSystem>();
        rightFoot = transform.GetChild(0).transform.GetChild(1).GetComponent<ParticleSystem>();
        for (int i = 0; i < 3; i++)
        {
            impact[i] = transform.GetChild(1).transform.GetChild(i).GetComponent<ParticleSystem>();
            impact[i].Stop();
        }
        

        leftFoot.Stop();
        rightFoot.Stop();

    }

    #region Left Foot particle
    public void LeftFoot_Start()
    {
        //if (!leftFoot.isPlaying)
        //{
            leftFoot.Play();
            StartCoroutine(LeftFoot_WaitForClose());
        //}
        
    }

    private IEnumerator LeftFoot_WaitForClose()
    {
        yield return new WaitForSeconds(durationOfFootParticles);
        leftFoot.Stop();
    }

    #endregion


    #region Right Foot particle
    public void RightFoot_Start()
    {
        //if (!rightFoot.isPlaying)
        //{
            rightFoot.Play();
            StartCoroutine(RightFoot_WaitToStop());
        //}
    }

    private IEnumerator RightFoot_WaitToStop()
    {
        yield return new WaitForSeconds(durationOfFootParticles);
        rightFoot.Stop();
    }
    #endregion

    #region Impact

    public void Impact_Start()
    {
        for (int i = 0; i < 3; i++)
        {
            impact[i].Play();
        }
        StartCoroutine(Impact_WaitToStop());
    }

    private IEnumerator Impact_WaitToStop()
    {
        yield return new WaitForSeconds(durationOfImpactParticle);
        for (int i = 0; i < 3; i++)
        {
            impact[i].Stop();
        }
    }
    #endregion
}
