using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthActor : MonoBehaviour {

    protected int mHealth = 10;

    protected int maxHealth = 10;

    protected void TakeDamage(int damage)
    {
        mHealth -= damage;
    }

    protected void Health_RestoreHealthToMax()
    {
        mHealth = maxHealth;
    }

    protected bool Health_CheckIfDead()
    {
        if (mHealth <= 0)
            return true;
        else
            return false;
    }

}
