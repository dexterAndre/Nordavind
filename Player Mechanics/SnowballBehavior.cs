using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
    private void Update()
    {
        Vector3 mVelocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = new Vector3(mVelocity.x, mVelocity.y-1f, mVelocity.z);
    }


    private void OnCollisionEnter(Collision collision)
	{
		Destroy(gameObject);
	}
}