using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
/* 
	#########################
	######### TO-DO #########
	#########################
	- Auto-detect when changing from gamepad to mouse and keyboard
*/

	[System.Serializable]
	public enum PlayerState
	{
		Walk,
		Fall,
		Hang,
		Climb,
		Throw
	}

	// Temporary controls using a gamepad
	[Header("Input")]
	[SerializeField]
	private float mTriggersSensitivity = 1f;
	private float mInputTriggers;
	private Vector2 mInputL;
	private Vector2 mInputR;

	[Header("Player State")]
	[SerializeField]
	private PlayerState mState = PlayerState.Walk;

	[Header("Movement")]
	[SerializeField]
	private float mWalkSpeed = 5f;
	private CharacterController mCharacterController = null;
	private Vector3 mMovementVector;
	private Vector3 mMovementVelocity;
	private Vector3 mMovementGravity = new Vector3(0f, -9.81f, 0f);

	[Header("Rotation")]
	[SerializeField]
	private float mRotationSpeed = 1f;

	[Header("Jumping")]
	[SerializeField]
	private float mJumpForce = 1f;
	private bool mIsGrounded = true;
	private RaycastHit mGroundCheck;
	[SerializeField]
	private float mGroundCheckOffset = 0.1f;
	[SerializeField]
	private float mGroundCheckRadius = 0.5f;
	private Vector3 mGroundNormalCurrent;

	[Header("Dash / Roll")]
	[SerializeField]
	private float mDashSpeed = 10f;
	[SerializeField]
	private float mDashCooldown = 2f;
	private float mDashCooldownTimer = 0f;
	[SerializeField]
	private float mDashDuration = 0.05f;

	[Header("Hanging")]
	[SerializeField]
	private float mHangHeightHigh = 2f;
	[SerializeField]
	private float mHangHeightLow = 1f;
	[SerializeField]
	private float mHangHeightDelta = 0.25f;
	[SerializeField]
	private float mClimbAnimationDuration = 0.25f;
	
	[Header("Camera")]
	[SerializeField]
	private float mCameraDistance = 1f;
	[SerializeField]
	private CinemachineFreeLook mFreeLook = null;
	private Quaternion mCameraDirection;
	private float[] mRigRadii = new float[3];

	[Header("Debug")]
	[SerializeField]
	private bool mIsDebugging = true;
	[SerializeField]
	private Vector3 mDebugOffset = new Vector3(0f, 2f, 0f);
	private Vector3 mDebugHangPoint;
	[SerializeField]
	private Color mDebugColorHangTop, mDebugColorHangPoint, mDebugColorHandPlanar, mDebugColorHangDirection;



	private void Start ()
	{
		mCharacterController = GetComponent<CharacterController>();

		// Storing initial camera distances for all 3 camera rigs. 
		for (int i = 0; i < 3; i++)
		{
			mRigRadii[i] = mFreeLook.m_Orbits[i].m_Radius;
		}
	}

	private void Update ()
	{
		// Input
		mInputL = new Vector2
		(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
		mInputR = new Vector2
		(
			Input.GetAxisRaw("RHorizontal"),
			Input.GetAxisRaw("RVertical")
		);
		mInputTriggers = Input.GetAxisRaw("Triggers");
		mCameraDirection = Camera.main.transform.rotation;

		// Movement
		mMovementVector = new Vector3(mInputL.x, mMovementVector.y, mInputL.y);
		mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed * Time.deltaTime;

		// Camera controls
		// Camera distance (for testing only)
		mCameraDistance += mInputTriggers * mTriggersSensitivity;
		if (mCameraDistance < 0.0f)
			mCameraDistance = 0.0f;
		// [The right stick is configured within Cinemachine]

		// Storing Cinemachine rig radii. Used to preserve ratios between them 
		// when dollying out the camera. 
		for (int i = 0; i < 3; i++)
		{
			mFreeLook.m_Orbits[i].m_Radius = mRigRadii[i] * mCameraDistance;
		}

		// Applying movement
		mCharacterController.Move(mMovementVector);

		// Debugging
		VisualDebug();
	}

	private void FixedUpdate ()
	{
		
	}

	private void VisualDebug()
	{

	}
}