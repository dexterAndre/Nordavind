using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{
    private PlayerMovement mPlayerMovement = null;

    private InputManager mInputManager = null;

    private bool hasJumped = false;


    #region Animator set-animationstates
    private void Animation_SetMovement()
    {
            mAnimator.SetFloat("Speed", mPlayerMovement.GetWalkSpeedNormalized());
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
    #endregion

    private bool rollUsed = false;
    private bool throwUsed = false;

    void Start()
    {
        GetHeadAnimtorComponents();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
        mInputManager = GameObject.Find("Managers").transform.GetChild(0).GetComponent<InputManager>();
    }

    private void Update()
    {
        
        if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
        {
            Animation_SetMovement();
          
        }


        if (!rollUsed && mPlayerMovement.GetState() == PlayerMovement.State.Roll)
        {
            mAnimator.SetTrigger("Roll");
            rollUsed = true;
        }
        else if (rollUsed && mPlayerMovement.GetState() != PlayerMovement.State.Roll)
        {
            rollUsed = false;
        }
            

        if (mInputManager.GetTriggers().y != 0f && !throwUsed)
        {
            StartCoroutine(ThrowCooldown());
            AnimationLayer_SwitchWeightForScarf();
            Animation_ThrowSnowball();
            throwUsed = true;
        }

    }

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        throwUsed = false;
    }
}
