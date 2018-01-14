using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateMachine), typeof(PlayerMovement))]
public class PlayerJump : MonoBehaviour
{
	[Header("Jumping")]
	[SerializeField]
	private float mJumpStrength = 10f;

	// References
	private PlayerMovement mPlayerMovement = null;

	private void Start ()
	{
		mPlayerMovement = GetComponent<PlayerMovement>();
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Jump"))
		{
			mPlayerMovement.mVerticalMovement = mJumpStrength;
			mPlayerMovement.mAirTimer += Time.deltaTime;
			print("Jump!");
		}
	}
}