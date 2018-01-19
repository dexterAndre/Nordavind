﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnowballerMother : Actor {

    #region Spawn

    /// <summary>
    /// The prefab used to spawn a snowballer during the encounter.
    /// </summary>
    [Header("Spawning of Snowballers")]
    [SerializeField]
    private GameObject Spawning_effectPrefab = null;

    /// <summary>
    /// The spawnPoints used to set the new snowballers out during the encounter.
    /// </summary>
    private Transform[] mSpawnPoints = new Transform[3];

    private int amountSpawnedByMother = 0;


    /// <summary>
    /// The cooldown before the mother can spawn a new snowballer.
    /// </summary>
    [SerializeField]
    private float spawnCooldown = 5f;

    /// <summary>
    /// While TRUE the mother cannot spawn a new snowballer, while FALSE the mother can spawn a new snowballer.
    /// </summary>
    private bool haveSpawnedNewSnowballer = false;

    /// <summary>
    /// The mother spawns a snowballer, then creates a cooldown before spawning a new one.
    /// </summary>
    /// <returns></returns>
    private void Spawn_Snowballer()
    {
        mAnimator.SetTrigger("Spawn");
        StartCoroutine(WaitForRoar());
    }

    private IEnumerator WaitForRoar()
    {
        yield return new WaitForSeconds(4f);
        for (int i = 0; i < mSpawnPoints.Length; i++)
        {
            GameObject spawnParticle = Instantiate(Spawning_effectPrefab, mSpawnPoints[i].position, Quaternion.identity, null);
            Destroy(spawnParticle, 4f);

        }
    }

    #endregion

    #region FrostBeam
    [Header("Breath")]
    [SerializeField]
    private GameObject[] GroundParticles = new GameObject[40];

    private bool breathIsBeingUsed = false;

    private Transform mHeadTransform = null;

    private SnowballerMotherAnimations mAnimations = null;

    private bool breathStarted = false;

    [SerializeField]
    private float spreadSizeForRays = 3f;

    [SerializeField]
    private GameObject prefabForBreathHit = null;
    [SerializeField]
    private float cooldownForBreathHit = 0.5f;
    private bool isBreathHitOnCooldown = false;

    [SerializeField]
    private GameObject PLH_BreathVisuals = null;

    private void Breath_Recharge()
    {
        StartCoroutine(Breath_Start());
        mAnimations.StartBreath();

    }

    private IEnumerator Breath_Start()
    {
        yield return new WaitForSeconds(7f);
        PLH_BreathVisuals.SetActive(true);
        breathIsBeingUsed = true;
    }

    public void Breath_DisableVisuals()
    {
        PLH_BreathVisuals.SetActive(false);
    }



    private void Breath_CheckForLenght()
    {
        float distanceFromObjectHit = 0f;
        RaycastHit breathFurthesHit = new RaycastHit();

        for (int i = 0; i < 3; i++)
        {
            RaycastHit breathHit = new RaycastHit();
            Vector3 startPos = Vector3.zero;

            if (i == 0)
                startPos = mHeadTransform.position + (mHeadTransform.forward * spreadSizeForRays);
            else if (i == 1)
                startPos = mHeadTransform.position;
            else if (i == 2)
                startPos = mHeadTransform.position - (mHeadTransform.forward * spreadSizeForRays);

            startPos += Vector3.up * 3;
            
            if (Physics.Raycast(startPos, -mHeadTransform.right, out breathHit, Mathf.Infinity))
            {

                if (i == 0 || distanceFromObjectHit > breathHit.distance)
                {
                    distanceFromObjectHit = breathHit.distance;
                }
                if (i == 2)
                    breathFurthesHit = breathHit;
            }
        }
        if (PLH_BreathVisuals.activeSelf)
        {
            GameObject SnowExplosionAtEnd = Instantiate(prefabForBreathHit, breathFurthesHit.point - (Vector3.forward *3f), Quaternion.identity, null);
            SnowExplosionAtEnd.transform.LookAt(SnowExplosionAtEnd.transform.position + breathFurthesHit.normal);
            Destroy(SnowExplosionAtEnd, 2f);
        }

        if (distanceFromObjectHit < 20f)
            print(distanceFromObjectHit + " is the distance.");
        if (distanceFromObjectHit != 0)
            Breath_SetAmountOfGroundParticles(distanceFromObjectHit);
        else
            Breath_SetAmountOfGroundParticles(500000f);

    }

    private void Breath_SetAmountOfGroundParticles(float distanceFromHead)
    {
        float amountProccessed = 10f;

        for (int i = 0; i < 40; i++)
        {
            if (distanceFromHead > amountProccessed)
            {
                GroundParticles[i].SetActive(true);
                amountProccessed += 4.9f;
            }
            else
                GroundParticles[i].SetActive(false);
        }
    }

    #endregion

    #region Mother logic / AI

    private int currentStep = 0;



    private IEnumerator MotherActionsLoop()
    {
        yield return new WaitForSeconds(15f);
        if (currentStep < 2) {
            Spawn_Snowballer();
            currentStep++;
        }
        else
        {
            Breath_Recharge();
            currentStep = 0;
        }
        StartCoroutine(MotherActionsLoop());
    }

    

#endregion


    #region UpdateFunctions

    private void Start()
    {
        mAnimations = GetComponent<SnowballerMotherAnimations>();
        Health_SetStandardHealth(mStartingHealth);
        GetActorComponents();

        for (int i = 1; i < 41; i++)
        {
            GroundParticles[i - 1] = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject;
        }
    }

    private void Awake ()
    {
        mHeadTransform = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Transform>();

        for (int i = 0; i < 3; i++)
        {
            mSpawnPoints[i] = transform.GetChild(2).transform.GetChild(i).GetComponent<Transform>();
        }

        StartCoroutine(MotherActionsLoop());
	}
	
	private void Update ()
    {
        if (breathIsBeingUsed)
            Breath_CheckForLenght();


    }
#endregion
}
