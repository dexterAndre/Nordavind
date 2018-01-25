using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHedgehog : HealthActor {

    [SerializeField]
    private int hedgehogHealth;

    private EnemyHedgehog mBehaviorScript = null;

	void Awake () {
        mHealth = hedgehogHealth;
        maxHealth = mHealth;
        mBehaviorScript = GetComponent<EnemyHedgehog>();
    }

    public void Hedgehog_Takingdamage(int damage)
    {
        TakeDamage(damage);
        if (Health_CheckIfDead())
        {
            mBehaviorScript.SetCurrentStance(EnemyHedgehog.TypeOfStances.Exploding);
        }
        else
        {
            if (mBehaviorScript.GetCurrentStance() == EnemyHedgehog.TypeOfStances.Sprinting)
            {
                mBehaviorScript.SetCurrentStance(EnemyHedgehog.TypeOfStances.ChargingUp);
            }
            else
            {
                mBehaviorScript.SetCurrentStance(EnemyHedgehog.TypeOfStances.Scared);
            }
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
