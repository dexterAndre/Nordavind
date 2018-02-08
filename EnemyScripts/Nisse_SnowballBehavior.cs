using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Nisse_SnowballBehavior : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabForSnowballHit = null;

    private Vector3 targetLocation = Vector3.zero;
    private float startTime = 0f;
    private float speed = 2.5f;
    private float journeyLength = 0f;

    public void SetTargetLocation(Vector3 target)
    {
        targetLocation = target;
        Destroy(this.gameObject, 5f);
        startTime = Time.time;
        journeyLength = Vector3.Distance(transform.position, target);
    }

    private void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;

        transform.position = Vector3.Lerp(transform.position, targetLocation + Vector3.up*2f, fracJourney);
    }

    #region TriggerCollision
    private void HitWithTrigger(Collider collision)
    {
        if (collision.gameObject.GetComponent<HealthHedgehog>() != null)
        {
            if (!collision.gameObject.GetComponent<InvincibilityFrames>().GetInvincibleState())
            {
                collision.gameObject.GetComponent<HealthHedgehog>().Hedgehog_Takingdamage(1);
                collision.gameObject.GetComponent<InvincibilityFrames>().StartInvincibility();
            }
            
        }
        else if (collision.gameObject.transform.parent.GetComponent<HealthSnehetta>() != null)
        {
            if (!collision.gameObject.transform.parent.GetComponent<InvincibilityFrames>().GetInvincibleState())
            {
                collision.gameObject.transform.parent.GetComponent<HealthSnehetta>().Snehetta_TakeDamage(1);
                collision.gameObject.transform.parent.GetComponent<InvincibilityFrames>().StartInvincibility();
            }
            
        }
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider collision)
    {
        HitWithTrigger(collision);
    }

    private void OnTriggerStay(Collider collision)
    {
        HitWithTrigger(collision);
    }


    private void OnTriggerExit(Collider collision)
    {
        HitWithTrigger(collision);
    }

    #endregion


    #region ColliderCollision

    private void HitWithCollider(Collision collision)
    {
        GameObject snowballhitParticle = Instantiate(prefabForSnowballHit, transform.position, Quaternion.identity, null);
        Destroy(snowballhitParticle, 1f);
        Destroy(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        HitWithCollider(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        HitWithCollider(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        HitWithCollider(collision);
    }
#endregion
}