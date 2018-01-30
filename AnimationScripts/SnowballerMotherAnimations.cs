using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballerMotherAnimations : MonoBehaviour {


    private Animator mAnimator = null;
    private GameObject crystalCollisionBox = null;

	void Start () {
        mAnimator = GetComponent<Animator>();
        transform.parent.GetComponent<EnemyHeadhogMother>().Breath_DisableVisuals();

    }

    #region Death

    [SerializeField]
    private GameObject prefabForDeathParticles = null;

    public void Death_SpawnParticles()
    {
        GameObject deathParticle = Instantiate(prefabForDeathParticles, transform.position, Quaternion.identity, null);
        Destroy(deathParticle, 2f);
    }

#endregion

    #region Spawning

    public void Spawn()
    {
        mAnimator.SetTrigger("Spawn");
    }
    #endregion

    #region Breath

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

#endregion

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

    #region crystal
    public void Snehetta_ActivateCrystalCollider()
    {
        crystalCollisionBox.SetActive(true);
    }


    public void Snehetta_DeactivateCrystalCollider()
    {
        crystalCollisionBox.SetActive(false);
    }


    #endregion

    #region Taking Damage

    public void Health_GetHit()
    {
        mAnimator.SetTrigger("GotHit");
    }

    public void Health_Dying()
    {
        mAnimator.SetTrigger("Dead");
    }

#endregion

    private void Awake()
    {
        inhaleParticle = transform.parent.transform.GetChild(3).gameObject;
        inhaleParticle.SetActive(false);


        crystalCollisionBox = transform.parent.transform.GetChild(4).gameObject;
        crystalCollisionBox.SetActive(false);
    }
}
