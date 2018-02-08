using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealthType : ScriptableObject {

    private int health;
    public int maxHealth;

    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void Health_RestoreHealthToMax()
    {
        health = maxHealth;
    }

    public bool Health_CheckIfDead()
    {
        if (health <= 0)
            return true;
        else
            return false;
    }

    public int Health_GetHealth()
    {
        return health;
    }
}
