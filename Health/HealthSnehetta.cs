using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSnehetta : MonoBehaviour {

    private EnemySnehetta snehettaMainScript = null;

    [SerializeField]
    private HealthType mHealthScript = null;

    private int currentHealth;


	// Use this for initialization
	void Awake () {
        snehettaMainScript = GetComponent<EnemySnehetta>();
        currentHealth = mHealthScript.health;
        mHealthScript.Health_RestoreHealthToMax(currentHealth);    
    }

    public void Snehetta_TakeDamage(int damage)
    {
        mHealthScript.TakeDamage(damage, currentHealth);
        snehettaMainScript.NextPhase();
    }




}
