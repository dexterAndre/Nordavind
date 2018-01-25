using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballerMotherAnimations : MonoBehaviour {


    private Animator mAnimator = null;

	void Start () {
        mAnimator = GetComponent<Animator>();
        transform.parent.GetComponent<EnemyHeadhogMother>().Breath_DisableVisuals();
    }

    public void StartBreath()
    {
        mAnimator.SetTrigger("Breath");
    }

    public void StartBreathVisuals()
    {
        transform.parent.GetComponent<EnemyHeadhogMother>().Breath_EnableVisuals();
    }

    public void StopBreath()
    {
        transform.parent.GetComponent<EnemyHeadhogMother>().Breath_DisableVisuals();
    }

    #region Inhale
    private GameObject inhaleParticle = null;

    public void Inhale_StartInhale()
    {
        inhaleParticle.SetActive(true);
    }

    public void Inhale_EndInhale()
    {
        inhaleParticle.SetActive(false);
    }


    #endregion

    private void Awake()
    {
        inhaleParticle = transform.parent.transform.GetChild(3).gameObject;
        inhaleParticle.SetActive(false);
    }
}
