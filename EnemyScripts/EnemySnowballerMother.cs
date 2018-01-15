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
        GameObject Snowballer = Instantiate(SnowballerPrefab, mSpawnPoints[0].position, Quaternion.identity, null);
        Snowballer = Instantiate(SnowballerPrefab, mSpawnPoints[1].position, Quaternion.identity, null);
        Snowballer = Instantiate(SnowballerPrefab, mSpawnPoints[2].position, Quaternion.identity, null);
        haveSpawnedNewSnowballer = true;
        yield return new WaitForSeconds(spawnCooldown);
        haveSpawnedNewSnowballer = false;
    }



    #endregion

    #region FrostBeam

    private Transform mHeadTransformForward = null;

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
        mHeadTransformForward = transform.GetChild(0).GetComponent<Transform>();

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
