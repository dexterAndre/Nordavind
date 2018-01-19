using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadhogMother : Actor {


    #region Spawning of Hedgehogs

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

    /// <summary>
    /// The cooldown before the mother can spawn a new snowballer.
    /// </summary>
    [SerializeField]
    private float spawnCooldown = 5f;


    /// <summary>
    /// This int defines amount of spawn each time the mother does an spawn iteration, it allows some visual delay to give it a nicer look.
    /// </summary>
    private int numberSpawned = 0;

    /// <summary>
    /// The mother spawns a snowballer, then creates a cooldown before spawning a new one.
    /// </summary>
    /// <returns></returns>
    private void Spawn_Hedgehog()
    {
        numberSpawned = 0;
        mAnimator.SetTrigger("Spawn");
        StartCoroutine(WaitForRoar(1f));
    }

    /// <summary>
    /// Spawns the particles after the roar has been initialized.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRoar(float timeBeforeNextSpawn)
    {
        yield return new WaitForSeconds(timeBeforeNextSpawn);
  
        GameObject spawnParticle = Instantiate(Spawning_effectPrefab, mSpawnPoints[numberSpawned].position, Quaternion.identity, null);
        Destroy(spawnParticle, 4f);
        numberSpawned++;

        if (numberSpawned < 3)
            StartCoroutine(WaitForRoar(1f));
    }

    #endregion

    #region Frost-breath

    /// <summary>
    /// Used to turn on and off the ground particles during the breath (damage zones).
    /// </summary>
    [Header("Breath")]
    [SerializeField]
    private GameObject[] GroundParticles = new GameObject[40];

    /// <summary>
    /// Get a refrence to the transform of the where the breath is spawned.
    /// </summary>
    private Transform mHeadTransform = null;

    /// <summary>
    /// Get a refrence to the animator of the mother.
    /// </summary>
    private SnowballerMotherAnimations motherAnimator = null;

    /// <summary>
    /// Used to find the best way of spreading the raycast used to register the lenght of ground particles.
    /// </summary>
    private float spreadSizeForRays = 3f;

    /// <summary>
    /// The particle effect spawned once the breath hit an object.
    /// <para>  Will be spawned at the raycast hit.</para>
    /// </summary>
    [SerializeField]
    private GameObject prefabForBreathHit = null;


    /// <summary>
    /// This bool is being used to check if the breath is active, once TRUE this will allow for raycast to check distance to nearest object.
    /// </summary>
    private bool breathIsBeingUsed = false;

    /// <summary>
    /// This is the VFX of the breath. This is used to fix the ground particles, and
    /// </summary>
    [SerializeField]
    private GameObject Breath_visualsPLH = null;

    private void Breath_Recharge()
    {
        StartCoroutine(Breath_Start());
        motherAnimator.StartBreath();

    }

    //------SHOULD BE REWRITTEN TO ANIMATION EVENT------
    /// <summary>
    /// This function will enable the breath visuals.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Breath_Start()
    {
        yield return new WaitForSeconds(7f);
        Breath_visualsPLH.SetActive(true);
        breathIsBeingUsed = true;
    }

    /// <summary>
    /// This public function will be called on a event in the animation to disable the breath.
    /// </summary>
    public void Breath_DisableVisuals()
    {
        Breath_visualsPLH.SetActive(false);
        breathIsBeingUsed = false;
    }

    /// <summary>
    /// This function checks the lenght to the nearest object infront of the breath. It will help with collision detection and visuals of the breath.
    /// </summary>
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
        if (Breath_visualsPLH.activeSelf)
        {
            GameObject SnowExplosionAtEnd = Instantiate(prefabForBreathHit, breathFurthesHit.point - (Vector3.forward * 3f), Quaternion.identity, null);
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

    /// <summary>
    /// This checks the distance from the raycastpoints in the Breath_CheckForLenght() - function, and enable/disable ground effects accordingly.
    /// </summary>
    /// <param name="distanceFromHead"></param>
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

    [Header("Logic / AI")]
    /// <summary>
    /// Just for checking out stuff when press.
    /// </summary>
    [SerializeField]
    private bool useLoop = false;

    private IEnumerator MotherActionsLoop()
    {
        yield return new WaitForSeconds(20f);
        if (currentStep < 2)
        {
            Spawn_Hedgehog();
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

    #region Update functions

    private void Start()
    {
        motherAnimator = GetComponent<SnowballerMotherAnimations>();
        Health_SetStandardHealth(mStartingHealth);
        GetActorComponents();

        for (int i = 1; i < 41; i++)
        {
            GroundParticles[i - 1] = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(i).gameObject;
        }
    }

    private void Awake()
    {
        mHeadTransform = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Transform>();

        for (int i = 0; i < 3; i++)
        {
            mSpawnPoints[i] = transform.GetChild(2).transform.GetChild(i).GetComponent<Transform>();
        }

        if (useLoop)
        {
            StartCoroutine(MotherActionsLoop());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Breath_Recharge();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            Spawn_Hedgehog();
        }

        if (breathIsBeingUsed)
            Breath_CheckForLenght();
    }


    #endregion
}
