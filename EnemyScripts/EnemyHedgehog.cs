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

    private NavMeshAgent mNavMeshAgent = null;

    private HedgehogAnimations mAnimations = null;

    [Header("Data - constants")]
    [SerializeField]
    private float mDetectionRange = 100f;

    [SerializeField]
    private float mSprintingRange = 25f;

    [SerializeField]
    private float mSelfDestructRange = 5f;

    [SerializeField]
    private float mNormalSpeed = 10f;

    [SerializeField]
    private float mSprintingSpeed = 20f;

    #endregion

    #region Data - dynamic

    [Header("Data - dynamic")]
    [SerializeField]
    private Transform mTargetToFollow = null;

    [SerializeField]
    private Vector3 mIdleTarget = Vector3.zero;



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

        float distanceToTarget  = (mTargetToFollow.position - transform.position).magnitude;

        if (distanceToTarget < mDetectionRange && mCurrentStance == TypeOfStances.Idle)
        {
            if (TargetInSight()) {
                mCurrentStance = TypeOfStances.Following;
            }
                
        }
        else if (distanceToTarget < mSprintingRange && mCurrentStance == TypeOfStances.Following)
        {
            if (TargetInSight())
                mCurrentStance = TypeOfStances.Screaming;
            else
                mCurrentStance = TypeOfStances.Idle;
            
        }
        else if (distanceToTarget < mSelfDestructRange && mCurrentStance == TypeOfStances.Sprinting)
        {
            mCurrentStance = TypeOfStances.ChargingUp;
        }

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
            mNavMeshAgent.speed = mSprintingSpeed;
        }
    }

    public void AnimationEvent_SetStateBackToIdle()
    {
        mCurrentStance = TypeOfStances.Idle;
        gotScared = false;
        mNavMeshAgent.speed = mNormalSpeed;
    }
    #endregion

    #region Following

    /// <summary>
    /// What happens during the follow-state.
    /// </summary>
    private void Actions_Following()
    {
        mNavMeshAgent.SetDestination(mTargetToFollow.position);

        if(mNavMeshAgent.speed != mNormalSpeed)
            mNavMeshAgent.speed = mNormalSpeed;

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
    private float durationOfScream = 2f;

    private bool screamStarted = false;

    /// <summary>
    /// What happens during the screaming state.
    /// </summary>
    private void Actions_Screaming()
    {
        if (!screamStarted)
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
                mCurrentStance = TypeOfStances.Sprinting;
            }
               
            else
            {
                mCurrentStance = TypeOfStances.Idle;
                mAnimations.Animation_SetSprintState(false);
                durationOfScream = 1f;
                screamStarted = false;
            }
        }


    }


    #endregion

    #region Sprinting

    /// <summary>
    /// What happens during the sprinting-state.
    /// </summary>
    private void Actions_Sprinting()
    {
        mNavMeshAgent.SetDestination(mTargetToFollow.position);

        if (mNavMeshAgent.speed != mSprintingSpeed)
        {
            mNavMeshAgent.speed = mSprintingSpeed;
        }
            


    }
    #endregion

    #region ChargingUp


    [Header("Charging Up")]
    [SerializeField]
    private float durationOfChargingUp = 1.3f;

    [SerializeField]
    private float chargingUpSpeed = 5f;

    /// <summary>
    /// What happens during the chargingUp-state.
    /// </summary>
    private void Actions_ChargingUp()
    {
        transform.LookAt(new Vector3(mTargetToFollow.position.x, transform.position.y, mTargetToFollow.position.z));
        if (mNavMeshAgent.speed != chargingUpSpeed) {
            mNavMeshAgent.acceleration = 100f;
            mNavMeshAgent.speed = 0f;
            mAnimations.Animation_InRange();
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
    private bool ShouldGizmosBeDrawn = true;
    void OnDrawGizmos()
    {
        if (ShouldGizmosBeDrawn)
        {
            Gizmos.color = Color.yellow;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawSphere(transform.position, mDetectionRange);

            Gizmos.color = Color.blue;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawSphere(transform.position, mSprintingRange);

            Gizmos.color = Color.green;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
            Gizmos.DrawWireSphere(transform.position, mSelfDestructRange);

            Gizmos.color = Color.red;
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.3f);
            Gizmos.DrawWireSphere(transform.position, damageZoneRadius);
        }
    }

    #endregion


    private void Awake()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
        mAnimations = transform.GetChild(0).GetComponent<HedgehogAnimations>();
    }

    private void Update()
    {
        WhatToDo(); 
    }

}
