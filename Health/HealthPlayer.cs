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
        currentHealth = mHealthScript.health;
    }

    public void Player_TakingDamage(int damage, bool includeKnockback, Vector3 knockDirection)
    {
        if (!includeKnockback)
            mHealthScript.TakeDamage(damage, currentHealth);
        else if(includeKnockback)
        {
            mHealthScript.TakeDamage(damage, currentHealth);
            mPlayerMovement.SetMovementVector(knockDirection);
            mPlayerMovement.SetState(PlayerMovement.State.Stun);

        }
        if (mHealthScript.Health_CheckIfDead(currentHealth))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
