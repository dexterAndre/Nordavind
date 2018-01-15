using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerStateMachine : MonoBehaviour
{
	[System.Serializable]
	public enum PlayerState
	{
		Walk,
		Air,
		Hang,
		Roll,
		Balance,
		Throw,
		Slide
	};
	[Header("Player State")]
	[SerializeField]
	private PlayerState mState = PlayerState.Walk;
	public PlayerState GetState() { return mState; }
	[SerializeField]
	private bool mCanHang = true;
	public bool GetHangability() { return mCanHang; }
	public void SetHangability(bool statement) { mCanHang = statement; }
	private CharacterController mCharacterController = null;
	private PlayerMovement mPlayerMovement = null;
	private PlayerCameraController mPlayerCameraController = null;
	private PlayerRoll mRoll = null;

	[Header("Input Multipliers")]
	[SerializeField]
	private float mInputStickLSensitivityX = 1f;
	[SerializeField]
	private float mInputStickLSensitivityY = 1f;
	[SerializeField]
	private float mInputStickRSensitivityX = 1f;
	[SerializeField]
	private float mInputStickRSensitivityY = 1f;
	[SerializeField]
	private float mInputTriggerLSensitivity = 1f;
	[SerializeField]
	private float mInputTriggerRSensitivity = 1f;
	[SerializeField]
	private float mDPadSensitivityX = 1f;
	[SerializeField]
	private float mDPadSensitivityY = 1f;

	[Header("Input Sensitivity")]
	public float mInputMovementSensitivityX = 1f;
	public float mInputMovementSensitivityY = 1f;
	public float mInputCameraSensitivityX = 1f;
	public float mInputCameraSensitivityY = 1f;
	public float mInputThrowSensitivityX = 1f;
	public float mInputThrowSensitivityY = 1f;

	private Vector2 mInputStickL;
	public Vector2 GetInputStickL() { return mInputStickL; }
	private Vector2 mInputStickR;
	public Vector2 GetInputStickR() { return mInputStickR; }
	private Vector2 mInputTriggers;
	public Vector2 GetInputTriggers() { return mInputTriggers; }
	private Vector2 mInputDPad;
	public Vector2 GetInputDPad() {return mInputDPad; }



	private void Start ()
	{
		mState = PlayerState.Walk;
		mCharacterController = GetComponent<CharacterController>();
		mPlayerMovement = GetComponent<PlayerMovement>();
		mPlayerCameraController = GetComponent<PlayerCameraController>();
		mRoll = GetComponent<PlayerRoll>();

		// Axis inversion
	}

	private void Update ()
	{
		// Left stick
		mInputStickL.x = Input.GetAxisRaw("Horizontal") * mInputStickLSensitivityX;
		mInputStickL.y = Input.GetAxisRaw("Vertical") * mInputStickLSensitivityY;
		// Right stick
		mInputStickR.x = Input.GetAxisRaw("RHorizontal") * mInputStickRSensitivityX;
		mInputStickR.y = Input.GetAxisRaw("RVertical") * mInputStickRSensitivityY;
		// D-Pad
		mInputDPad.x = Input.GetAxisRaw("DPadX") * mDPadSensitivityX;
		mInputDPad.y = Input.GetAxisRaw("DPadY") * mDPadSensitivityY;
		// Triggers
		mInputTriggers.x = Input.GetAxisRaw("TriggerL") * mInputTriggerLSensitivity;
		mInputTriggers.y = Input.GetAxisRaw("TriggerR") * mInputTriggerRSensitivity;
	}

	private void FixedUpdate ()
	{
		// Resetting avatar
		if (Input.GetKeyDown(KeyCode.Return))
		{
			mPlayerMovement.Reset();
		}

		// Setting air state
		if (!mCharacterController.isGrounded
		&& mState != PlayerState.Hang
		&& mState != PlayerState.Balance)
		{
			mState = PlayerState.Air;
		}

		// Air
		if (mState == PlayerState.Air)
		{
			// Walk
			if (mCharacterController.isGrounded)
			{
				// Switch to Walk state
				mState = PlayerState.Walk;
				mPlayerMovement.ResetAirTimer();
				SetHangability(true);
			}
			// Hang

			// Balance
		}
		// Walk
		else if (mState == PlayerState.Walk)
		{
			// Air
			if (!mCharacterController.isGrounded)
			{
				mState = PlayerState.Air;
			}
			// Roll
			else if (
				Input.GetButtonDown("Fire2")
				&& mRoll.GetRollCooldownTimer() == 0f)
			{
				mPlayerMovement.InitiateRoll();
			}
		}
		// Walk
		else if (mState == PlayerState.Walk )
		{
			// Throw
			if (GetInputTriggers().x != 0f)
			{
				mState = PlayerState.Throw;
				mPlayerCameraController.InitiateThrowCamera();
			}
		}
		// Throw
		else if (mState == PlayerState.Throw)
		{
			// Walk
			if (GetInputTriggers().x == 0f)
			{
				mState = PlayerState.Walk;
				mPlayerCameraController.InitiateThirdPersonCamera();
			}
		}
		// Hang
		else if (mState == PlayerState.Hang)
		{
			// Climb
			if (Input.GetButtonDown("Jump"))
			{
				StartCoroutine(mPlayerMovement.InitiateClimb());
			}
			// Drop
			if (Input.GetButtonDown("Fire2"))
			{
				mPlayerMovement.InitiateDrop();
			}
		}
		
	}

	public void SetState(PlayerState state) { mState = state; }
}