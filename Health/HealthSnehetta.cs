using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSnehetta : MonoBehaviour {

    private EnemySnehetta snehettaMainScript = null;

    [SerializeField]
    private HealthType mHealthScript = null;

	// Use this for initialization
	void Awake () {
        snehettaMainScript = GetComponent<EnemySnehetta>();
        mHealthScript.Health_RestoreHealthToMax();
        mHealthScript.Health_RestoreHealthToMax();    
    }

    public void Snehetta_TakeDamage(int damage)
    {
        mHealthScript.TakeDamage(damage);
        snehettaMainScript.NextPhase();
    }




}
