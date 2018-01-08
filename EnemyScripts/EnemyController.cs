using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    #region protected-variables
    protected NavMeshAgent mNavmeshAgent = null;
    protected Animator mAnimator = null;
    protected AudioSource mAudioSource = null;
    protected float mSpeed = 10f;
    protected float mDetectionRange = 10f;
    protected Transform mTargetToFollow = null;
    protected Vector3 mLastPosition = Vector3.zero;
    protected bool followingMovingTarget = false;
    #endregion


    #region Pathfinding

    protected void UpdateFollowingTarget() {
        if (followingMovingTarget) {
            mNavmeshAgent.SetDestination(mTargetToFollow.position);
        }
    }

    protected void SetNavMeshDestinationToMovingObject(Transform target)
    {
        mTargetToFollow = target;
        followingMovingTarget = true;
    }

    protected void SetNavMeshDestinationToCertainPosition(Vector3 newTargetPosition)
    {
        mNavmeshAgent.SetDestination(newTargetPosition);
    }

    protected void GoBackToIdleState()
    {
        mNavmeshAgent.SetDestination(mLastPosition);
        followingMovingTarget = false;
    }

    protected void StopNavMesh()
    {
        mNavmeshAgent.enabled = false;
    }
    protected void StartNavMesh(){
        mNavmeshAgent.enabled = true;
    }

    private void SaveCurrentPosition(Vector3 mPosition)
    {
        mLastPosition = mPosition;
    }
    #endregion

}
