using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSnehetta : HealthActor {

    private EnemyHeadhogMother snehettaMainScript = null;
    
    [SerializeField]
    private int snehettaHealth = 3;


	// Use this for initialization
	void Awake () {
        snehettaMainScript = GetComponent<EnemyHeadhogMother>();
        maxHealth = snehettaHealth;
        Health_RestoreHealthToMax();    
    }

    public void Snehetta_TakeDamage(int damage)
    {
        TakeDamage(damage);
        snehettaMainScript.NextPhase();
    }




}
