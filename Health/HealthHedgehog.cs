using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHedgehog : MonoBehaviour {

    private int currentHealth;

    [SerializeField]
    private HealthType mHealthScript;

    private EnemyHedgehog mBehaviorScript = null;

	void Awake () {
        currentHealth = mHealthScript.health;
        mBehaviorScript = GetComponent<EnemyHedgehog>();
    }

    public void Hedgehog_Takingdamage(int damage)
    {
        mHealthScript.TakeDamage(damage, currentHealth);
        if (mHealthScript.Health_CheckIfDead(currentHealth))
        {
            mBehaviorScript.SetCurrentStance(EnemyHedgehog.TypeOfStances.ChargingUp);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            mBehaviorScript.SetCurrentStance(EnemyHedgehog.TypeOfStances.Scared);
        }
    }
}
