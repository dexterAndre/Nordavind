using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateMachine), typeof(PlayerMovement))]
public class PlayerRoll : MonoBehaviour
{
	[Header("Roll")]
	[SerializeField]
	private float mRollSpeed = 10f;
	public float GetRollSpeed() { return mRollSpeed; }
	[SerializeField]
	private float mRollCooldown = 2f;
	public float GetRollCooldown() { return mRollCooldown; }
	private float mRollCooldownTimer = 0f;
	public float GetRollCooldownTimer() { return mRollCooldownTimer; }
	public void StartRollCooldownTimer() { mRollCooldownTimer += Time.fixedDeltaTime; }
	[SerializeField]
	private float mRollDuration = 0.05f;
	public float GetRollDuration() { return mRollDuration; }
	private float mRollTimer = 0f;
	public void StartRollTimer() { mRollTimer += Time.fixedDeltaTime; }
	public float GetRollTimer() { return mRollTimer; }
	private Vector3 mRollDirection;
	public Vector3 GetRollDirection() { return mRollDirection; }

	// References
	private PlayerStateMachine mStateMachine;



	private void Start()
	{
		mStateMachine = GetComponent<PlayerStateMachine>();
	}
	
	private void FixedUpdate()
	{
		// Cooldown
		if (mRollCooldownTimer > 0f)
		{
			mRollCooldownTimer += Time.fixedDeltaTime;
			if (mRollCooldownTimer >= mRollCooldown)
			{
				mRollCooldownTimer = 0f;
			}
		}

		// Roll timer
		if (mRollTimer > 0f)
		{
			mRollTimer += Time.fixedDeltaTime;
			if (mRollTimer >= mRollDuration)
			{
				mRollTimer = 0f;
				mStateMachine.SetState(PlayerStateMachine.PlayerState.Walk);
			}
		}
	}
}