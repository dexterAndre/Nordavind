﻿using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
/* 
	#########################
	######### TO-DO #########
	#########################
	- Mouse and keyboard support
	- Auto-detect when changing from gamepad to mouse and keyboard
	- Get rid of mIsGrounded
*/

#region Input
	// Temporary controls using a gamepad
	[Header("Input")]
	// [SerializeField]
	// private float mTriggersSensitivity = 1f;
	private float mInputTriggerL;
	// private float mInputTriggerR;
	[SerializeField]
	private float mDPadSensitivity = 1f;
	// private float mInputDPadX;
	private float mInputDPadY;
	private Vector2 mInputLStick;
	// private Vector2 mInputRStick;
#endregion

#region Player State
	[System.Serializable]
	public enum PlayerState
	{
		Walk,
		Fall,
		Roll,
		Hang,
		Climb,
		Throw
	}
	[Header("Player State")]
	[SerializeField]
	private PlayerState mState = PlayerState.Walk;
#endregion

#region Movement
	[Header("Movement")]
	[SerializeField]
	private float mWalkSpeed = 5f;
	private CharacterController mCharacterController = null;
	private Vector3 mMovementVector;
	private Vector3 mMovementVelocity;
#endregion

#region Rotation
	[Header("Rotation")]
	[SerializeField]
	private float mRotationSlerpParameter = 0.3f;
#endregion

#region Jumping
	// [Header("Jumping")]
	// [SerializeField]
	// private float mJumpForce = 1f;
	// private float mJumpTimer = 0f;
	// private float mVerticalMovement = 0f;
	// [SerializeField]
	// private bool mIsGrounded = true;
	// private RaycastHit mGroundCheck;
	// [SerializeField]
	// private float mGroundCheckOffset = 0.1f;
	// [SerializeField]
	// private float mGroundCheckRadius = 0.5f;
	// private Vector3 mGroundNormalCurrent;
	// private float mGravity = -9.81f;
	// [SerializeField]
	// private float mGravityScale = 1f;
#endregion

#region Roll
	[Header("Roll")]
	[SerializeField]
	private float mRollSpeed = 10f;
	[SerializeField]
	private float mRollCooldown = 2f;
	[SerializeField]
	private float mRollCooldownTimer = 0f;
	[SerializeField]
	private float mRollDuration = 0.05f;
	[SerializeField]
	private float mRollTimer = 0f;
	private Vector3 mRollDirection;
	/// <summary> 
	/// Resets timers, updates player state, and sets mMovementVector. 
	/// </summary>
	private void InitiateRoll()
	{
		mRollCooldownTimer = 0f;
		mRollTimer = 0f;
		mState = PlayerState.Roll;

		mMovementVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up) * mRollSpeed;
	}
#endregion

#region Hanging
	// [Header("Hanging")]
	// [SerializeField]
	// private float mHangHeightHigh = 2f;
	// [SerializeField]
	// private float mHangHeightLow = 1f;
	// [SerializeField]
	// private float mHangHeightDelta = 0.25f;
	// [SerializeField]
	// private float mClimbAnimationDuration = 0.25f;
#endregion
	
#region Camera
	[Header("Camera")]
	[SerializeField]
	private float mCameraDistance = 1f;
	[SerializeField]
	private CinemachineFreeLook mFreeLook = null;
	[SerializeField]
	private GameObject mCameraPositionThrow = null;
	private Quaternion mCameraDirection;
	private float[] mRigRadii = new float[3];
#endregion

#region Debug
	// [Header("Debug")]
	// [SerializeField]
	// private bool mIsDebugging = true;
	// [SerializeField]
	// private Vector3 mDebugOffset = new Vector3(0f, 2f, 0f);
	// private Vector3 mDebugHangPoint;
	// [SerializeField]
	// private Color mDebugColorHangTop, mDebugColorHangPoint, mDebugColorHandPlanar, mDebugColorHangDirection;
	
	/// <summary> 
	/// Runs the debug code. 
	/// </summary>
	/// <param name="other"></param>
	private void VisualDebug()
	{

	}
#endregion

