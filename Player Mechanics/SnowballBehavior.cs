using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabForSnowballHit = null;

    private void OnTriggerEnter(Collider collision)
	{
        //print("Hit ON_COLLISION_ENTER!");
        if (collision.gameObject.GetComponent<EnemyHedgehog>() != null)
        {
            collision.gameObject.GetComponent<EnemyHedgehog>().GetHit();
        }
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }
}