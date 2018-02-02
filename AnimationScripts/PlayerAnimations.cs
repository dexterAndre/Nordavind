using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : HeadAnimatorBehavior
{
    private PlayerMovement mPlayerMovement = null;

    private InputManager mInputManager = null;

    private bool hasJumped = false;

    private ParticlesPlayer mParticles = null;


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

    #region Particles

    public void Particles_StartLeftFoot()
    {
        mParticles.LeftFoot_Start();
    }


    public void Particles_StartRightFoot()
    {
        mParticles.RightFoot_Start();
    }

    #endregion

    private bool rollUsed = false;
    private bool throwUsed = false;
    private bool jumpUsed = false;
    private bool impactHit = false;

    void Start()
    {
        GetHeadAnimtorComponents();
        mPlayerMovement = transform.parent.GetComponent<PlayerMovement>();
        mInputManager = GameObject.Find("Managers").transform.GetChild(0).GetComponent<InputManager>();
        mParticles = transform.parent.transform.GetChild(4).GetComponent<ParticlesPlayer>();
    }

    private void Update()
    {
        switch (mPlayerMovement.GetState())
        {
            case PlayerMovement.State.Walk:
                {
                    if (mAnimator.GetBool("isAiming"))
                    {
                        mAnimator.SetBool("isAiming", false);
                    }
                    Animation_SetMovement();
                    break;
                }
            case PlayerMovement.State.Air:
                {
                    
                    break;
                }

            case PlayerMovement.State.Jump:
                {
                    if (!jumpUsed)
                    {
                        jumpUsed = true;
                        mAnimator.SetTrigger("Jump");
                    }
                    break;
                }
            case PlayerMovement.State.JumpDelay:
                {
                    
                    
                    break;
                }
            case PlayerMovement.State.Roll:
                {
                    if (!rollUsed)
                    {
                        mAnimator.SetTrigger("Roll");
                        rollUsed = true;
                    }
                    break;
                }
            case PlayerMovement.State.RollDelay:
                {
                    break;
                }
            case PlayerMovement.State.Stun:
                {
                    if (!impactHit)
                    {
                        mParticles.Impact_Start();
                        impactHit = true;
                        mAnimator.SetTrigger("Stun");
                    }

                    break;
                }
            case PlayerMovement.State.StunRecover:
                {
                    if (!impactHit)
                    {
                        mParticles.Impact_Start();
                        impactHit = true;
                        mAnimator.SetTrigger("Stun");
                    }
                    break;
                }
            case PlayerMovement.State.Throw:
                {
                    if (!mAnimator.GetBool("isAiming"))
                        mAnimator.SetBool("isAiming", true);

                    mAnimator.SetFloat("StrafeX", mInputManager.GetTriggers().x);
                    mAnimator.SetFloat("StrafeY", mInputManager.GetTriggers().y);
                    if (new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y).magnitude <= 0.01f)
                    {
                        mAnimator.SetFloat("StrafeX", 0f);
                        mAnimator.SetFloat("StrafeY", 0f);
                    }
                    break;
                }
            case PlayerMovement.State.Hang:
                {
                    break;
                }
            case PlayerMovement.State.Balance:
                {
                    break;
                }
            case PlayerMovement.State.Slide:
                {
                    break;
                }
            default:
                {
                    print("This should never trigger!");
                    break;
                }
        }

        if (mPlayerMovement.GetState() != PlayerMovement.State.JumpDelay && mPlayerMovement.GetState() != PlayerMovement.State.Jump && mPlayerMovement.GetState() != PlayerMovement.State.Air && jumpUsed)
        {
            jumpUsed = false;
        }


        if (mInputManager.GetTriggers().y != 0f && !throwUsed)
        {
            StartCoroutine(ThrowCooldown());
            AnimationLayer_SwitchWeightForScarf();
            Animation_ThrowSnowball();
            throwUsed = true;
        }
        if (rollUsed && mPlayerMovement.GetState() != PlayerMovement.State.Roll && mPlayerMovement.GetState() != PlayerMovement.State.Stun)
        {
            rollUsed = false;
        }
        if (impactHit && mPlayerMovement.GetState() != PlayerMovement.State.Stun && mPlayerMovement.GetState() != PlayerMovement.State.StunRecover)
            impactHit = false;


    }

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        throwUsed = false;
    }
}
