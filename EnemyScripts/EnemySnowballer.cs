using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnowballer : EnemyController {

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
    /// Taking a charge timer, and subtracting 1 per second.
    /// <para>  The unit will look at the target without taking the y-axis into consideration.</para>
    /// <para>  Once the enemy charge, they will at the same frame as adding force look at the target in all axis, incase of having to charge downhill.</para>
    /// </summary>
    private void ChargeAtTarget()
    {
        chargeTimer -= 1 * Time.deltaTime;
        print(chargeTimer + " : until charge!");
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

        if (charged)
        {
            float distance = (transform.position - mTargetToFollow.position).magnitude;
            
        }
    }
    #endregion

    #region Explosion


#endregion

    #region UpdateFunctions
    void Start ()
    {
        SetComponentsAtStart();
	}
	
	void Update ()
    {
        if (!recentlySearchedForHostiles)
        {
            StartCoroutine(SearchForEnemy(2f));
        }

        if (foundEnemy && !gettingReadyToCharge) {
            if(mNavMeshAgent.enabled == true)
                Nav_SetNavMeshDestinationToMovingObject(mTargetToFollow);
            else
            {
                if (CheckIfHostileIsWithinAttackRange(transform.position, mTargetToFollow.position)) {
                    gettingReadyToCharge = true;
                    Nav_StopNavMesh();
                    chargeTimer = 1f;
                    print("Roar sound, and animation");
                }
            }
        }
        if (gettingReadyToCharge)
        {
            ChargeAtTarget();
        }
	}
    #endregion

}
