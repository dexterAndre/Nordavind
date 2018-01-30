using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthPlayer : HealthActor {

    [SerializeField]
    private int playerHealth;

    void Awake()
    {
        mHealth = playerHealth;
        maxHealth = mHealth;
    }

    public void Player_TakingDamage(int damage)
    {
        TakeDamage(damage);
        if (Health_CheckIfDead())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}
