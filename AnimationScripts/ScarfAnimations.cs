using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarfAnimations : HeadAnimatorBehavior {

    private void Start()
    {
        GetHeadAnimtorComponents();
    }


    #region Movement

    public void Animation_SetMovement(float movementIn)
    {
        mAnimator.SetFloat("MovementSpeed", movementIn);
    }

    public void Animation_SetJump()
    {
        mAnimator.SetTrigger("Jump");
        //mAnimator.SetBool("IsGrounded", false);
    }

#endregion

}
