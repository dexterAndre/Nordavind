using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnowballer : EnemyController {

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

        if (foundEnemy) {
            if(mNavmeshAgent.enabled == true)
                SetNavMeshDestinationToMovingObject(mTargetToFollow);
            else
            {
                if (CheckIfHostileIsWithinAttackRange(transform.position, mTargetToFollow.position)) {
                    ChargeAtTarget(); //Add something in here.
                }
            }
        }
	}

    private void ChargeAtTarget()
    {
        
    }

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




    #endregion

}
