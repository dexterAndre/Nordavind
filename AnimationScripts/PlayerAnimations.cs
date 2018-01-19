using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{

    private PlayerStateMachine mPlayerStateMachine = null;
    private PlayerMovement mPlayerMovement = null;

    private bool hasJumped = false;

	void Start () {
        GetHeadAnimtorComponents();
        mPlayerStateMachine = transform.parent.GetComponent<PlayerStateMachine>();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
    }

    private void Animation_SetMovement()
    {
            mAnimator.SetFloat("Speed", mPlayerMovement.GetMovementSpeed());
    }

    private void Animation_SetJump()
    {
        mAnimator.SetTrigger("Jump");
    }

    private void Animation_ThrowSnowball()
    {
        mAnimator.SetTrigger("Throw");
    }

    private void AnimationLayer_SwitchWeightForScarf()
    {
        mAnimator.SetLayerWeight(1, 0);
        mAnimator.SetLayerWeight(2, 1);
    }


    private void Update()
    {

        if (mPlayerMovement.GetGroundedState())
            Animation_SetMovement();

        if (Input.GetButtonDown("Fire3"))
        {
            AnimationLayer_SwitchWeightForScarf();
            Animation_ThrowSnowball();  
        }

    }
}
