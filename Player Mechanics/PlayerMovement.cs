using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateMachine), typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Walk")]
	[SerializeField]
	private float mWalkSpeed = 5f;
	[HideInInspector]
	public Vector3 mMovementVector;
	private Quaternion mCameraDirection;
	[SerializeField]
	[Range(0f, 1f)]
	private float mRotationSpeed = 0.3f;

	[Header("Gravity")]
	[SerializeField]
	private float mGravityScale = 1f;
	[HideInInspector]
	public float mAirTimer = 0f;
	[HideInInspector]
	public float mVerticalMovement = 0f;
	public bool mIsGrounded;

	// References
	private CharacterController mCharacterController;
	private PlayerStateMachine mStateMachine;

	private void Start ()
	{
		mCharacterController = GetComponent<CharacterController>();
		mStateMachine = GetComponent<PlayerStateMachine>();

		mMovementVector = Vector3.zero;

		mAirTimer = 0f;
		mVerticalMovement = 0f;
	}

	private void FixedUpdate ()
	{
		if (!mCharacterController.isGrounded)
		{
			mIsGrounded = false;
			// Quadratic gravity movement
			mAirTimer += Time.fixedDeltaTime;
			mVerticalMovement += Physics.gravity.y * mGravityScale * mAirTimer * mAirTimer;
		}
		else
		{
			mIsGrounded = true;
			// Constant downwards movement
			mVerticalMovement = Physics.gravity.y;
		}

		// Calculating planar movement
		mCameraDirection = Camera.main.transform.rotation;
		mMovementVector = new Vector3(mStateMachine.mInputStickL.x, 0f, mStateMachine.mInputStickL.y);
		mMovementVector = Vector3.ProjectOnPlane((mCameraDirection * mMovementVector), Vector3.up).normalized * mWalkSpeed;

		// Applying rotation
		transform.forward = Vector3.Slerp(transform.forward, mMovementVector.normalized, mRotationSpeed);
		
		// Calculating vertical movement
		mMovementVector += new Vector3(0f, mVerticalMovement, 0f);

		// Applying movement
		mCharacterController.Move(mMovementVector * Time.deltaTime);
	}
}