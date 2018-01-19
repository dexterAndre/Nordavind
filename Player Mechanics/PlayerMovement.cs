using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateMachine), typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	/* 
	To-do list: 
	- Make jump lean more forward rather than up. 
	- Stop before jumping. 
	*/

	[Header("Walk")]
	[SerializeField]
	private float mWalkSpeed = 5f;
	[HideInInspector]
	public Vector3 mMovementVector;
	private Quaternion mCameraDirection;
	[SerializeField]
	[Range(0f, 1f)]
	private float mRotationSpeed = 0.3f;
    private Vector3 mRollVector;

	[Header("Gravity")]
	[SerializeField]
	private float mGravityScale = 1f;
	private float mAirTimer = 0f;
	public void ResetAirTimer() { mAirTimer = 0f; }
	private float mVerticalMovement = 0f;
	public bool mIsGrounded;

	[Header("Jump")]
	[SerializeField]
	private float mJumpStrength = 10f;

	[Header("Hang")]
	[SerializeField]
	private float mHangHeightHigh = 2f;
	[SerializeField]
	private float mHangHeightLow = 1f;
	[SerializeField]
	private float mHangDelta = 0.25f;
	[SerializeField]
	private float mHangBottom = 1f;
	[SerializeField]
	private float mHangMagnitude = 0.5f;
	private RaycastHit mHangRaycastHigh;
	private RaycastHit mHangRaycastLow;
	private RaycastHit mHangRaycastDown;
	private Vector3 mHangLimitHigh = Vector3.zero;
    private Vector3 mHangLimitLow = Vector3.zero;
    private Vector3 mHangTop = Vector3.zero;
    private Vector3 mHangWall = Vector3.zero;
    private Vector3 mHangPlanarTW = Vector3.zero;
    private Vector3 mHangPoint = Vector3.zero;
    private Vector3 mHangPointActually = Vector3.zero;
    private Vector3 mHangDirection = Vector3.zero;
    private Vector3 mHangDisplacement = Vector3.zero;
    private Vector3 mHangDisplacementFinal = Vector3.zero;
	[SerializeField]
	private float mClimbAnimationDuration;
	[SerializeField]
	private float mHangDropSpeed = 1f;

	[Header("Debug")]
	[SerializeField]
	private Vector3 mDebugOffset;
	[SerializeField]
	private Color mDebugColor = Color.red;

	// References
	private CharacterController mCharacterController = null;
	private PlayerStateMachine mStateMachine = null;
	private PlayerRoll mRoll = null;

	private void Start ()
	{
		mCharacterController = GetComponent<CharacterController>();
		mStateMachine = GetComponent<PlayerStateMachine>();
		mRoll = GetComponent<PlayerRoll>();

		mMovementVector = Vector3.zero;

		mAirTimer = 0f;
		mVerticalMovement = 0f;
	}

	private void FixedUpdate ()
	{
		// Jump
		if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Walk)
		{
			if (Input.GetButtonDown("Jump"))
			{
				mVerticalMovement = mJumpStrength;
			}
			else
			{
				mVerticalMovement = Physics.gravity.y;
			}
		}
		// Airborne
		else if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Air)
		{
			//// Check for hang
			//if (mStateMachine.GetHangability())
			//{
			//	// If upper raycast hits nothing...
			//	if (!Physics.Raycast(transform.position + mHangLimitHigh, transform.forward, out mHangRaycastHigh, mHangMagnitude))
			//	{
			//		// If inset raycast DOES hit something...
			//		if (Physics.Raycast(transform.position + mHangLimitHigh + transform.forward * mHangMagnitude, Vector3.down, out mHangRaycastDown, mHangDelta))
			//		{
			//			InitiateHang();
			//		}
			//	}
			//}
			//// Check for balance
			//else if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Balance)
			//{
			//	// This doesn't trigger yet. 
			//	// Remember to replace the condition when finished! 
			//}
			// Else, apply gravity
			if (
				mStateMachine.GetState() != PlayerStateMachine.PlayerState.Hang 
				&& mStateMachine.GetState() != PlayerStateMachine.PlayerState.Balance)
			{
				mAirTimer += Time.fixedDeltaTime;
				mVerticalMovement += Physics.gravity.y * mGravityScale * mAirTimer * mAirTimer;
			}
		}

		// Planar movement while walking or airborne
		if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Walk
			|| mStateMachine.GetState() == PlayerStateMachine.PlayerState.Air)
		{
			// Calculating planar movement
			mCameraDirection = Camera.main.transform.rotation;
			mMovementVector = new Vector3(mStateMachine.GetInputStickL().x, 0f, mStateMachine.GetInputStickL().y);
			mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up) * mWalkSpeed;

			// Applying rotation
			transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSpeed);
			
			// Calculating vertical movement
			mMovementVector += new Vector3(0f, mVerticalMovement, 0f);

			// Applying movement
			mCharacterController.Move(mMovementVector * Time.deltaTime);
		}
		// Roll
		else if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Roll)
		{
            mMovementVector = mRollVector + new Vector3(0f, -9.81f, 0f);

			// Applying movement
			mCharacterController.Move(mMovementVector * Time.deltaTime);
		}
		// Hang
		else if (mStateMachine.GetState() == PlayerStateMachine.PlayerState.Hang)
		{
			transform.position = mHangPointActually;
			transform.rotation = Quaternion.LookRotation(mHangDirection);
		}
		// Slide
		// Balance
	}

	public void Reset()
	{
		transform.position = new Vector3(0f, 2f, 0f);
		ResetAirTimer();
		mStateMachine.SetState(PlayerStateMachine.PlayerState.Walk);
		mVerticalMovement = 0f;
	}

	public void InitiateHang()
	{
		// Resetting values
		mAirTimer = 0f;
		mVerticalMovement = 0f;

		// Low raycast to obtain wall normal
		Physics.Raycast(transform.position + mHangLimitLow, transform.forward, out mHangRaycastLow, mHangMagnitude);

		/*
		T: top hit point
		W: wall hit point
		TW: vector from T to W
		The following code block projects TW into xz-plane
		*/
		mHangTop = mHangRaycastDown.point;
		mHangWall = mHangRaycastLow.point;
		mHangPlanarTW = Vector3.ProjectOnPlane(mHangWall - mHangTop, Vector3.up);
		mHangPoint = mHangTop + mHangPlanarTW;
		mHangDirection = -mHangRaycastLow.normal;
		mHangPointActually = mHangPoint - mHangDirection * mCharacterController.radius + Vector3.down * 1f;

		// Updating state
		mStateMachine.SetState(PlayerStateMachine.PlayerState.Hang);
	}

	public IEnumerator InitiateClimb()
	{
		// Applying wait time
		WaitForSeconds wait = new WaitForSeconds(mClimbAnimationDuration);
		yield return wait;
		transform.position = transform.position + Vector3.up * mCharacterController.height + mHangDirection * mCharacterController.radius * 2f;
		mStateMachine.SetState(PlayerStateMachine.PlayerState.Walk);
		// animator.SetBool("isHanging", isHanging);
        // animator.SetBool("isClimbing", isClimbing);
        // animator.Play("slide-ground-end");
		
		// Cleanup
		mHangTop = mHangWall = mHangPlanarTW = mHangPoint = mHangPointActually = Vector3.zero;
	}

	public void InitiateDrop()
	{
		mStateMachine.SetState(PlayerStateMachine.PlayerState.Air);
		mStateMachine.SetHangability(false);
		mVerticalMovement = -mHangDropSpeed;
	}

	public void InitiateRoll()
	{
		// Setting state
		mStateMachine.SetState(PlayerStateMachine.PlayerState.Roll);
		mRoll.StartRollTimer();
		mRoll.StartRollCooldownTimer();

		// Calculating planar movement
		mCameraDirection = Camera.main.transform.rotation;
		mMovementVector = new Vector3(mStateMachine.GetInputStickL().x, 0f, mStateMachine.GetInputStickL().y);
		mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mRoll.GetRollSpeed();

        mRollVector = mMovementVector;

		// Setting rotation
		transform.forward = mMovementVector.normalized;
	}

	private void OnDrawGizmosSelected()
	{
		mHangLimitHigh = new Vector3(0f, mHangHeightHigh, 0f);
		mHangLimitLow = new Vector3(0f, mHangHeightLow, 0f);
		Gizmos.color = mDebugColor;
		Gizmos.DrawLine(
			transform.position + mHangLimitHigh,
			transform.position + mHangLimitHigh + transform.forward * mHangMagnitude);
		Gizmos.DrawLine(
			transform.position + mHangLimitHigh + transform.forward * mHangMagnitude,
			transform.position + mHangLimitHigh + transform.forward + Vector3.down * mHangDelta);
		Gizmos.DrawLine(
			transform.position + mHangLimitLow,
			transform.position + mHangLimitLow + transform.forward * mHangMagnitude);

		if (mHangTop != Vector3.zero)
		{
			Gizmos.DrawLine(
				mHangTop,
				mHangTop + new Vector3(0f, 1f, 0f));
		}
		if (mHangPlanarTW != Vector3.zero)
		{
			Gizmos.DrawLine(
				mHangTop,
				mHangTop + mHangPlanarTW);
		}
		if (mHangPoint != Vector3.zero)
		{
			Gizmos.DrawLine(
				mHangWall,
				mHangWall - mHangDirection);
		}
		if (mHangPointActually != Vector3.zero)
		{
			Gizmos.DrawLine(
				mHangPointActually,
				mHangPointActually - mHangDirection);
		}
		if (mHangDirection != Vector3.zero)
		{
			Gizmos.DrawLine(
				transform.position + mDebugOffset,
				transform.position + mDebugOffset + mHangDirection);
		}
	}

    public float GetMovementSpeed()
    {
        return new Vector3(mMovementVector.x, 0f, mMovementVector.z).magnitude / mWalkSpeed;
    }

    public PlayerStateMachine.PlayerState GetPlayerState()
    {
        return mStateMachine.GetState();
    }

    public bool GetGroundedState()
    {
        return mCharacterController.isGrounded;
    }

}