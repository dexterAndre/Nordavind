using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNisse : MonoBehaviour {

    #region Stances
    public enum TypeOfStance
    {
        Idle,
        Following,
        InitializingAttack,
        RePosition,
        Throwing,
        Retreating,
        Reloading,
        TakingDamage,
        Dying
    }

    public TypeOfStance mCurrentStance = TypeOfStance.Idle;

    #endregion


    #region Static-variables
    [SerializeField]
    private EnemyWithNavigation nisseVariables = null;

    private NavMeshAgent mNavMeshAgent = null;

    [SerializeField]
    private GameObject prefab_snowball = null;

    private Transform mTargetToFollow = null;
    #endregion


    #region Dynamic - variables

    private Vector3 mIdleTarget = Vector3.zero;

    private bool isDying = false;

    private Vector3 throwHitLocation = Vector3.zero;

    private Vector3 targetForwardWithInput = Vector3.zero;

    #endregion


    #region Searching for enemies
    [SerializeField]
    private LayerMask playerLayer;

    private bool TargetInSight()
    {
        RaycastHit lineOfSightCheck;

        Vector3 direction = (mTargetToFollow.position - transform.position).normalized;
        Debug.DrawRay(transform.position, direction * 100f, Color.yellow);



        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out lineOfSightCheck, Mathf.Infinity))
        {
            if (lineOfSightCheck.collider.tag == "Player")
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

            if (distanceToTarget < nisseVariables.detectionRange && mCurrentStance == TypeOfStance.Idle)
            {
                if (TargetInSight())
                {
                    mCurrentStance = TypeOfStance.Following;
                }
            }
            else if (distanceToTarget < nisseVariables.combatRange && mCurrentStance == TypeOfStance.Following)
            {
                if (TargetInSight())
                    mCurrentStance = TypeOfStance.InitializingAttack;
                else
                    mCurrentStance = TypeOfStance.Idle;

            }
        }
    }


    #endregion

    #region Combat


    private void SetLocationToThrowAt()
    {

        if (mTargetToFollow.GetComponent<PlayerMovement>().GetState() == PlayerMovement.State.Walk && nisseVariables.useForwardPrediction)
        {
            targetForwardWithInput = new Vector3(
                mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().x,
                0f,
                mTargetToFollow.GetComponent<PlayerMovement>().GetMovementVector().z)
                * nisseVariables.forwardPredrictionSensetivity;
        }
        else
            targetForwardWithInput = Vector3.zero;


        throwHitLocation = mTargetToFollow.position + targetForwardWithInput * nisseVariables.forwardPredrictionSensetivity;

        throwHitLocation = new Vector3(throwHitLocation.x, mTargetToFollow.GetChild(0).transform.position.y + 0.1f, throwHitLocation.z);

    }
    #endregion


    #region --Action initialization--

    private void WhatToDo()
    {
        if (mCurrentStance == TypeOfStance.Idle)
        {
            Actions_Idle();

            CheckRangeOfTarget();
        }
        else if (mCurrentStance == TypeOfStance.Following)
        {
            Actions_Following();

            CheckRangeOfTarget();
        }
        else if (mCurrentStance == TypeOfStance.InitializingAttack)
        {
            Actions_InitializingAttack();
        }
        else if (mCurrentStance == TypeOfStance.RePosition)
        {
            Actions_RePosition();
        }
        else if (mCurrentStance == TypeOfStance.Throwing)
        {
            Actions_Throwing();
        }
        else if (mCurrentStance == TypeOfStance.Retreating)
        {
            Actions_Retreating();
        }
        else if (mCurrentStance == TypeOfStance.Reloading)
        {
            Actions_Reloading();
        }
        else if (mCurrentStance == TypeOfStance.TakingDamage)
        {
            Actions_TakingDamage();
        }
        else if (mCurrentStance == TypeOfStance.Dying)
        {
            Actions_Dying();
        }

    }
    #endregion


    #region Actions

    #region Idle
    private void Actions_Idle()
    {
        //Enveiroment behavior here, will become rather complex after a while.

        if (mNavMeshAgent.acceleration != nisseVariables.normalAcceleration)
            mNavMeshAgent.acceleration = nisseVariables.normalAcceleration;

        if (mNavMeshAgent.speed != nisseVariables.normalSpeed)
            mNavMeshAgent.speed = nisseVariables.normalSpeed;

        mNavMeshAgent.SetDestination(mIdleTarget);
    }

    #endregion

    #region Following
    private void Actions_Following()
    {
        mNavMeshAgent.SetDestination(mTargetToFollow.position);

        if (mNavMeshAgent.speed != nisseVariables.normalSpeed)
            mNavMeshAgent.speed = nisseVariables.normalSpeed;
        if (mNavMeshAgent.speed != nisseVariables.normalAcceleration)
            mNavMeshAgent.speed = nisseVariables.normalAcceleration;

        //Debugging if player ever LOS the target.
        if (!TargetInSight())
        {
            mCurrentStance = TypeOfStance.Idle;
        }
    }

    #endregion

    #region Intializing Attack

    [Header("Scream")]
    [SerializeField]
    private float durationOfScream = 4f;

    private bool screamStarted = false;


    private void Actions_InitializingAttack()
    {
        if (mNavMeshAgent.acceleration != nisseVariables.combatAcceleration)
            mNavMeshAgent.acceleration = nisseVariables.combatAcceleration;

        if (mNavMeshAgent.speed != nisseVariables.combatSpeed)
            mNavMeshAgent.speed = nisseVariables.combatSpeed;


        transform.LookAt(new Vector3(mTargetToFollow.position.x, transform.position.y, mTargetToFollow.position.z));
        if (!screamStarted && TargetInSight())
        {
            //mAnimations.Animation_StartScream();
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
                //mAnimations.Animation_SetSprintState(true);
                SetLocationToThrowAt();
                durationOfScream = 2f;
                mCurrentStance = TypeOfStance.Throwing;
            }

            else
            {
                mCurrentStance = TypeOfStance.RePosition;
                //mAnimations.Animation_SetSprintState(false);
                durationOfScream = 2f;
                screamStarted = false;
            }
        }
    }

    #endregion

    #region Re-Positioning

    private void Actions_RePosition()
    {
        if (!TargetInSight())
            mNavMeshAgent.SetDestination(mTargetToFollow.position);
        else
        {
            SetLocationToThrowAt();
            durationOfScream = 2f;
            mCurrentStance = TypeOfStance.Throwing;
        }
    }
    #endregion

    #region Throwing
    [Header("Throwing")]
    private int amountToThrow = 3;
    [SerializeField]
    private GameObject throwIndicatorPrefab = null;

    private bool needToReload = false;
    private int amountThrown = 0;



    private void Actions_Throwing()
    {
        if (!needToReload)
        {
            mNavMeshAgent.SetDestination(transform.position);
            for (int i = 0; i < amountToThrow; i++)
            {

                StartCoroutine(ThrowSnowball((float)i));
            }

            //print("have now thrown: " + amountThrown);

            needToReload = true;
        }

        transform.LookAt(new Vector3(mTargetToFollow.position.x, transform.position.y, mTargetToFollow.position.z));
    }
    private IEnumerator ThrowSnowball(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);

        GameObject Nisse_Snowball = Instantiate(prefab_snowball, transform.position + 5f * Vector3.up, Quaternion.identity, null);

        Nisse_Snowball.GetComponent<Nisse_SnowballBehavior>().SetTargetLocation(throwHitLocation);
        GameObject Nisse_Indicator = Instantiate(throwIndicatorPrefab, throwHitLocation, transform.rotation, null);
        Destroy(Nisse_Indicator, 1f);
        amountThrown++;
        if (amountThrown == 3)
        {

            mCurrentStance = TypeOfStance.Retreating;
            amountThrown = 0;
        }
    }

    #endregion

    #region Retreating

    [Header("Retreat")]
    [SerializeField]
    private float durationOfRetreat = 3f;
    [SerializeField]
    private float retreatSpeed = 10f;
    [SerializeField]
    private float retreatAcceleration = 300f;

    private Vector3 escapingDirection = Vector3.zero;

    private void Actions_Retreating()
    {
        if (needToReload)
        {
            StartCoroutine(ReatreatDuration());
            mNavMeshAgent.speed = retreatSpeed;
            mNavMeshAgent.acceleration = retreatAcceleration;
            escapingDirection = (mTargetToFollow.position - transform.position).normalized;
            needToReload = false;
        }
        mNavMeshAgent.SetDestination(transform.position - escapingDirection);
    }

    private IEnumerator ReatreatDuration()
    {
        yield return new WaitForSeconds(durationOfRetreat);
        mCurrentStance = TypeOfStance.Reloading;
    }

    #endregion

    #region Reloading
    [Header("Reloading")]
    [SerializeField]
    private float reloadDuration = 1f;

    private bool reloadingStarted = false;

    private void Actions_Reloading()
    {
        if (!reloadingStarted)
        {
            StartCoroutine(Reloading());
            reloadingStarted = true;
        }
            
    }

    private IEnumerator Reloading()
    {
        yield return new WaitForSeconds(reloadDuration);
        mCurrentStance = TypeOfStance.Following;
        reloadingStarted = false;
    }

    #endregion

    #region Taking Damage
    private void Actions_TakingDamage()
    {

    }

    #endregion

    #region Dying
    private void Actions_Dying()
    {

    }

    #endregion

    #endregion


    #region Update-Functions

    void Awake () {

        mNavMeshAgent = GetComponent<NavMeshAgent>();

        if (mTargetToFollow == null)
            mTargetToFollow = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        mIdleTarget = transform.position;
    }
	
	void Update () {
        WhatToDo();
        SetLocationToThrowAt();

	}
#endregion
}
