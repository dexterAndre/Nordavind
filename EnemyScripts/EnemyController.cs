using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : Actor {

    #region Pathfinding

    /// <summary>
    /// These are the type of stances the behavior 
    /// </summary>
    protected enum TypeOfStances
    {
        Idle,
        Following,
        Attacking
    };

    /// <summary>
    /// Current active stance, this will tell you if you're idle, following a target or attacking.
    /// </summary>
    protected TypeOfStances mCurrentStance = TypeOfStances.Idle;

    /// <summary>
    /// Changes the current stance depending of index inserted.
    /// <para>  List:</para>
    /// <para>  0 = Idle</para>
    /// <para>  1 = Following</para>
    /// <para>  2 = Attacking</para>
    /// </summary>
    /// <param name="stanceIndex"></param>
    protected void ChangeCurrentStance(int stanceIndex)
    {
        if (stanceIndex == 0)
            mCurrentStance = TypeOfStances.Idle;
        else if (stanceIndex == 1)
            mCurrentStance = TypeOfStances.Following;
        else if (stanceIndex == 2)
            mCurrentStance = TypeOfStances.Attacking;
        else
            print("The stance you chose does not exist.");
    }

    /// <summary>
    /// Checking if the stanceIndex you sent in matches mCurrentStance.
    /// <para>  List:</para>
    /// <para>  0. Idle</para>
    /// <para>  1. Following</para>
    /// <para>  2. Attacking</para>
    /// </summary>
    /// <param name="stanceIndex"></param>
    /// <returns></returns>
    protected bool CheckStance(int stanceIndex)
    {
        if (stanceIndex == 0)
        {
            if (mCurrentStance == TypeOfStances.Idle)
                return true;
            else
                return false;
        }
        else if (stanceIndex == 1)
        {
            if (mCurrentStance == TypeOfStances.Following)
                return true;
            else
                return false;
        }
        else if (stanceIndex == 2)
        {
            if (mCurrentStance == TypeOfStances.Attacking)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    ///<summary>
    ///This is the agent that you will be using to move this unit with pathfinding.
    ///</summary>
    protected NavMeshAgent mNavMeshAgent = null;

    ///<summary>
    ///This is the variable holding the current target you want to follow during a chase.
    ///</summary>
    protected Transform mTargetToFollow = null;

    ///<summary>
    ///This is your last position, and is ment to send you back here after a chase.
    ///</summary>
    protected Vector3 mLastPosition = Vector3.zero;
    
    ///<summary>
    ///This is the bool checking if you are moving towards a target, or if FALSE it allows for the normal idle movement to work.
    ///</summary>
    protected bool followingMovingTarget = false;

    ///<summary>
    ///The function you call to continue updating the position of this unit's target.
    ///</summary>
    protected void Nav_UpdateFollowingTarget() {
        if (followingMovingTarget) {
            mNavMeshAgent.SetDestination(mTargetToFollow.position);
        }
    }

    ///<summary>
    ///Remembers the target's transform to be able to send in the correct information to UpdateFollowingTarget().
    ///</summary>
    protected void Nav_SetNavMeshDestinationToMovingObject(Transform target)
    {
        mTargetToFollow = target;
        followingMovingTarget = true;
        mNavMeshAgent.speed = mCombatSpeed;
        ChangeCurrentStance(1);
        print("Unit started following a hostile target");
    }
    
    ///<summary>
    ///This function is called one time to set the destination of this unit to a certain world coordinate,
    ///will also be the change mLastPosition to this value incase a moving target is spotted.
    ///</summary>
    protected void Nav_SetNavMeshDestinationToCertainPosition(Vector3 newTargetPosition)
    {
        mNavMeshAgent.SetDestination(newTargetPosition);
        mLastPosition = newTargetPosition;
    }

    /// <summary>
    /// Reseting this unit's destination to mLastPosition, also set the followingMovingTarget to FALSE.
    /// </summary>
    protected void Nav_GoBackToIdleState()
    {
        Nav_StartNavMesh();
        mNavMeshAgent.SetDestination(mLastPosition);
        mTargetToFollow = null;
        ChangeCurrentStance(0);
        mNavMeshAgent.speed = mIdleSpeed;
        followingMovingTarget = false;
    }

    ///<summary>
    ///Turns the pathfinding system off. This gives the ability to move the unit automaticly.
    ///</summary>
    protected void Nav_StopNavMesh()
    {
        mNavMeshAgent.enabled = false;
        mRigidBody.isKinematic = false;
        mRigidBody.detectCollisions = true;
    }
    
    ///<summary>
    ///Turns the pathfinding system ON. This removes the ability to move the unit manually.
    ///</summary>
    protected void Nav_StartNavMesh(){
        mNavMeshAgent.enabled = true;
        mRigidBody.isKinematic = true;
        mRigidBody.detectCollisions = false;
    }

    ///<summary>
    ///Saves the current position of this unit to mLastPosition.
    ///</summary>
    private void Nav_SaveCurrentPosition(Vector3 mPosition)
    {
        mLastPosition = mPosition;
    }
    #endregion

    #region Health / Game information

    /// <summary>
    /// The speed you use when hostile units have been spotted.
    /// </summary>
    protected float mCombatSpeed = 10f;

    /// <summary>
    /// The speed you use when there is no hostile units.
    /// </summary>
    protected float mIdleSpeed = 5f;

    /// <summary>
    /// Within this range this unit can spot hostile units.
    /// </summary>
    protected float mDetectionRange = 50f;

    /// <summary>
    /// Within this range this unit can use their attack against hostile units.
    /// </summary>
    protected float mAttackRange = 15f;

    /// <summary>
    /// Will be true if an enemy have been found.
    /// </summary>
    protected bool foundEnemy = false;

    /// <summary>
    /// Checks if there are any hostiles within range.
    /// <para>  If there is any, this will be returned TRUE.</para>
    /// <para>  If there is non, this will be returned FALSE.</para>
    /// </summary>
    /// <param name="mPostion"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected void CheckIfHostileIsWithinRange(Vector3 mPosition)
    {
        Transform targetTransform = null;

        //Using a overlapshere to find nearby targets, then going through the list to see if there is a player here.
        Collider[] hitColliders = Physics.OverlapSphere(mPosition, mDetectionRange);
        for (uint i = 0; i < hitColliders.Length - 1; i++) {
            if (hitColliders[i].tag == "Player") {
                targetTransform = hitColliders[i].GetComponent<Transform>();
                print(targetTransform + " - has now been set. Which means that this unit found a hostile.");
            }
        }

        //Now we check if we found a target, and then take the correct actions accordingly.
        if (targetTransform == null)
        {
            print("This unit has no hostile targets in range.");
            Nav_StartNavMesh();
            foundEnemy = false;
        }
        else
        {
            Nav_SetNavMeshDestinationToMovingObject(targetTransform);
            foundEnemy = true;
        }
    }

    /// <summary>
    /// Checks if this unit can attack the hostile unit.
    /// <para>  If there is any, this will be returned TRUE.</para>
    /// <para>  If there is non, this will be returned FALSE.</para>
    /// </summary>
    /// <param name="mPosition"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected bool CheckIfHostileIsWithinAttackRange(Vector3 mPosition, Vector3 targetPosition)
    {
        float distance = (mPosition - targetPosition).magnitude;
        if (distance <= mAttackRange)
        {
            return true;
        }
        return false;
    }

    #endregion

    #region Physics

    /// <summary>
    /// This unit's "Rigidbody", can be used to apply physics.
    /// </summary>
    protected Rigidbody mRigidBody = null;

    #endregion

    #region StartUp
    
    /// <summary>
    /// Here every component needed from the live scene will be set.
    /// <para>  List of components added:</para>
    /// <para>  Nav Mesh Agent.</para>
    /// <para>  Animator.</para>
    /// <para>  Rigidbody.</para>
    /// </summary>
    protected void SetComponentsAtStart()
    {
        GetActorComponents();

        if (GetComponent<NavMeshAgent>() != null)
            mNavMeshAgent = GetComponent<NavMeshAgent>();

        if(GetComponent<Rigidbody>() != null)
            mRigidBody = GetComponent<Rigidbody>();

        Nav_StartNavMesh();

        mNavMeshAgent.speed = mIdleSpeed;

        Health_SetStandardHealth(mStartingHealth);
    }

    #endregion
}
