using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogVisualRotator : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        RaycastHit faceHit;
        if (Physics.Raycast(transform.position, Vector3.down, out faceHit, 5f))
        {
            transform.rotation = Quaternion.LookRotation(transform.parent.forward, faceHit.normal);            
        }
	}
}
