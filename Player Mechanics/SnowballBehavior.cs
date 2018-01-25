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
        if (collision.gameObject.GetComponent<HealthHedgehog>() != null)
        {
            collision.gameObject.GetComponent<HealthHedgehog>().Hedgehog_Takingdamage(1);
        }
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
	}

    private void OnTriggerStay(Collider collision)
    {
        //print("Hit ON_COLLISION_ENTER!");
        if (collision.gameObject.GetComponent<HealthHedgehog>() != null)
        {
            collision.gameObject.GetComponent<HealthHedgehog>().Hedgehog_Takingdamage(1);
        }
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }


    private void OnTriggerExit(Collider collision)
    {
        //print("Hit ON_COLLISION_ENTER!");
        if (collision.gameObject.GetComponent<HealthHedgehog>() != null)
        {
            collision.gameObject.GetComponent<HealthHedgehog>().Hedgehog_Takingdamage(1);
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
    private void OnCollisionStay(Collision collision)
    {
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }
    private void OnCollisionExit(Collision collision)
    {
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }
}