using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{

    private PlayerStateMachine mPlayerStateMachine = null;
    private PlayerMovement mPlayerMovement = null;

	// Use this for initialization
	void Start () {
        GetHeadAnimtorComponents();
        mPlayerStateMachine = transform.parent.GetComponent<PlayerStateMachine>();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
	}

    private void Animation_SetMovement()
    {
        if (mPlayerMovement.GetPlayerState() == PlayerStateMachine.PlayerState.Walk)
        {
            mAnimator.SetFloat("MovementSpeed", mPlayerMovement.GetMovementSpeed());
        }
    }

    private void Update()
    {
        Animation_SetMovement();
    }
}
