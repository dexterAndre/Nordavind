using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
	{
        print("Hit ON_COLLISION_ENTER!");
		Destroy(gameObject);
	}
}