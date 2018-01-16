using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{

    private PlayerStateMachine mPlayerStateMachine = null;
    private PlayerMovement mPlayerMovement = null;

    private ScarfAnimations mScarfAnimations = null;


    private bool hasJumped = false;

	// Use this for initialization
	void Start () {
        GetHeadAnimtorComponents();
        mPlayerStateMachine = transform.parent.GetComponent<PlayerStateMachine>();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
        //mScarfAnimations = transform.GetChild(3).transform.GetChild(16).GetComponent<ScarfAnimations>();
	}

    private void Animation_SetMovement()
    {
            //Setting animations for Rene.
            mAnimator.SetFloat("MovementSpeed", mPlayerMovement.GetMovementSpeed());
            //Setting animations for scarf.
           // mScarfAnimations.Animation_SetMovement(mPlayerMovement.GetMovementSpeed());
    }

    private void Animation_SetJump()
    {
        mAnimator.SetTrigger("Jump");
       // mAnimator.SetBool("IsGrounded", false);
       // mScarfAnimations.Animation_SetJump();
    }

    private void Update()
    {

        if (mPlayerMovement.GetPlayerState() == PlayerStateMachine.PlayerState.Walk)
        {
            if (!mPlayerMovement.GetGroundedState() && !hasJumped)
            {
                hasJumped = true;
                Animation_SetJump();
            }
            else if (hasJumped && mPlayerMovement.GetGroundedState())
            {
                hasJumped = false;
            }
            else
                Animation_SetMovement();
                
        }
    }
}
