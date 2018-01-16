using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{

    private PlayerStateMachine mPlayerStateMachine = null;
    private PlayerMovement mPlayerMovement = null;

    //[SerializeField]
    //private ScarfAnimations mScarfAnimations = null;


    private bool hasJumped = false;

	// Use this for initialization
	void Start () {
        GetHeadAnimtorComponents();
        mPlayerStateMachine = transform.parent.GetComponent<PlayerStateMachine>();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
    }

    private void Animation_SetMovement()
    {
            mAnimator.SetFloat("MovementSpeed", mPlayerMovement.GetMovementSpeed());
    }

    private void Animation_SetJump()
    {
        mAnimator.SetTrigger("Jump");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            mAnimator.SetTrigger("Jump");
        }
        //if (mPlayerMovement.GetPlayerState() == PlayerStateMachine.PlayerState.Walk)
        //{

        //    else
        //        Animation_SetMovement();
                
        //}
    }
}
