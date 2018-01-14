using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerStateMachine))]
public class PlayerCameraController : MonoBehaviour
{
	[Header("Regerences")]
	[SerializeField]
	private CinemachineFreeLook mCameraFreeLook = null;
	private PlayerStateMachine mStateMachine = null;

	[Header("Bounds")]
	[SerializeField]
	private Vector2 mCameraHorizontalParameterBounds;
	private float mCameraHorizontalParameter = 0.25f;
	[SerializeField]
	private Vector2 mCameraVerticalParameterBounds;
	private float mCameraVerticalParameter = 0.25f;

	private void Start ()
	{
		mStateMachine = GetComponent<PlayerStateMachine>();
	}

	private void Update ()
	{
		// Input
		mCameraHorizontalParameter = mStateMachine.mInputStickR.x;
		mCameraVerticalParameter += mStateMachine.mInputStickR.y;

		// Clamping vertical input within mCameraVerticalParameterBounds
		if (mCameraVerticalParameter < mCameraVerticalParameterBounds.x)
			mCameraVerticalParameter = mCameraVerticalParameterBounds.x;
		else if (mCameraVerticalParameter > mCameraVerticalParameterBounds.y)
			mCameraVerticalParameter = mCameraVerticalParameterBounds.y;
		
		// Updating camera
		mCameraFreeLook.m_XAxis.m_InputAxisValue = mCameraHorizontalParameter;
		mCameraFreeLook.m_YAxis.Value = mCameraVerticalParameter;
	}
}