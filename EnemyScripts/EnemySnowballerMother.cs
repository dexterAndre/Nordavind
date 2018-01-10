using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnowballerMother : Actor {

    #region Spawn

    /// <summary>
    /// The prefab used to spawn a snowballer during the encounter.
    /// </summary>
    [Header("Spawning of Snowballers")]
    [SerializeField]
    private GameObject SnowballerPrefab = null;

    /// <summary>
    /// This is the force that the snowballer will be using to get pushed out of the mothers mouth.
    /// <para>  This should be written as a high number, due to it being multiplied by Time.deltaTime.</para>
    /// </summary>
    [SerializeField]
    private Vector2 forceOfSpawningSnowballer = new Vector3(500000f, 2000f);


    /// <summary>
    /// The cooldown before the mother can spawn a new snowballer.
    /// </summary>
    [SerializeField]
    private float spawnCooldown = 5f;

    /// <summary>
    /// While TRUE the mother cannot spawn a new snowballer, while FALSE the mother can spawn a new snowballer.
    /// </summary>
    private bool haveSpawnedNewSnowballer = false;

    /// <summary>
    /// The mother spawns a snowballer, then creates a cooldown before spawning a new one.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnSnowballer()
    {
        GameObject Snowballer = Instantiate(SnowballerPrefab, transform.position, Quaternion.identity, null);
        Snowballer.GetComponent<EnemySnowballer>().GetSpawnedAndPushed(transform.forward + (Vector3.up * forceOfSpawningSnowballer.y), forceOfSpawningSnowballer.x);
        haveSpawnedNewSnowballer = true;
        yield return new WaitForSeconds(spawnCooldown);
        haveSpawnedNewSnowballer = false;
    }



#endregion

    #region UpdateFunctions

    private void Start ()
    {
        Health_SetStandardHealth(mStartingHealth);
        GetActorComponents();
	}
	
	private void Update ()
    {
		
	}
#endregion
}
