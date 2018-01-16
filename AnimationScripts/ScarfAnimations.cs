using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarfAnimations : MonoBehaviour {

    private Animator scarftAnimator = null;

    private void Start()
    {
        scarftAnimator = GetComponent<Animator>();
    }


    #region Movement

    public void Animation_SetMovement(float movementIn)
    {
        scarftAnimator.SetFloat("MovementSpeed", movementIn);
    }

    public void Animation_SetJump()
    {
        scarftAnimator.SetTrigger("Jump");
        //mAnimator.SetBool("IsGrounded", false);
    }

#endregion

}
