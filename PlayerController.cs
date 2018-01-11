using UnityEngine;
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
	- Fix camera jitter when rolling
	- Roll and crash
	- Camera controls only if elgible (e.g. not while throwing)
	- Limit movement over edges to not fall down (especially if throwing)
	- Camera: needs to have a better look upwards when close to avatar
	- Dynamic input stick inversion
*/

#region Input
	// Temporary controls using a gamepad. Will extend to mouse and keyboard later. (also Rewired?)
	[Header("Input")]
	[SerializeField]
	private float mDPadXSensitivity = 1f;
	[SerializeField]
	private float mDPadYSensitivity = 1f;
	private float mInputDPadX;
	private float mInputDPadY;
	// [SerializeField]
	// private float mTriggersSensitivity = 1f;
	private float mInputTriggerL;
	// private float mInputTriggerR;
	private Vector2 mInputLStick;
	[SerializeField]
	private float mInputRStickSensitivity = 1f;
	private Vector2 mInputRStick;
#endregion
#region Player State
	[System.Serializable]
	public enum PlayerState
	{
		Walk,
		Air,
		Roll,
		Hang,
		Climb,
		Throw, 
		Slide,
		Balance
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
	private void InitiateWalk()
	{
		mState = PlayerState.Walk;
		mAirTimer = 0f;

		// Setting camera mode (third person follow-cam)
		mFreeLook.m_Follow = transform;
		mFreeLook.m_LookAt = transform;
		mFreeLook.m_YAxis.Value = 0.5f;
	}
#endregion
#region Rotation
	[Header("Rotation")]
	[SerializeField]
	private float mRotationSlerpParameter = 0.3f;
#endregion
#region Jumping
	[Header("Jumping")]
	// [SerializeField]
	// private float mJumpForce = 1f;
	private float mAirTimer = 0f;
	private float mVerticalMovement = 0f;
	// [SerializeField]
	// private float mAirMovementInfluence = 1f;
	// private RaycastHit mGroundCheck;
	// [SerializeField]
	// private float mGroundCheckOffset = 0.1f;
	// [SerializeField]
	// private float mGroundCheckRadius = 0.5f;
	// private Vector3 mGroundNormalCurrent;
	// [SerializeField]
	// private float mGravityScale = 1f;
	private void InitiateAir()
	{
		mState = PlayerState.Air;

		// Setting camera mode (third person follow-cam)
		mFreeLook.m_Follow = transform;
		mFreeLook.m_LookAt = transform;
	}
#endregion
#region Throw
[SerializeField]
private float mThrowVerticalAngleMinimum = -90f;
[SerializeField]
private float mThrowVerticalAngleMaximum = 90f;
private float mVerticalAngle = 0f;
private Vector3 mCameraThrowLookAtOriginalDisplacement;
private float mCameraThrowLookAtRadius;
void InitiateThrow()
{
	mState = PlayerState.Throw;

	// Setting camera mode (over-the-shoulder aiming)
	mFreeLook.m_Follow = mCameraThrowShoulder.transform;
	mFreeLook.m_LookAt = mCameraThrowLookAt.transform;

	mFreeLook.m_YAxis.Value = 1f;
}
#endregion
#region Roll
	[Header("Roll")]
	[SerializeField]
	private float mRollSpeed = 10f;
	[SerializeField]
	private float mRollCooldown = 2f;
	private float mRollCooldownTimer = 0f;
	[SerializeField]
	private float mRollDuration = 0.05f;
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

		transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSlerpParameter);

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
#region Balancing
#endregion
#region Sliding
#endregion
#region Camera
	[Header("Camera")]
	[SerializeField]
	private float mCameraDistance = 1f;
	private float mCameraParameter = 0.25f;	// In-between top and bottom view. 
	[SerializeField]
	private CinemachineFreeLook mFreeLook = null;
	[SerializeField]
	private GameObject mCameraThrowShoulder = null;
	[SerializeField]
	private GameObject mCameraThrowLookAt = null;
	private Quaternion mCameraDirection;
	private float[] mRigRadii = new float[3];