#region Update Functions
	private void Start ()
	{
		mCharacterController = GetComponent<CharacterController>();
		mRollCooldownTimer = mRollCooldown;
		mRollTimer = 0f;

		// Storing initial camera distances for all 3 camera rigs. 
		for (int i = 0; i < 3; i++)
		{
			mRigRadii[i] = mFreeLook.m_Orbits[i].m_Radius;
		}
	}

	private void Update ()
	{
	#region Input
		mInputLStick = new Vector2
		(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
		// mInputRStick = new Vector2
		// (
		// 	Input.GetAxisRaw("RHorizontal"),
		// 	Input.GetAxisRaw("RVertical")
		// );
		// mInputDPadX = Input.GetAxisRaw("DPadX");
		mInputDPadY = Input.GetAxisRaw("DPadY");
		mInputTriggerL = Input.GetAxisRaw("TriggerL");
	#endregion



	#region State Machine
	// Roll state
	if (mRollCooldownTimer < mRollCooldown)
	{
		mRollCooldownTimer += Time.deltaTime;
		if (mRollCooldownTimer >= mRollCooldown)
		{
			mRollCooldownTimer = mRollCooldown;
		}
	}
	if (Input.GetButtonDown("Fire3") && mRollCooldownTimer >= mRollCooldown)
	{
		InitiateRoll();
	}
	if (mState == PlayerState.Roll)
	{
		// Handling timers
		if (mRollTimer < mRollDuration)
		{
			mRollTimer += Time.deltaTime;
			if (mRollTimer >= mRollDuration)
			{
				mRollTimer = 0f;
				mState = PlayerState.Walk;
			}
		}
	}
	// Throw state
	else if (mInputTriggerL != 0f)
	{
		mState = PlayerState.Throw;
		mFreeLook.m_Follow = mCameraPositionThrow.transform;
		mFreeLook.m_LookAt = mCameraPositionThrow.transform;
		transform.forward = mCameraDirection.eulerAngles;
	}
	// Walk state by default
	else
	{
		mState = PlayerState.Walk;
		mFreeLook.m_Follow = transform;
		mFreeLook.m_LookAt = transform;
	}
	#endregion

	#region Walk State
	if (mState == PlayerState.Walk)
	{
	#region Camera Controls
		// Camera distance (for testing only)
		mCameraDistance += mInputDPadY * mDPadSensitivity;
		if (mCameraDistance < 0.0f)
			mCameraDistance = 0.0f;
		// Setting camera dolly distances for all 3 rigs. 
		if (mInputDPadY != 0.0f)
		{
			for (int i = 0; i < 3; i++)
			{
				mFreeLook.m_Orbits[i].m_Radius = mRigRadii[i] * mCameraDistance;
			}
		}
	#endregion

	#region Movement
		// Calculating movement vector
		mCameraDirection = Camera.main.transform.rotation;
		mMovementVector = new Vector3(mInputLStick.x, 0f, mInputLStick.y);
		mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed;

		// Applying movement
		transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSlerpParameter);
		mMovementVector += new Vector3(0f, /*mVerticalMovement*/ 0f, 0f);
		mCharacterController.Move(mMovementVector * Time.deltaTime);
	#endregion
	}
	#endregion
	#region Throw State
	else if (mState == PlayerState.Throw)
	{
	#region Camera Controls
		mCameraDirection = Camera.main.transform.rotation;
		mFreeLook.m_Follow = mCameraPositionThrow.transform;
		mFreeLook.m_LookAt = mCameraPositionThrow.transform;
		
	#endregion

	#region Movement
		// Calculating movement vector
		mMovementVector = new Vector3(mInputLStick.x, 0f, mInputLStick.y);
		mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed;

		// Applying movement
		transform.forward = Vector3.ProjectOnPlane(mCameraDirection.eulerAngles.normalized, Vector3.up);
	#endregion
	}
	#endregion

	#region Roll
	if (mState == PlayerState.Roll)
	{
	#region Camera Controls
		// Camera distance (for testing only)
		mCameraDistance += mInputDPadY * mDPadSensitivity;
		if (mCameraDistance < 0.0f)
			mCameraDistance = 0.0f;
		// Setting camera dolly distances for all 3 rigs. 
		if (mInputDPadY != 0.0f)
		{
			for (int i = 0; i < 3; i++)
			{
				mFreeLook.m_Orbits[i].m_Radius = mRigRadii[i] * mCameraDistance;
			}
		}
	#endregion

	#region Movement
		// Applying movement
		mCameraDirection = Camera.main.transform.rotation;
		transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSlerpParameter);
		mMovementVector += new Vector3(0f, /*mVerticalMovement*/ 0f, 0f);
		mCharacterController.Move(mMovementVector * Time.deltaTime);
	#endregion
	}
	#endregion

		// Debugging
		VisualDebug();
	}

	private void FixedUpdate ()
	{

	}
#endregion
}