using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SnowballBehavior : MonoBehaviour
{
    private void Update()
    {
        Vector3 mVelocity = GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = new Vector3(mVelocity.x, mVelocity.y-1f, mVelocity.z);

        RaycastHit snowballHit;
        if (Physics.Raycast(transform.position, transform.forward, out snowballHit, 2f))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            transform.GetChild(0).gameObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }


}