#endregion
#region Animation
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

#region Updating
	private void Start ()
	{
		mCharacterController = GetComponent<CharacterController>();
		mRollCooldownTimer = mRollCooldown;
		mRollTimer = 0f;

		// Storing the displacement vector from Shoulder to ShoulderLookAt GameObjects in scene. 
		mCameraThrowLookAtOriginalDisplacement = mCameraThrowLookAt.transform.position - mCameraThrowShoulder.transform.position;
		// Storing radius of original displacement from Shoulder to ShoulderLookAt
		mCameraThrowLookAtRadius = mCameraThrowLookAtOriginalDisplacement.magnitude;

		// Default mode of locomotion
		InitiateWalk();

		// Storing initial camera distances for all 3 camera rigs. 
		// Used as reference when dollying the camera in and out. 
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
		mInputRStick = new Vector2
		(
			Input.GetAxisRaw("RHorizontal"),
			Input.GetAxisRaw("RVertical")
		);
		mInputDPadX = Input.GetAxisRaw("DPadX");
		mInputDPadY = Input.GetAxisRaw("DPadY");
		mInputTriggerL = Input.GetAxisRaw("TriggerL");

		// Camera distance (for testing purposes only)
		if (mState != PlayerState.Throw)
		{
			mCameraDistance += mInputDPadX * mDPadXSensitivity;
			if (mCameraDistance < 0.0f)
				mCameraDistance = 0.0f;
			mCameraParameter += mInputDPadY * mDPadYSensitivity;
			if (mCameraParameter < 0.0f)
				mCameraParameter = 0.0f;
			else if (mCameraParameter > 0.5f)
				mCameraParameter = 0.5f;
			// Setting camera dolly distances for all 3 rigs. 
			if (mInputDPadX != 0.0f)
			{
				for (int i = 0; i < 3; i++)
				{
					mFreeLook.m_Orbits[i].m_Radius = mRigRadii[i] * mCameraDistance;
				}
			}
		}
	#endregion
	#region Cooldowns
	if (mRollCooldownTimer < mRollCooldown)
	{
		mRollCooldownTimer += Time.deltaTime;
		if (mRollCooldownTimer >= mRollCooldown)
		{
			mRollCooldownTimer = mRollCooldown;
		}
	}
	#endregion
	#region States' Transitions
		// Can fall from any state
		if (!mCharacterController.isGrounded)
		{
			InitiateAir();

			// This is unfinished. You're also not grounded while hanging. 
			// Fix this later. 
		}

		// Checking transition conditions for current state
		if (mState == PlayerState.Walk)
		{
			// Walk-to-air (jump)
			if (false)
			{
				
			}
			// Walk-to-air (fall)
			else if (false)
			{

			}
			// Walk-to-roll
			else if (Input.GetButtonDown("Fire3") && mRollCooldownTimer >= mRollCooldown)
			{
				InitiateRoll();
			}
			// Walk-to-throw
			else if (mInputTriggerL != 0f)
			{
				InitiateThrow();
			}
		}
		else if (mState == PlayerState.Air)
		{
			// Air-to-walk
			if (mCharacterController.isGrounded)
			{
				InitiateWalk();
			}

			// Air-to-hang

			// Air-to-balance
		}
		else if (mState == PlayerState.Roll)
		{
			// Roll-to-walk
			if (mRollTimer >= mRollDuration)
			{
				InitiateWalk();
			}

			// Roll-to-walk (crash)

			// Roll-to-air
		}
		else if (mState == PlayerState.Throw)
		{
			// Throw-to-walk
			if (Mathf.Abs(mInputTriggerL) < 0.1f)
			{
				InitiateWalk();
			}
		}
		else if (mState == PlayerState.Hang)
		{
			// Hang-to-walk (climb)

			// Hang-to-air

			// Hang-to-sidle
		}
		else if (mState == PlayerState.Climb)
		{
			// Climb-to-walk
		}
		else if (mState == PlayerState.Balance)
		{
			// Balance-to-walk

			// Balance-to-air
		}
		else if (mState == PlayerState.Slide)
		{
			// Slide-to-walk

			// Slide-to-walk (crash)

			// Slide-to-air
		}
		else
		{
			
		}
	#endregion
	#region Movement
		if (mState == PlayerState.Walk)
		{
			// Camera controls
			// Setting camera distance
			mCameraDistance += mInputDPadX * mDPadXSensitivity;
			if (mCameraDistance < 0.0f)
				mCameraDistance = 0.0f;

			// Setting camera curve parameter
			mFreeLook.m_YAxis.Value = mCameraParameter;

			// Calculating movement vector
			mCameraDirection = Camera.main.transform.rotation;
			mMovementVector = new Vector3(mInputLStick.x, 0f, mInputLStick.y);
			mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed;

			// Constant downwards movement
			mVerticalMovement = Physics.gravity.y;

			// Applying rotation
			transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSlerpParameter);
			
			// Applying movement
			mMovementVector += new Vector3(0f, mVerticalMovement, 0f);
			mCharacterController.Move(mMovementVector * Time.deltaTime);
		}
		else if (mState == PlayerState.Air)
		{ // RIGHT NOW JUST A COPY OF WALK. NEEDS TWEAKING!!! 

			// Third person camera controls
			// Distance control (for testing only)
			mCameraDistance += mInputDPadX * mDPadXSensitivity;
			if (mCameraDistance < 0.0f)
				mCameraDistance = 0.0f;
			// Setting camera dolly distances for all 3 rigs. 
			if (mInputDPadY != 0.0f)
			{
				// for (int i = 0; i < 3; i++)
				// {
				// 	mFreeLook.m_Orbits[i].m_Radius = mRigRadii[i] * mCameraDistance;
				// }
			}

			// Calculating movement vector
			mCameraDirection = Camera.main.transform.rotation;
			mMovementVector = new Vector3(mInputLStick.x, 0f, mInputLStick.y);
			mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed;

			// Quadratic downwards movement (like real-life)
			mAirTimer += Time.deltaTime;
			mVerticalMovement = Physics.gravity.y * mAirTimer * mAirTimer;

			// Applying rotation
			transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSlerpParameter);
			
			// Applying movement
			mMovementVector += new Vector3(0f, mVerticalMovement, 0f);
			mCharacterController.Move(mMovementVector * Time.deltaTime);
		}
		else if (mState == PlayerState.Roll)
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
			
			// Applying movement
			mCharacterController.Move(mMovementVector * Time.deltaTime);
		}
		else if (mState == PlayerState.Throw)
		{
			if (Mathf.Abs(mInputRStick.x) > 0.19f)
			{
				transform.Rotate(0f, mInputRStick.x * mInputRStickSensitivity, 0f);
			}
			// Rotating the throw mode LookAt object vertically. 
			if (Mathf.Abs(mInputRStick.y) > 0.19f)
			{
				// Controlling the angle and clamping between [-90, 90]
				mVerticalAngle += mInputRStick.y * mInputRStickSensitivity;
				if (mVerticalAngle < mThrowVerticalAngleMinimum)
					mVerticalAngle = mThrowVerticalAngleMinimum;
				else if (mVerticalAngle > mThrowVerticalAngleMaximum)
					mVerticalAngle = mThrowVerticalAngleMaximum;

				// Applying rotation
				mCameraThrowLookAt.transform.position 
					= mCameraThrowShoulder.transform.position 
					+ transform.forward * Mathf.Cos(mVerticalAngle * Mathf.Deg2Rad) * mCameraThrowLookAtRadius
					+ Vector3.up * Mathf.Sin(mVerticalAngle * Mathf.Deg2Rad) * mCameraThrowLookAtRadius * (-1f);
			}
		}
		else if (mState == PlayerState.Hang)
		{
			
		}
		else if (mState == PlayerState.Climb)
		{
			
		}
		else if (mState == PlayerState.Balance)
		{
			
		}
		else if (mState == PlayerState.Slide)
		{
			
		}
		else
		{

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