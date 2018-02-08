using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour {

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private GameObject impactParticle = null;

    [SerializeField]
    private bool shouldKnockPlayerIfHit = true;

    [SerializeField]
    private float knockBackPower = 4f;



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.transform.GetChild(2).GetComponent<HealthPlayer>() != null)
            {
                if (!other.transform.GetChild(2).GetComponent<InvincibilityFrames>().GetInvincibleState())
                {

                    other.transform.GetChild(2).GetComponent<InvincibilityFrames>().StartInvincibility();

                    other.transform.GetChild(2).GetComponent<HealthPlayer>().Player_TakingDamage(damage, shouldKnockPlayerIfHit, transform.forward * knockBackPower + other.transform.forward);
                    GameObject hitParticle = Instantiate(impactParticle, other.gameObject.transform.position, Quaternion.identity, null);
                    Destroy(hitParticle, 0.5f);

                    print("Player got hit by damage = " + damage);
                }
            }
        }
    }
}
