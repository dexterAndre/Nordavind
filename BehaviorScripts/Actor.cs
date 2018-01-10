using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    #region Resources

    /// <summary>
    /// This is the unit's max health, and will be used at the start of the game, or whenever you have to reset the unit's health.
    /// </summary>
    protected int mMaxHealth = 10;

    /// <summary>
    /// The unit's current health, this is the health used during gameplay.
    /// </summary>
    [Header("Health")]
    [SerializeField]
    protected int mCurrentHealth = 0;

    /// <summary>
    /// If you ever want to know what the max health of the selected unit call this function.
    /// </summary>
    /// <returns></returns>
    protected int Health_GetMaxHealth() { return mMaxHealth; }

    /// <summary>
    /// If you ever want to know what the current health of the selected unit call this function.
    /// </summary>
    /// <returns></returns>
    protected int Health_GetCurrentHealth() { return mCurrentHealth; }

    /// <summary>
    /// This function is used to set the unit's health state. It will affect both current and max health, so be CAREFUL when using this function.
    /// </summary>
    /// <param name="newHealth"></param>
    protected void Health_SetStandardHealth(int newHealth)
    {
        mMaxHealth = newHealth;
        mCurrentHealth = mMaxHealth;
    }

    /// <summary>
    /// Restoring this unit's current health to full (max health).
    /// </summary>
    protected void Health_SetHealthBackToFull()
    {
        mCurrentHealth = mMaxHealth;
    }

    /// <summary>
    /// This function is used to deal damage to the unit.
    /// </summary>
    /// <param name="damage"></param>
    protected void Health_TakeDamage(int damage)
    {
        mCurrentHealth -= damage;
    }

    /// <summary>
    /// Will return TRUE if health is equal or below 0. Will return FALSE if health is higher than 0.
    /// </summary>
    /// <returns></returns>
    protected bool Health_CheckIfDead()
    {
        if (mCurrentHealth <= 0)
        {
            return true;
        }
        else
            return false;
    }


    #endregion

    #region Components

    protected Animator mAnimator = null;

    protected void GetActorComponents()
    {
        if (GetComponent<Animator>() != null)
            mAnimator = GetComponent<Animator>();
    }
    #endregion

}
