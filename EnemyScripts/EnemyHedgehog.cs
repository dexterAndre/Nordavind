using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHedgehog : MonoBehaviour {

    #region Stances

    protected enum TypeOfStances
    {
        Idle,
        Searching,
        Following,
        Sprinting,
        ChargingUp,
        Exploding
    };

    /// <summary>
    /// Current active stance, this will tell you if you're idle, following a target or attacking.
    /// </summary>
    protected TypeOfStances mCurrentStance = TypeOfStances.Idle;
    #endregion

    #region Data - constants

    private NavMeshAgent mNavMeshAgent = null;

    [SerializeField]
    private float mDisablingRange = 200f;

    [SerializeField]
    private float mDetectionRange = 100f;

    [SerializeField]
    private float mSprintingRange = 25f;

    [SerializeField]
    private float mSelfDestructRange = 5f;

    #endregion

    #region Data - dynamic

    [SerializeField]
    private Transform mTargetToFollow = null;

    [SerializeField]
    private Vector3 mLastPosition = Vector3.zero;


    #endregion

    #region Searching for enemies


    private bool TargetInSight()
    {
        RaycastHit lineOfSightCheck;

        Vector3 direction = transform.position - mTargetToFollow.position;

        Debug.DrawLine(transform.position, direction, Color.red);

        direction = direction.normalized;

        if (Physics.Raycast(transform.position, direction, out lineOfSightCheck, Mathf.Infinity))
        {
            if (lineOfSightCheck.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                return true;
        }
        return false;
    }

    private void CheckRangeOfTarget()
    {

        float distanceToTarget  = (mTargetToFollow.position - transform.position).magnitude;

        if (distanceToTarget < mDisablingRange && mCurrentStance == TypeOfStances.Idle)
        {
            mCurrentStance = TypeOfStances.Searching;
        }
        else if (distanceToTarget < mDetectionRange && mCurrentStance == TypeOfStances.Searching)
        {
            if(TargetInSight())
                mCurrentStance = TypeOfStances.Following;
            else
                mCurrentStance = TypeOfStances.Idle;
        }
        else if (distanceToTarget < mSprintingRange && mCurrentStance == TypeOfStances.Following)
        {
            mCurrentStance = TypeOfStances.Sprinting;
        }
        else if (distanceToTarget < mSelfDestructRange && mCurrentStance == TypeOfStances.Sprinting)
        {
            mCurrentStance = TypeOfStances.Exploding;
        }
    }







    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mDetectionRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, mSprintingRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mSelfDestructRange);
    }
    #endregion

    private void Start()
    {
        mNavMeshAgent = GetComponent<NavMeshAgent>();
    }

}
