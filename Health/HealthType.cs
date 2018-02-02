using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HealthType : ScriptableObject {

    public int health;

    public void TakeDamage(int damage, int currentHealth)
    {
        currentHealth -= damage;
    }

    public void Health_RestoreHealthToMax(int currentHealth)
    {
        currentHealth = health;
    }

    public bool Health_CheckIfDead(int currentHealth)
    {
        if (currentHealth <= 0)
            return true;
        else
            return false;
    }
}
