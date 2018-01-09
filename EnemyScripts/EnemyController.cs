﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour {

    #region Pathfinding

    ///<summary>
    ///This is the agent that you will be using to move this unit with pathfinding.
    ///</summary>
    protected NavMeshAgent mNavmeshAgent = null;

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
    protected void UpdateFollowingTarget() {
        if (followingMovingTarget) {
            mNavmeshAgent.SetDestination(mTargetToFollow.position);
        }
    }
    ///<summary>
    ///Remembers the target's transform to be able to send in the correct information to UpdateFollowingTarget().
    ///</summary>
    protected void SetNavMeshDestinationToMovingObject(Transform target)
    {
        mTargetToFollow = target;
        followingMovingTarget = true;
        print("Unit started following a hostile target");
    }
    
    ///<summary>
    ///This function is called one time to set the destination of this unit to a certain world coordinate,
    ///will also be the change mLastPosition to this value incase a moving target is spotted.
    ///</summary>
    protected void SetNavMeshDestinationToCertainPosition(Vector3 newTargetPosition)
    {
        mNavmeshAgent.SetDestination(newTargetPosition);
        mLastPosition = newTargetPosition;
    }

    /// <summary>
    /// Reseting this unit's destination to mLastPosition, also set the followingMovingTarget to FALSE.
    /// </summary>
    protected void GoBackToIdleState()
    {
        mNavmeshAgent.SetDestination(mLastPosition);
        followingMovingTarget = false;
    }

    ///<summary>
    ///Turns the pathfinding system off. This gives the ability to move the unit automaticly.
    ///</summary>
    protected void StopNavMesh()
    {
        mNavmeshAgent.enabled = false;
    }
    
    ///<summary>
    ///Turns the pathfinding system ON. This removes the ability to move the unit manually.
    ///</summary>
    protected void StartNavMesh(){
        mNavmeshAgent.enabled = true;
    }

    ///<summary>
    ///Saves the current position of this unit to mLastPosition.
    ///</summary>
    private void SaveCurrentPosition(Vector3 mPosition)
    {
        mLastPosition = mPosition;
    }
    #endregion

    #region Animations
    /// <summary>
    /// This unit's "Animator", can be used to apply physics.
    /// </summary>
    protected Animator mAnimator = null;

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
    /// This unit's health, if  less than 0 this unit dies.
    /// </summary>
    protected int mHealth = 10;

    /// <summary>
    /// Within this range this unit can spot hostile units.
    /// </summary>
    protected float mDetectionRange = 50f;

    /// <summary>
    /// Within this range this unit can use their attack against hostile units.
    /// </summary>
    protected float attackRange = 15f;

    /// <summary>
    /// Returns this unit's health.
    /// </summary>
    /// <returns>int</returns>
    protected int GetHealth() { return mHealth; }

    /// <summary>
    ///This function will deal damage to the unit it is attached to.
    /// </summary>
    /// <param name="damageDealt"></param>
    public void TakeDamage(int damageDealt){ mHealth -= damageDealt; }

    /// <summary>
    /// Checks if there are any hostiles within range.
    /// <para>  If there is any, this will be returned TRUE.</para>
    /// <para>  If there is non, this will be returned FALSE.</para>
    /// </summary>
    /// <param name="mPostion"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected bool CheckIfHostileIsWithinRange(Vector3 mPosition)
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
            return false;
        }
        else
        {
            SetNavMeshDestinationToMovingObject(targetTransform);
            return true;
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
        if (distance <= mDetectionRange)
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
        if (GetComponent<NavMeshAgent>() != null)
            mNavmeshAgent = GetComponent<NavMeshAgent>();

        if (GetComponent<Animator>() != null)
            mAnimator = GetComponent<Animator>();

        if(GetComponent<Rigidbody>() != null)
            mRigidBody = GetComponent<Rigidbody>();
    }
    
    #endregion
}
