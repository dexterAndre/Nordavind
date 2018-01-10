using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    #region Resources

    protected int mMaxHealth = 10;

    protected int mCurrentHealth = 0;

    protected int GetHealth() { return mMaxHealth; }

    protected void SetStandardHealth(int newHealth)
    {
        mMaxHealth = newHealth;
        mCurrentHealth = mMaxHealth;
    }

    protected void SetHealthBackToFull()
    {
        mCurrentHealth = mMaxHealth;
    }

    protected void TakeDamage(int damage)
    {
        mCurrentHealth -= damage;
    }

    protected bool CheckIfDead()
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
#endregion

}
