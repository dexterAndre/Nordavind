using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyWithNavigation : ScriptableObject {
    [Header("Speed")]
    public float acceleration = 12f;
    public float normalSpeed = 6f;
    public float combatSpeed = 20f;

    [Header("Range to target")]
    public float detectionRange = 100f;
    public float combatRange = 25f;
    public float attackingRange = 5f;

    [Header("Target movement prediction")]
    public float forwardPredrictionSensetivity = 20f;
}
