using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHedgehog : MonoBehaviour {

    #region Stances

    public enum TypeOfStances
    {
        Idle,
        Scared,
        Searching,
        Following,
        Screaming,
        Sprinting,
        Recharging,
        ChargingUp,
        Exploding
    };

    /// <summary>
    /// Current active stance, this will tell you if you're idle, following a target or attacking.
    /// </summary>
    private TypeOfStances mCurrentStance = TypeOfStances.Idle;

    public TypeOfStances GetCurrentStance() {   return mCurrentStance; }

    public void SetCurrentStance(TypeOfStances newStance) { mCurrentStance = newStance; }

    #endregion

    #region Data - constants
    [SerializeField]
    private EnemyWithNavigation hedgehogVariables = null;

    private NavMeshAgent mNavMeshAgent = null;

    private GameObject damagingAura = null;

    private GameObject rechargingParticleEffect = null;

    private HedgehogAnimations mAnimations = null;

    [Header("Data - constants")]

    [Range(1f, 2f)]
    [SerializeField]
    private float sprintingTargetMultiplier = 1.5f;

    #endregion

    #region Data - dynamic

    [Header("Data - dynamic")]
    [SerializeField]
    private Transform mTargetToFollow = null;

    [SerializeField]
    private Vector3 mIdleTarget = Vector3.zero;

    [Header("DEBUGING")]
    [SerializeField]
    private Vector3 sprintLocationGoal = Vector3.zero;

    private Vector3 targetForwardWithInput = Vector3.zero;

    private bool isDying = false;

    #endregion

    #region Searching for enemies
    [SerializeField]
    private LayerMask playerLayer;

    private bool TargetInSight()
    {
        RaycastHit lineOfSightCheck;

        Vector3 direction = (mTargetToFollow.position - transform.position).normalized;
        Debug.DrawRay(transform.position, direction*100f, Color.yellow);

        

        if (Physics.Raycast(transform.position, direction, out lineOfSightCheck, Mathf.Infinity))
        {
            if(lineOfSightCheck.collider.tag == "Player")
                return true;    
        }
        return false;
    }

    public void SetTargetToFollow(Transform newTarget)
    {
        mTargetToFollow = newTarget;
    }

    public void SetIdleTarget(Vector3 position)
    {
        mIdleTarget = position;
        mNavMeshAgent.SetDestination(mIdleTarget);
    }


    private void CheckRangeOfTarget()
    {
        if (!isDying)
        {
            float distanceToTarget = (mTargetToFollow.position - transform.position).magnitude;

            if (distanceToTarget < hedgehogVariables.detectionRange && mCurrentStance == TypeOfStances.Idle)
            {
                if (TargetInSight())
                {
                    mCurrentStance = TypeOfStances.Following;
                }

            }
            else if (distanceToTarget < hedgehogVariables.combatRange && mCurrentStance == TypeOfStances.Following)
            {
                if (TargetInSight())
                    mCurrentStance = TypeOfStances.Screaming;
                else
                    mCurrentStance = TypeOfStances.Idle;

            }
            else if ((sprintLocationGoal - transform.position).magnitude < 3f && mCurrentStance == TypeOfStances.Sprinting)
            {
                mCurrentStance = TypeOfStances.Recharging;
            }
        }


    }

    #endregion

    #region Movement - combat


    private void SetTargetToMoveTowards()
    {

        if (mTargetToFollow.GetComponent<PlayerMovement>().GetState() == PlayerMovement.State.Walk && hedgehogVariables.useForwardPrediction)
        {
            targetForwardWithInput = new Vector3(
                mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().x,
                0f,
                mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().z)
                * hedgehogVariables.forwardPredrictionSensetivity;
        }
        else
            targetForwardWithInput = Vector3.zero;


        sprintLocationGoal =
            (mTargetToFollow.position - transform.position).normalized * 1.5f
            + mTargetToFollow.position
            + targetForwardWithInput * hedgehogVariables.forwardPredrictionSensetivity;

        sprintLocationGoal = new Vector3(sprintLocationGoal.x, mTargetToFollow.position.y, sprintLocationGoal.z);

    }
    #endregion


    #region ---Actions---

    private void WhatToDo()
    {



        if (mCurrentStance == TypeOfStances.Idle)
        {
            Actions_Idle();
        }
        else if (mCurrentStance == TypeOfStances.Scared)
        {
            Actions_Scared();
        }
        else if (mCurrentStance == TypeOfStances.Following)
        {
            Actions_Following();
        }
        else if (mCurrentStance == TypeOfStances.Screaming)
        {
            Actions_Screaming();
        }
        else if (mCurrentStance == TypeOfStances.Sprinting)
        {
            Actions_Sprinting();
        }
        else if (mCurrentStance == TypeOfStances.Recharging)
        {
            Actions_Recharging();
        }
        else if (mCurrentStance == TypeOfStances.ChargingUp)
        {
            Actions_ChargingUp();
        }
        else if (mCurrentStance == TypeOfStances.Exploding)
        {
            Actions_Exploding();
        }

        //Checks the range of the target, then sets the correct stance accordingly.
        CheckRangeOfTarget();
    }


    #endregion

    #region Idle

    /// <summary>
    /// What happens during the idle state.
    /// </summary>
    private void Actions_Idle()
    {
        //Enveiroment behavior here, will become rather complex after a while.
        if (mNavMeshAgent.acceleration != hedgehogVariables.normalAcceleration)
            mNavMeshAgent.acceleration = hedgehogVariables.normalAcceleration;
        if (mNavMeshAgent.speed != hedgehogVariables.normalSpeed)
            mNavMeshAgent.speed = hedgehogVariables.normalSpeed;
        mNavMeshAgent.SetDestination(mIdleTarget);
    }

    #endregion

    #region Scared

    bool gotScared = false;
    float timeUntilRunScared = 0.25f;
    private void Actions_Scared()
    {
        if (!gotScared)
        {
            mAnimations.Animation_GotHit();
            mNavMeshAgent.SetDestination(transform.position + (transform.forward * 30f));
            mNavMeshAgent.speed = 0f;
            timeUntilRunScared = 0.25f;
            gotScared = true;
        }
        else if (timeUntilRunScared > 0f)
        {
            timeUntilRunScared -= 1 * Time.deltaTime;
        }
        else
        {
            mNavMeshAgent.speed = hedgehogVariables.combatSpeed;
        }
    }

    public void AnimationEvent_SetStateBackToIdle()
    {
        mCurrentStance = TypeOfStances.Idle;
        gotScared = false;
        mNavMeshAgent.speed = hedgehogVariables.normalSpeed;   
    }
    #endregion

    #region Following

    /// <summary>
    /// What happens during the follow-state.
    /// </summary>
    private void Actions_Following()
    {
        mNavMeshAgent.SetDestination(mTargetToFollow.position);

        if(mNavMeshAgent.speed != hedgehogVariables.normalSpeed)
            mNavMeshAgent.speed = hedgehogVariables.normalSpeed;

        //Debugging if player ever LOS the target.
        if (!TargetInSight())
        {
            mCurrentStance = TypeOfStances.Idle;
        }
    }

    #endregion

    #region Screaming

    [Header("Scream")]
    [SerializeField]
    private float durationOfScream = 4f;

    private bool screamStarted = false;

    /// <summary>
    /// What happens during the screaming state.
    /// </summary>
    private void Actions_Screaming()
    {
        transform.LookAt(new Vector3(mTargetToFollow.position.x, transform.position.y, mTargetToFollow.position.z));
        if (!screamStarted && TargetInSight())
        {
            mAnimations.Animation_StartScream();
            mNavMeshAgent.SetDestination(transform.position);
            screamStarted = true;
        }        

        if (durationOfScream >= 0f)
        {
            durationOfScream -= Time.deltaTime;
        }
        else
        {
            if (TargetInSight())
            {
                mAnimations.Animation_SetSprintState(true);
                SetTargetToMoveTowards();
                durationOfScream = 2f;
                mCurrentStance = TypeOfStances.Sprinting;
            }
               
            else
            {
                mCurrentStance = TypeOfStances.Idle;
                mAnimations.Animation_SetSprintState(false);
                durationOfScream = 2f;
                screamStarted = false;
            }
        }


    }


    #endregion

    #region Sprinting

    private float durationOfSprint = 2f;

    /// <summary>
    /// What happens during the sprinting-state.
    /// </summary>
    private void Actions_Sprinting()
    {
        if (mNavMeshAgent.speed != hedgehogVariables.combatSpeed)
        {
            damagingAura.SetActive(true);

            if (mNavMeshAgent.acceleration != hedgehogVariables.combatAcceleration)
                mNavMeshAgent.acceleration = hedgehogVariables.combatAcceleration;

            if (mNavMeshAgent.speed != hedgehogVariables.combatSpeed)
                mNavMeshAgent.speed = hedgehogVariables.combatSpeed;


            durationOfSprint = (mTargetToFollow.transform.position - transform.position).magnitude * Time.deltaTime;
            durationOfSprint += 1f;
            print(durationOfSprint + " - SprintDuration");

        }

        if (durationOfSprint > 0)
        {
            SetTargetToMoveTowards(); //Setting target

            mNavMeshAgent.SetDestination(sprintLocationGoal);

            durationOfSprint -= 1 * Time.deltaTime;
        }
        else
        {
            mCurrentStance = TypeOfStances.Recharging;
        }


    }
    #endregion

    #region Recharging

    [Header("Recharging")]
    [SerializeField]
    private float durationOfRecharge = 2f;

    private void Actions_Recharging()
    {
        if (mNavMeshAgent.speed != 0f)
        {
            durationOfRecharge = 3f;
            damagingAura.SetActive(false);
            rechargingParticleEffect.SetActive(true);
            mNavMeshAgent.acceleration = 100f;
            mNavMeshAgent.speed = 0f;
            mAnimations.Animation_Recharging();
        }


        if (durationOfRecharge >= 0f)
        {
            //transform.LookAt(new Vector3(mTargetToFollow.position.x, transform.position.y, mTargetToFollow.position.z));
            durationOfRecharge -= Time.deltaTime;
        }
        else
        {
            SetTargetToMoveTowards();
            rechargingParticleEffect.SetActive(false);
            mCurrentStance = TypeOfStances.Sprinting;
        }
    }


#endregion

    #region ChargingUp


    [Header("Charging Up")]
    [SerializeField]
    private float durationOfChargingUp = 1.3f;

    /// <summary>
    /// What happens during the chargingUp-state.
    /// </summary>
    private void Actions_ChargingUp()
    {
        damagingAura.SetActive(false);
        rechargingParticleEffect.SetActive(false);
        if (mNavMeshAgent.speed != 0f) {
            mNavMeshAgent.acceleration = 100f;
            mNavMeshAgent.speed = 0f;
            mAnimations.Animation_StartExplosion();
            isDying = true;
        }
            

        if (durationOfChargingUp >= 0f)
        {
            durationOfChargingUp -= Time.deltaTime;
        }
        else
        {
            mCurrentStance = TypeOfStances.Exploding;
        }
    }



    #endregion

    #region Exploding

    [Header("Exploding")]
    [SerializeField]
    private GameObject mExplosionParticle = null;

    [SerializeField]
    private float damageZoneRadius = 10f;



    /// <summary>
    /// What happens during the exploding-state.
    /// </summary>
    private void Actions_Exploding()
    {
        GameObject hedgehogDeathExplosion = Instantiate(mExplosionParticle, transform.position, Quaternion.identity, null);
        DealDamageToEnemy();
        Destroy(hedgehogDeathExplosion, 1f);
        Destroy(this.gameObject);
    }    

    private void DealDamageToEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageZoneRadius, playerLayer);
        for (uint i = 0; i < hitColliders.Length - 1; i++)
        {
            if (hitColliders[i].tag == "Player" && hitColliders[i].transform.parent == null)
            {
                print("Would have hit" + hitColliders[i].name);
            }
        }
    }

    #endregion

    #region Gizmos
    [Header("Gizmos")]
    [SerializeField]
    public bool ShouldGizmosBeDrawn = true;

    private void OnDrawGizmosSelected()
    {
        Ray forwardCrashingRay = new Ray(transform.position, transform.forward);
        Debug.DrawRay(forwardCrashingRay.origin, forwardCrashingRay.direction * 4f, Color.red);
    }
    void OnDrawGizmos()
    {

        if (ShouldGizmosBeDrawn)
        {
            Gizmos.color = Color.yellow;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawSphere(transform.position, hedgehogVariables.detectionRange);

            Gizmos.color = Color.blue;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawSphere(transform.position, hedgehogVariables.combatRange);

            Gizmos.color = Color.green;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
            Gizmos.DrawWireSphere(transform.position, hedgehogVariables.attackingRange);

            Gizmos.color = Color.red;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
            Gizmos.DrawWireSphere(transform.position, damageZoneRadius);
        }
    }

    #endregion

    #region MoreDebugging
    [Header("Debugging for direction of hedgehog")]
    [SerializeField]
    private LineRenderer debugingLine = null;
    [SerializeField]
    private bool useDebugingLinesForDirection = false;
    private void Debuging_drawLineForDirection()
    {
        debugingLine.SetPosition(0, transform.position);

        sprintLocationGoal = new Vector3(sprintLocationGoal.x, mTargetToFollow.position.y, sprintLocationGoal.z);

        Vector3 newPos =
            (mTargetToFollow.position - transform.position).normalized * 1.5f
            + mTargetToFollow.position +
            new Vector3(mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().x,
            0f,
            mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().z
            *hedgehogVariables.forwardPredrictionSensetivity);

        newPos = new Vector3(newPos.x, mTargetToFollow.position.y, newPos.z);

        debugingLine.SetPosition(1, newPos);
    }

#endregion

    #region Update functions

    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mAnimations = transform.GetChild(0).GetComponent<HedgehogAnimations>();
        damagingAura = transform.GetChild(1).gameObject;
        rechargingParticleEffect = transform.GetChild(2).gameObject;
        rechargingParticleEffect.SetActive(false);
        damagingAura.SetActive(false);

        if (mTargetToFollow == null)
            mTargetToFollow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Update()
    {
        WhatToDo();

        if(useDebugingLinesForDirection)
            Debuging_drawLineForDirection();
    }

#endregion

}
