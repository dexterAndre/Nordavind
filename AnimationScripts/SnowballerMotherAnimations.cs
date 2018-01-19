using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballerMotherAnimations : MonoBehaviour {


    private Animator mAnimator = null;

	void Start () {
        mAnimator = GetComponent<Animator>();
	}

    public void StartBreath()
    {
        mAnimator.SetTrigger("Breath");
    }
}
