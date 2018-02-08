using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyWithNavigation : ScriptableObject {
    [Header("Normal movement")]
    public float normalAcceleration = 12f;
    public float normalSpeed = 6f;

    [Header("Combat movement")]
    public float combatAcceleration = 20f;
    public float combatSpeed = 20f;

    [Header("Range to target")]
    public float detectionRange = 100f;
    public float combatRange = 50f;
    public float attackingRange = 5f;

    [Header("Target movement prediction")]
    public float forwardPredrictionSensetivity = 20f;
    public bool useForwardPrediction = true;
}
