using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnehetta : MonoBehaviour {

    #region
    private enum TypeOfStance
    {
        Idle,
        Spawning,
        Breathing,
        TakingDamage
    }

    private TypeOfStance mTypeOfStance = TypeOfStance.Idle;

    private enum TypeOfPhase
    {
        phase1,
        phase2,
        phase3
    }
    private TypeOfPhase mTypeOfPhase = TypeOfPhase.phase1;


#endregion


    #region Spawning of Hedgehogs

    /// <summary>
    /// The prefab used to spawn a snowballer during the encounter.
    /// </summary>
    [Header("Spawning of Snowballers")]
    [SerializeField]
    private GameObject Spawning_effectPrefab = null;

    [SerializeField]
    private Transform player = null;

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
        motherAnimator.Spawn();
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
        spawnParticle.GetComponent<SpawningHedgehogParticle>().SetTarget(player);
        spawnParticle.GetComponent<SpawningHedgehogParticle>().SetPositionAtStart(mSpawnPoints[numberSpawned].GetChild(0).transform.position);
        print("New target: " + mSpawnPoints[numberSpawned].GetChild(0).transform.position);
        Destroy(spawnParticle, 4f);
        numberSpawned++;

        if (numberSpawned < 3)
            StartCoroutine(WaitForRoar(2f));
    }

    #endregion



    #region Frost-breath


    /// <summary>
    /// Used to turn on and off the ground particles during the breath (damage zones).
    /// </summary>
    [Header("Breath")]
    [SerializeField]
    private GameObject[] GroundParticles_NegativeOuter = new GameObject[28];
    [SerializeField]
    private GameObject[] GroundParticles_NegativeInner = new GameObject[29];
    [SerializeField]
    private GameObject[] GroundParticles_Center = new GameObject[30];
    [SerializeField]
    private GameObject[] GroundParticles_PositiveInner = new GameObject[29];
    [SerializeField]
    private GameObject[] GroundParticles_PositiveOuter = new GameObject[28];
    private Transform BreathImpactLocation = null;


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
    private float spreadSizeForRays = -5f;

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

    [SerializeField]
    private Transform eyeJointTransform = null;

    private void Breath_Recharge()
    {
        motherAnimator.StartBreath();

    }

    /// <summary>
    /// This function will enable the breath visuals.
    /// </summary>
    /// <returns></returns>
    public void Breath_EnableVisuals()
    {
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
        for (int i = 0; i < 5; i++)
        {
            RaycastHit breathHit = new RaycastHit();
            Vector3 startPos = Vector3.zero;

            float distanceFromObjectHit = -5f;

            startPos = mHeadTransform.position + transform.right * spreadSizeForRays;


            Debug.DrawRay(startPos, mHeadTransform.forward * 500f, Color.red);

            if (Physics.Raycast(startPos, mHeadTransform.forward, out breathHit, Mathf.Infinity))
            {
              distanceFromObjectHit = breathHit.distance;

                if (Breath_visualsPLH.activeSelf)
                {
                    BreathImpactLocation.position = breathHit.point - transform.forward * 10f - transform.right * 3f;
                    BreathImpactLocation.LookAt(BreathImpactLocation.position + breathHit.normal);
                }
            }
            

            if (distanceFromObjectHit != 0)
                Breath_SetAmountOfGroundParticles(distanceFromObjectHit, i-2);
            else
                Breath_SetAmountOfGroundParticles(500000f, i-2);


            spreadSizeForRays += 2.5f;
        }
        spreadSizeForRays = -5f;

    }

    /// <summary>
    /// This checks the distance from the raycastpoints in the Breath_CheckForLenght() - function, and enable/disable ground effects accordingly.
    /// </summary>
    /// <param name="distanceFromHead"></param>
    private void Breath_SetAmountOfGroundParticles(float distanceFromHead, int rayIndex)
    {
        float amountProccessed = 5f;

        if (rayIndex == -2)
        {
            amountProccessed = 15f;
            for (int i = 0; i < GroundParticles_NegativeOuter.Length; i++)
            {
                if (distanceFromHead > amountProccessed)
                {
                    GroundParticles_NegativeOuter[i].SetActive(true);
                    amountProccessed += 5f;
                }
                else
                    GroundParticles_NegativeOuter[i].SetActive(false);
            }
        }
        else if (rayIndex == -1)
        {
            amountProccessed = 10f;
            for (int i = 0; i < GroundParticles_NegativeInner.Length; i++)
            {
                if (distanceFromHead > amountProccessed)
                {
                    GroundParticles_NegativeInner[i].SetActive(true);
                    amountProccessed += 5f;
                }
                else
                    GroundParticles_NegativeInner[i].SetActive(false);
            }
        }
        else if (rayIndex == 0)
        {
            amountProccessed = 5f;
            for (int i = 0; i < GroundParticles_Center.Length; i++)
            {
                if (distanceFromHead > amountProccessed)
                {
                    GroundParticles_Center[i].SetActive(true);
                    amountProccessed += 5f;
                }
                else
                    GroundParticles_Center[i].SetActive(false);
            }
        }
        else if (rayIndex == 1)
        {
            amountProccessed = 10f;
            for (int i = 0; i < GroundParticles_PositiveInner.Length; i++)
            {
                if (distanceFromHead > amountProccessed)
                {
                    GroundParticles_PositiveInner[i].SetActive(true);
                    amountProccessed += 5f;
                }
                else
                    GroundParticles_PositiveInner[i].SetActive(false);
            }
        }
        else if (rayIndex == 2)
        {
            amountProccessed = 15f;
            for (int i = 0; i < GroundParticles_PositiveOuter.Length; i++)
            {
                if (distanceFromHead > amountProccessed)
                {
                    GroundParticles_PositiveOuter[i].SetActive(true);
                    amountProccessed += 5f;
                }
                else
                    GroundParticles_PositiveOuter[i].SetActive(false);
            }


        }
    }


    #endregion

    #region Phases

    public void NextPhase()
    {
        print("Took damage!");

        if (mTypeOfPhase == TypeOfPhase.phase1)
        {
            mTypeOfPhase = TypeOfPhase.phase2;
            motherAnimator.Health_GetHit();
            
        }
        else if (mTypeOfPhase == TypeOfPhase.phase2)
        {
            mTypeOfPhase = TypeOfPhase.phase3;
            motherAnimator.Health_GetHit();
        }
        else if (mTypeOfPhase == TypeOfPhase.phase3)
        {
            print("###SNEHETTA### just died!");
            motherAnimator.Health_Dying();
            Destroy(this.gameObject, 5f);
        }
    }

