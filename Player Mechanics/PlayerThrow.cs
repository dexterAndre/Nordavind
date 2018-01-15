using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerStateMachine), typeof(PlayerMovement), typeof(PlayerCameraController))]
public class PlayerThrow : MonoBehaviour
{
	[Header("Throw Settings")]
	[SerializeField]
	private float mThrowStrength;
	[SerializeField]
	private float mThrowLifetime;
	private Vector3 mThrowVector;

	[Header("References")]
	[SerializeField]
	private GameObject mProjectile = null;
	[SerializeField]
	private Transform mProjectileParent = null;
	private PlayerStateMachine mStateMachine = null;
	[SerializeField]
	private PlayerCameraController mCameraController = null;
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;

	private void Start()
	{
		mStateMachine = GetComponent<PlayerStateMachine>();
		mCameraController = GetComponent<PlayerCameraController>();
        mPlayerMovement = GetComponent<PlayerMovement>();
	}

	private void Update ()
	{
		// Aimed throw
		if (
			mStateMachine.GetState() == PlayerStateMachine.PlayerState.Throw
			&& Input.GetButtonDown("Fire3"))
		{
			mThrowVector = (mCameraController.GetCameraThrowLookAt().position - mCameraController.GetCameraThrowPosition().position).normalized * mThrowStrength;
			Throw(
				mCameraController.GetCameraThrowPosition().position,
				mThrowVector,
				true);
		}
		// Free throw
		else if (
			mStateMachine.GetState() == PlayerStateMachine.PlayerState.Walk
			&& Input.GetButtonDown("Fire3"))
		{
			mThrowVector = (mCameraController.GetCameraFreeThrowLookAt().position - mCameraController.GetCameraFreeThrowPosition().position).normalized * mThrowStrength;
			Throw(
				mCameraController.GetCameraFreeThrowPosition().position,
				mThrowVector,
				true);
		}
	}

	private void Throw(Vector3 position, Vector3 velocity, bool destroy)
	{
		GameObject snowball = (GameObject)Instantiate(
			mProjectile,
			position,
			Quaternion.identity,
			mProjectileParent);
		snowball.GetComponent<Rigidbody>().velocity = velocity;
		if (destroy)
			Destroy(snowball, mThrowLifetime);
	}

	private void OnDrawGizmosSelected()
	{
        if (mPlayerMovement.GetIsDebugging())
        {
		    Gizmos.color = new Color(255, 125, 0, 1);
		    Gizmos.DrawLine(
			    mCameraController.GetCameraThrowPosition().position,
			    mCameraController.GetCameraThrowLookAt().position);
		    Gizmos.DrawLine(
			    mCameraController.GetCameraFreeThrowPosition().position,
			    mCameraController.GetCameraFreeThrowLookAt().position);
        }
	}
}