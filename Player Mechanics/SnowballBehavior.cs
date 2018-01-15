using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
	private void OnCollisionEnter(Collision collision)
	{
		Destroy(gameObject);
	}
}