#endregion

    #region Mother logic / AI

    private int currentStep = 0;

    private float durationUntilNextMove = 10f;

    [Header("Logic / AI")]
    /// <summary>
    /// Just for checking out stuff when press.
    /// </summary>
    [SerializeField]
    private bool useLoop = false;

    private IEnumerator MotherActionsLoop(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (currentStep < 2)
        {
            if (mTypeOfPhase == TypeOfPhase.phase1)
            {
                durationUntilNextMove = 12f;
            }
            else if (mTypeOfPhase == TypeOfPhase.phase2)
            {
                durationUntilNextMove = 10f;
            }
            else if (mTypeOfPhase == TypeOfPhase.phase3)
            {
                durationUntilNextMove = 8f;
            }
            
            Spawn_Hedgehog();
            currentStep++;
        }
        else
        {
            if (mTypeOfPhase == TypeOfPhase.phase1)
            {
                durationUntilNextMove = 25f;
            }
            else if (mTypeOfPhase == TypeOfPhase.phase2)
            {
                durationUntilNextMove = 23f;
            }
            else if (mTypeOfPhase == TypeOfPhase.phase3)
            {
                durationUntilNextMove = 20f;
            }
            Breath_Recharge();
            currentStep = 0;
        }
        StartCoroutine(MotherActionsLoop(durationUntilNextMove));
    }

#endregion

    #region Update functions

    private void Start()
    {
        motherAnimator = transform.GetChild(0).GetComponent<SnowballerMotherAnimations>();

        for (int i = 0; i < 30; i++)
        {
            if (i == 0)
            {
                GroundParticles_Center[i] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(0).gameObject;
            }
            else if (i == 1)
            {
                GroundParticles_NegativeInner[i-1] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(0).gameObject;
                GroundParticles_Center[i] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(1).gameObject;
                GroundParticles_PositiveInner[i-1] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(2).gameObject;
            }
            else
            {
                GroundParticles_NegativeOuter[i-2] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(0).gameObject;
                GroundParticles_NegativeInner[i-1] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(1).gameObject;

                GroundParticles_Center[i] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(2).gameObject;

                GroundParticles_PositiveInner[i-1] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(3).gameObject;
                GroundParticles_PositiveOuter[i-2] = Breath_visualsPLH.transform.transform.GetChild(i).transform.GetChild(4).gameObject;
            }
        }
    }

    private void Awake()
    {
        mHeadTransform = transform.GetChild(2).transform.GetChild(0).GetComponent<Transform>();
        BreathImpactLocation = mHeadTransform.GetChild(0).transform.GetChild(31);

        for (int i = 0; i < 3; i++)
        {
            mSpawnPoints[i] = transform.GetChild(3).transform.GetChild(i).GetComponent<Transform>();
        }

        if (useLoop)
        {
            StartCoroutine(MotherActionsLoop(4f));
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


        if (Breath_visualsPLH.activeSelf)
        {
            mHeadTransform.position = new Vector3(eyeJointTransform.position.x, mHeadTransform.position.y, eyeJointTransform.position.z);

            mHeadTransform.rotation = new Quaternion(mHeadTransform.rotation.x, eyeJointTransform.rotation.y, mHeadTransform.rotation.z, mHeadTransform.rotation.w);
            
        }
    }


    #endregion
}
