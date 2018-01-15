using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerCameraController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private CinemachineFreeLook mCameraFreeLook = null;
	private PlayerStateMachine mStateMachine = null;
	[SerializeField]
	private Transform mCameraStandardFocus = null;
	public Transform GetCameraStandardFocus() { return mCameraStandardFocus; }
	[SerializeField]
	private Transform mCameraThrowPosition = null;
	public Transform GetCameraThrowPosition() { return mCameraThrowPosition; }
	[SerializeField]
	private Transform mCameraThrowLookAt = null;
	public Transform GetCameraThrowLookAt() { return mCameraThrowLookAt; }
	[SerializeField]
	private Transform mCameraFreeThrowPosition = null;
	public Transform GetCameraFreeThrowPosition() { return mCameraFreeThrowPosition; }
	[SerializeField]
	private Transform mCameraFreeThrowLookAt = null;
	public Transform GetCameraFreeThrowLookAt() { return mCameraFreeThrowLookAt; }
	private float mCameraThrowInitialRadius;

	[Header("Bounds")]
	private float mCameraHorizontalParameter = 0f;
	[SerializeField]
	private Vector2 mCameraVerticalParameterBounds;
	private float mCameraVerticalParameter = 0.25f;
	[SerializeField]
	private Vector2 mCameraThrowVerticalBounds;
	private float mCameraThrowVerticalAngle = 0f;

	private void Start ()
	{
        mCameraFreeLook = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
		mStateMachine = GetComponent<PlayerStateMachine>();
		mCameraThrowInitialRadius = (mCameraThrowLookAt.position - mCameraThrowPosition.position).magnitude;
	}

	private void Update ()
	{
		// Third-person camera mode
		if (mStateMachine.GetState() != PlayerStateMachine.PlayerState.Throw)
		{
			// Storing input
			mCameraHorizontalParameter
				= mStateMachine.GetInputStickR().x
				* mStateMachine.mInputCameraSensitivityX;
			mCameraVerticalParameter
				+= mStateMachine.GetInputStickR().y
				* mStateMachine.mInputCameraSensitivityY;

			// Clamping vertical input within mCameraVerticalParameterBounds
			if (mCameraVerticalParameter < mCameraVerticalParameterBounds.x)
				mCameraVerticalParameter = mCameraVerticalParameterBounds.x;
			else if (mCameraVerticalParameter > mCameraVerticalParameterBounds.y)
				mCameraVerticalParameter = mCameraVerticalParameterBounds.y;
			
			// Updating camera
			mCameraFreeLook.m_XAxis.m_InputAxisValue = mCameraHorizontalParameter;
			mCameraFreeLook.m_YAxis.Value = mCameraVerticalParameter;
		}
		// Over-the-shoulder aim mode
		else
		{
			// Horizontal rotation
			transform.Rotate(0f, mStateMachine.GetInputStickR().x * mStateMachine.mInputThrowSensitivityX, 0f);

			// Setting vertical angle and clamping within mCameraThrowVerticalBounds
			mCameraThrowVerticalAngle += mStateMachine.GetInputStickR().y * mStateMachine.mInputThrowSensitivityY;
			if (mCameraThrowVerticalAngle < mCameraThrowVerticalBounds.x)
				mCameraThrowVerticalAngle = mCameraThrowVerticalBounds.x;
			else if (mCameraThrowVerticalAngle > mCameraThrowVerticalBounds.y)
				mCameraThrowVerticalAngle = mCameraThrowVerticalBounds.y;
			
			// Applying vertical rotation
			mCameraThrowLookAt.position 
				= mCameraThrowPosition.position
				+ transform.forward * Mathf.Cos(mCameraThrowVerticalAngle * Mathf.Deg2Rad) * mCameraThrowInitialRadius
				+ Vector3.up * Mathf.Sin(mCameraThrowVerticalAngle * Mathf.Deg2Rad) * mCameraThrowInitialRadius * (-1f);
		}
	}

	public void InitiateThirdPersonCamera()
	{
		mCameraFreeLook.m_YAxis.Value = mCameraVerticalParameter;
		mCameraFreeLook.Follow = transform;
		mCameraFreeLook.LookAt = mCameraStandardFocus;
	}

	public void InitiateThrowCamera()
	{
		mCameraFreeLook.m_YAxis.Value = 1f;
		mCameraFreeLook.Follow = mCameraThrowPosition;
		mCameraFreeLook.LookAt = mCameraThrowLookAt;
	}
}