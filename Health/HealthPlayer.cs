using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPlayer : MonoBehaviour {

    private int currentHealth = 10;

    [SerializeField]
    private HealthType mHealthScript = null;

    private PlayerMovement mPlayerMovement = null;

    void Awake()
    {
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
        mHealthScript.Health_RestoreHealthToMax();
       // print(mHealthScript.Health_GetHealth() + " starting health");
    }

    public void Player_TakingDamage(int damage, bool includeKnockback, Vector3 knockDirection)
    {
        if (!includeKnockback)
            mHealthScript.TakeDamage(damage);
        else if(includeKnockback)
        {
            mHealthScript.TakeDamage(damage);
            //print(mHealthScript.Health_GetHealth() + " is the current health");
            mPlayerMovement.SetMovementVector(knockDirection);
            mPlayerMovement.SetState(PlayerMovement.State.Stun);

        }
        if (mHealthScript.Health_CheckIfDead())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
