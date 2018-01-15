using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnowballer : EnemyController {

    //Should change the pipeline to be more effient.

    #region Spawned during mother encounter

    public void GetDestionationAfterSpawn(Vector3 motherFowardAxis, float forceOfPush)
    {
        Nav_StopNavMesh();
        mRigidBody.AddForce(motherFowardAxis * forceOfPush *Time.deltaTime, ForceMode.Impulse);
    }

    #endregion

    #region Searching for enemies.

    /// <summary>
    /// Used to check if this unit have recently searched for an hostile unit.
    /// </summary>
    private bool recentlySearchedForHostiles = false;

    /// <summary>
    /// Making a delay on the search for enemies, currently quick fix before delegate implementation.
    /// </summary>
    /// <param name="timer"></param>
    /// <returns></returns>
    private IEnumerator SearchForEnemy(float timer)
    {
        recentlySearchedForHostiles = true;
        CheckIfHostileIsWithinRange(transform.position);

        yield return new WaitForSeconds(timer);

        recentlySearchedForHostiles = false;

    }
    #endregion

    #region Charge

    /// <summary>
    /// Bool used to make a delay before checking if the charge hit or not.
    /// </summary>
    private bool checkDelayIfHit = false;

    /// <summary>
    /// Bool used to check if the charging sequence should be started.
    /// </summary>
    private bool gettingReadyToCharge = false;

    /// <summary>
    /// The bool that activates what happens after the charge, searching for explosion zone / starting damage zone.
    /// </summary>
    private bool charged = false;

    /// <summary>
    /// This value is set to 1f when the charging sequence starts
    /// </summary>
    private float chargeTimer = 0f;

    /// <summary>
    /// The force used to push the unit forward when charging at a hostile unit.
    /// </summary>
    [SerializeField]
    private float chargeForce = 25f;
    
    /// <summary>
    /// This float should be tweaked, and is the distance from where this unit goes for a self destruction.
    /// </summary>
    [SerializeField]
    private float explosionZoneDistanceToActive = 5f;

    /// <summary>
    /// Taking a charge timer, and subtracting 1 per second.
    /// <para>  The unit will look at the target without taking the y-axis into consideration.</para>
    /// <para>  Once the enemy charge, they will at the same frame as adding force look at the target in all axis, incase of having to charge downhill.</para>
    /// </summary>
    private void ChargeAtTarget()
    {
        if (!selfDestructionInitialized)
        {
            if(chargeTimer >= 0)
                chargeTimer -= 1 * Time.deltaTime;
            Vector3 lookTargetWithoutY = mTargetToFollow.position;
            lookTargetWithoutY = new Vector3(lookTargetWithoutY.x, 0, lookTargetWithoutY.z);
            transform.LookAt(lookTargetWithoutY);
            if (chargeTimer <= 0 && !charged)
            {
                transform.LookAt(mTargetToFollow.position);
                mRigidBody.AddForce(transform.forward * chargeForce * Time.deltaTime);
                gettingReadyToCharge = false;
                charged = true;
            }

            if (charged && !checkDelayIfHit)
            {
                StartCoroutine(WaitToCheckIfHit());
            }
            else if (!CheckIfHostileIsWithinAttackRange(transform.position, mTargetToFollow.position))
            {
                Nav_GoBackToIdleState();
                charged = false;
                gettingReadyToCharge = false;
                selfDestructionInitialized = false;
                CheckIfHostileIsWithinRange(transform.position);
            }
        }

    }
   
    /// <summary>
    /// Waits a little bit before it checks if it hits the target, makes it easier to dodge. PS: should perhaps remake it.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitToCheckIfHit() {
        checkDelayIfHit = true;
        yield return new WaitForSeconds(0.15f);
        float distance = (transform.position - mTargetToFollow.position).magnitude;
        if (distance < explosionZoneDistanceToActive)
        {
            StartCoroutine(SelfDestructionStart());
        }
        else
        {
            if (!(CheckIfHostileIsWithinAttackRange(transform.position, mTargetToFollow.position)))
            {
                Nav_GoBackToIdleState();
                charged = false;
                gettingReadyToCharge = false;
                selfDestructionInitialized = false;
                CheckIfHostileIsWithinRange(transform.position);
            }
            else
                StartCoroutine(WaitToCheckIfHit());
        }
    }
    #endregion

    #region Explosion

    /// <summary>
    /// This bool is here to make sure no more iterations of the charge starts once you start self destruction.
    /// </summary>
    private bool selfDestructionInitialized = false;

    /// <summary>
    /// This float will be the duration this unit stands still before exploding.
    /// <para>  This should depend on the difficulty to dodge, and the duration of the animation.</para>
    /// </summary>
    [SerializeField]
    private float selfDestructionAnimationDuration = 1f;

    /// <summary>
    /// This will stop the velocity in the x- and y-axis, and after the ienumerator is complete it will initialize the explosion.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SelfDestructionStart()
    {
        charged = false;
        mRigidBody.velocity = new Vector3(0f, mRigidBody.velocity.y, 0f);
        selfDestructionInitialized = true;
        yield return new WaitForSeconds(selfDestructionAnimationDuration);
        Explode();
    }

    /// <summary>
    /// The function used to make this unit explode.
    /// <para>  Will set the this gameobject to FALSE.</para>
    /// <para>  Will spawn explosion particles.</para>
    /// <para>  Will deal damage to hostile units inside the damage zone.</para>
    /// </summary>
    private void Explode()
    {
        print(this.gameObject.name + " just exploded");
        //spawn deathParticle here.
        this.gameObject.SetActive(false);
    }
        
#endregion

    #region UpdateFunctions
    void Start ()
    {
        SetComponentsAtStart();
	}
	
	void Update ()
    {
        if (!recentlySearchedForHostiles && mCurrentStance == TypeOfStances.Idle)
        {
            StartCoroutine(SearchForEnemy(2f));
        }

        if ((mCurrentStance == TypeOfStances.Following || mCurrentStance == TypeOfStances.Attacking) && !gettingReadyToCharge)
        {
            if (CheckIfHostileIsWithinAttackRange(transform.position, mTargetToFollow.position))
            {
                gettingReadyToCharge = true;
                Nav_StopNavMesh();
                chargeTimer = 1f;
                print("Roar sound, and animation");
            }
            else if (mNavMeshAgent.enabled == true)
            {
                print("Should start moving");

                if (mTargetToFollow == null)
                    Nav_SetNavMeshDestinationToMovingObject(mTargetToFollow);

                Nav_UpdateFollowingTarget();
            }
        }

        if (gettingReadyToCharge)
        {
            ChargeAtTarget();
        }
	}
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mAttackRange);
    }
    #endregion

}
    