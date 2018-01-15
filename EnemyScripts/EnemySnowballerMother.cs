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
    /// The spawnPoints used to set the new snowballers out during the encounter.
    /// </summary>
    private Transform[] mSpawnPoints = new Transform[3];

    private int amountSpawnedByMother = 0;


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
        GameObject snowballer = Instantiate(SnowballerPrefab, mSpawnPoints[0].position, Quaternion.identity, null);
        snowballer.name = "Snowballer " + amountSpawnedByMother;
        amountSpawnedByMother++;
        snowballer.GetComponent<EnemySnowballer>().GetDestionationAfterSpawn(mHeadTransform.forward);

        haveSpawnedNewSnowballer = true;
        yield return new WaitForSeconds(spawnCooldown);
        haveSpawnedNewSnowballer = false;
    }



    #endregion

    #region FrostBeam

    private Transform mHeadTransform = null;

    public void StopBeam()
    {

    }

    private void StartBeam()
    {

    }


#endregion

    #region UpdateFunctions

    private void Start ()
    {
        Health_SetStandardHealth(mStartingHealth);
        GetActorComponents();

        mHeadTransform = transform.GetChild(0).GetComponent<Transform>();

        for (int i = 0; i < 3; i++)
        {
            mSpawnPoints[i] = transform.GetChild(1).transform.GetChild(i).GetComponent<Transform>();
        }
	}
	
	private void Update ()
    {
        if (!haveSpawnedNewSnowballer)
            StartCoroutine(SpawnSnowballer());
	}
#endregion
}
