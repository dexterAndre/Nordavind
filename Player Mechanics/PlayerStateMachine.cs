using UnityEngine;
using System.Collections;

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

	[HideInInspector]
	public Vector2 mInputStickL;
	[HideInInspector]
	public Vector2 mInputStickR;
	[HideInInspector]
	public float mInputTriggerL;
	[HideInInspector]
	public float mInputTriggerR;
	[HideInInspector]
	public float mInputDPadX;
	[HideInInspector]
	public float mInputDPadY;



	private void Start ()
	{
		mState = PlayerState.Walk;
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
		mInputDPadX = Input.GetAxisRaw("DPadX") * mDPadSensitivityX;
		mInputDPadY = Input.GetAxisRaw("DPadY") * mDPadSensitivityY;
		// Triggers
		mInputTriggerL = Input.GetAxisRaw("TriggerL") * mInputTriggerLSensitivity;
		mInputTriggerR = Input.GetAxisRaw("TriggerR") * mInputTriggerRSensitivity;
	}
}