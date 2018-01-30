using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningHedgehogParticle : MonoBehaviour {

    private ParticleSystem Meteor = null;
    private ParticleSystem OuterCircle = null;
    private ParticleSystem InnerExplosion = null;
    private GameObject IndicatorForSpawn = null;

    [SerializeField]
    private GameObject hedgehogprefab = null;

    private Transform player = null;
    private Vector3 targetPosition = Vector3.zero;


    private void Awake()
    {
        Meteor = transform.GetChild(0).GetComponent<ParticleSystem>();
        OuterCircle = transform.GetChild(1).GetComponent<ParticleSystem>();
        InnerExplosion = transform.GetChild(2).GetComponent<ParticleSystem>();
        IndicatorForSpawn = transform.GetChild(3).gameObject;

        StartCoroutine(ParticlePipeline());
    }

    private IEnumerator ParticlePipeline()
    {
        Meteor.Play();
        OuterCircle.Stop();
        InnerExplosion.Stop();
        IndicatorForSpawn.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        Spawn_Hedgehog();
        IndicatorForSpawn.SetActive(false);
        Meteor.Stop();
        OuterCircle.Play();
        InnerExplosion.Play();
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }

    public void SetPositionAtStart(Vector3 position)
    {
        targetPosition = position;
    }

    private void Spawn_Hedgehog()
    {
        GameObject snowballer = Instantiate(hedgehogprefab, transform.position, Quaternion.identity, null);
        snowballer.GetComponent<EnemyHedgehog>().SetTargetToFollow(player);
        snowballer.GetComponent<EnemyHedgehog>().SetIdleTarget(targetPosition);
        snowballer.GetComponent<EnemyHedgehog>().SetCurrentStance(EnemyHedgehog.TypeOfStances.Idle);
        snowballer.name = "Hedgehog";
    }
}
