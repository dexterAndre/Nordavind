using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogAnimations : MonoBehaviour {

    private Animator mAnimator = null;

	// Use this for initialization
	void Start () {
        mAnimator = GetComponent<Animator>();
	}

    public void Animation_StartScream()
    {
        mAnimator.SetTrigger("Scream");
    }

    public void Animation_SetSprintState(bool state)
    {
        mAnimator.SetBool("shouldSprint", state);
    }

    public void Animation_InRange()
    {
        mAnimator.SetTrigger("inRange");
    }

    public void Animation_GotHit()
    {
        mAnimator.SetTrigger("gotHit");
    }


}
