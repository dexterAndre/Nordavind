using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
    [SerializeField]
    private LayerMask mLayerMask;
    private SphereCollider mSphereCollider;
    private Rigidbody mRigidbody;
    private RaycastHit mRaycastHit;
    private Color mDebugColor = new Color(255, 125, 0, 1);

    private void Awake()
    {
        mLayerMask = ~(1 << 10); // 10: Player layer
        if(mSphereCollider == null)
            mSphereCollider = GetComponent<SphereCollider>();
        if (mRigidbody = null)
            mRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 vel = Vector3.zero;
        if(mRigidbody != null)
            vel = mRigidbody.velocity;
        if(Physics.SphereCast(transform.position, transform.localScale.x /* Assuming uniform scale*/, vel, out mRaycastHit, vel.magnitude * 10f, mLayerMask))
        {
            print("Hit SPHERE_CAST!");
            Destroy(gameObject);
        }

        VisualDebug();
    }

    private void OnCollisionEnter(Collision collision)
	{
        print("Hit ON_COLLISION_ENTER!");
		Destroy(gameObject);
	}

    private void VisualDebug()
    {
        if (mRigidbody != null)
        {
            Debug.DrawLine(transform.position, transform.position + mRigidbody.velocity * 10f, mDebugColor);
        }
    }
}