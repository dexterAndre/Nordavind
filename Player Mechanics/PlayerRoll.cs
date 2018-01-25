﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerRoll : MonoBehaviour
{
    /* 
        To do: 
    */


    #region Roll
    [Header("Roll")]
    [SerializeField]
    private float mRollSpeed = 10f;
    [SerializeField]
    private float mRollDuration = 0.05f;
    private float mRollTimer = 0f;
    [SerializeField]
    private float mRollDelayDuration = 0.035f;
    private float mRollDelayTimer = 0f;
    [SerializeField]
    private bool mRollSoftAdjustment = true;
    [Header("Cooldown")]
    [SerializeField]
    private float mRollCooldown = 2f;
    private float mRollCooldownTimer = 0f;
    [Header("Crash")]   // Make crash into a generic STUN feature
    [SerializeField]
    [Tooltip("How high up the raycast checks. ")]
    private float mCrashHeight = 0.25f;
    [SerializeField]
    [Tooltip("How far the raycast checks. ")]
    private float mCrashRayMagnitude = 0.5f;
    [SerializeField]
    [Tooltip("Adjustment to avoid hitting self. ")]
    private float mCrashRayMagnitudeSafeZone = 0.1f;
    [SerializeField]
    [Tooltip("How long the crash lasts. ")]
    private float mCrashDuration = 0.5f;
    [SerializeField]
    [Tooltip("How big the angle of impact must be to trigger a crash. Counts all angles above this value as well. ")]
    private float mCrashAngleLimit = 60f;
    //[SerializeField]
    //[Tooltip("How far the avatar goes flying backwards upon crashing.")]
    //private float mCrashPushDistance = 1f;
    private RaycastHit mCrashHit;
    private Vector3 mCrashPos0;
    [SerializeField]
    private LayerMask mCrashLayer;
    #endregion
    #region References
    [Header("References")]
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private CharacterController mCharacterController = null;
    #endregion
    #region Debug
    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingRoll = true;
    [SerializeField]
    private Color mDebugColor = new Color(255f, 125f, 255f, 255f);
    #endregion



    private void Awake()
    {
        //Physics.IgnoreLayerCollision(10, 2);

        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement>();
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>(); // optimize this! 
        if (mCharacterController == null)
            mCharacterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2")
            && mRollCooldownTimer == 0f
            && mPlayerMovement.GetState() == PlayerMovement.State.Walk)
        {
            // Debug
            if (mIsDebuggingRoll)
            {
                print("BUTTON PRESS: \t B. ");
            }

            Roll();

            // Debug
            if (mIsDebuggingRoll)
            {
                print("MAN TRANSITION: \t WALK \t -> \t ROLL. ");
            }
        }
    }

    private void FixedUpdate ()
	{
        // Ray start position
        mCrashPos0 =
            transform.position
            + new Vector3(0f, -mCharacterController.height / 2f + mCrashHeight, 0f)
            + transform.forward * mCharacterController.radius;

        // Cooldown
        if (mRollCooldownTimer > 0f)
        {
            mRollCooldownTimer += Time.fixedDeltaTime;
            if (mRollCooldownTimer >= mRollCooldown)
            {
                mRollCooldownTimer = 0f;
            }
        }
        
        // During roll
        if (mPlayerMovement.GetState() == PlayerMovement.State.Roll
            && mRollTimer > 0f)
        {
            // Check for crash
            if (mPlayerMovement.GetState() == PlayerMovement.State.Roll)
            {
                CheckCrash();
            }

            mRollTimer += Time.fixedDeltaTime;
            if (mRollTimer >= mRollDuration)
            {
                mRollTimer = 0f;
                mRollDelayTimer += Time.fixedDeltaTime;
                mPlayerMovement.SetState(PlayerMovement.State.RollDelay);

                // Debug
                if (mIsDebuggingRoll)
                {
                    print("AUTO TRANSITION: \t ROLL \t -> \t R DELAY. ");
                }
            }
        }

        // After rolling (some delay time to prevent spamming)
        if (mPlayerMovement.GetState() == PlayerMovement.State.RollDelay
            && mRollDelayTimer > 0f)
        {
            mPlayerMovement.SetState(PlayerMovement.State.RollDelay);  // Quick bug-fix! 
            mRollDelayTimer += Time.fixedDeltaTime;
            if (mRollDelayTimer >= mRollDelayDuration)
            {
                mRollDelayTimer = 0f;
                mPlayerMovement.SetState(PlayerMovement.State.Walk);

                // Pushes the CharacterController downwards to avoid transitioning to air. 
                // Now it transitions directly from ROLL DELAY to WALK. 
                mCharacterController.Move(Physics.gravity * Time.fixedDeltaTime);

                // Debug
                if (mIsDebuggingRoll)
                {
                    print("AUTO TRANSITION: \t R DELAY \t -> \t WALK. ");
                }
            }
        }
    }

    private void Roll()
    {
        // Executing roll
        mPlayerMovement.SetState(PlayerMovement.State.Roll);

        Vector3 inputVector;
        if (mRollSoftAdjustment)
            inputVector = PlayerMovement.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y));
        else
            inputVector = PlayerMovement.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y)).normalized;

        if (mInputManager.GetStickLeft().x != 0f || mInputManager.GetStickLeft().y != 0f)
            mPlayerMovement.SetMovementVector(inputVector * mRollSpeed);
        else
            mPlayerMovement.SetMovementVector(transform.forward * mRollSpeed);
        mRollTimer += Time.fixedDeltaTime;
    }

    private void CheckCrash()
    {
        //int layerMask = ~LayerMask.NameToLayer("Player");

        // If hitting something
        if (Physics.Raycast(
            mCrashPos0 + transform.forward * mCrashRayMagnitudeSafeZone, 
            transform.forward, 
            out mCrashHit, 
            mCrashRayMagnitude - mCrashRayMagnitudeSafeZone, 
            mCrashLayer))
        {
            print("HIT NORMAL: " + mCrashHit.normal);
            print("Angle between: " + Vector3.Angle(-transform.forward, mCrashHit.normal));
            // If normal is within crashing angle
            if (Vector3.Angle(mCrashHit.normal, -transform.forward) <= mCrashAngleLimit)
            {
                // Resetting variables
                mRollTimer = 0f;
                mRollDelayTimer = 0f;

                mPlayerMovement.Stun(-transform.forward, mCrashDuration);

                // Debug
                if (mIsDebuggingRoll)
                {
                    print("RAYCAST HIT: \t" + mCrashHit.transform.gameObject.name);
                    print("RAYCAST ANGLE: \t" + mCrashHit.normal);
                    print("AUTO TRANSITION: \t ROLL \t -> \t STUN. ");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = mDebugColor;

        mCrashPos0 =
            transform.position
            + new Vector3(0f, -mCharacterController.height / 2f + mCrashHeight, 0f)
            + transform.forward * mCharacterController.radius;

        Gizmos.DrawLine(
            mCrashPos0 + transform.forward * mCrashRayMagnitudeSafeZone,
            mCrashPos0 + transform.forward * mCrashRayMagnitude);
    }
}