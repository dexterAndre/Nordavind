using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerMovement2))]
public class PlayerThrow2 : MonoBehaviour
{
    /*
        To do: 
    */

    [Header("Movement")]
    [SerializeField]
    private float mStrafeMultiplier;
    private float mWalkSpeedOriginal;

    [Header("Projectile")]
    [SerializeField]
    private GameObject mProjectile = null;
    [SerializeField]
    private float mProjectileLifetime = 5f;
    [SerializeField]
    private Transform mProjectileParent = null;

    [Header("Cooldown")]
    [SerializeField]
    [Tooltip("Cooldown resets upon releasing the right trigger. You can throw as fast as you can spam the right trigger. ")]
    private bool mCooldown = false;

    [Header("Physics")]
    [SerializeField]
    private float mThrowStrength;
    [SerializeField]
    private float mProjectileGravityScale;

    [Header("References")]
    [SerializeField]
    private Transform mAimedThrowSpawn = null;
    [SerializeField]
    private Transform mFreeThrowSpawn = null;
    [SerializeField]
    private Transform mFreeThrowFocus = null;
    [SerializeField]
    private PlayerMovement2 mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private CinemachineFreeLook mCameraStandard = null;
    [SerializeField]
    private CinemachineFreeLook mCameraAim = null;

    [Header("Debug")]
    [SerializeField]
    private bool mIsDebugging = true;
    [SerializeField]
    private Color mDebugAimedColor = new Color(255f / 255f, 125f / 255f, 0f / 255f, 255f / 255f);
    [SerializeField]
    private Color mDebugFreeColor = new Color(255f / 255f, 55f / 255f, 55f / 255f, 255f / 255f);



    private void Awake ()
	{
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement2>();
        
        // Sets walk speed at awake. Does not respond to real-time changes. 
        mWalkSpeedOriginal = mPlayerMovement.GetWalkSpeed();

        if (mAimedThrowSpawn == null)
            mAimedThrowSpawn = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(0).transform;

        if (mFreeThrowSpawn == null)
            mFreeThrowSpawn = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(1).transform;

        if (mFreeThrowFocus == null)
            mFreeThrowFocus = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(1).transform
                .GetChild(0).transform;

        if (mProjectileParent == null)
            mProjectileParent = GameObject.Find("Projectiles").transform;

        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        if (mCameraStandard == null)
            mCameraStandard = GameObject.Find("Camera Rig Standard").GetComponent<CinemachineFreeLook>();

        if (mCameraAim == null)
            mCameraAim = GameObject.Find("Camera Rig Aim").GetComponent<CinemachineFreeLook>();
    }

	private void Update ()
	{
        // Cooldown
        if (mCooldown)
            if (mInputManager.GetTriggers().y == 0f)
                mCooldown = false;

        // Sending signals
        if (mInputManager.GetTriggers().x != 0f)
        {
            // Walk-to-throw
            if (mPlayerMovement.GetState() == PlayerMovement2.State.Walk)
            {
                mPlayerMovement.SetState(PlayerMovement2.State.Throw);

                // Movement
                mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier);

                // Enabling aim camera, disabling standard camera
                mCameraStandard.Priority = 1;
                mCameraAim.Priority = 10;
                // Prevent standard camera from drifting when changing between cameras
                mCameraStandard.m_XAxis.m_InputAxisValue = 0f;
            }
            
            // Perform aimed throw
            if (!mCooldown)
            {
                if (
                    mPlayerMovement.GetState() == PlayerMovement2.State.Throw 
                    && mInputManager.GetTriggers().y != 0f)
                {
                    Throw(
                        mAimedThrowSpawn.position,
                        (mAimedThrowSpawn.position - Camera.main.transform.position)
                        .normalized
                        * mThrowStrength,
                        true);
                }
            }
        }
        else
        {
            // Throw-to-walk
            if (mPlayerMovement.GetState() == PlayerMovement2.State.Throw)
            {
                mPlayerMovement.SetState(PlayerMovement2.State.Walk);

                // Movement
                mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);

                // Enabling standard camera, disabling aim camera
                mCameraStandard.Priority = 10;
                mCameraAim.Priority = 1;
            }
            
            // Perform free throw
            if (!mCooldown)
            {
                if (
                    (mPlayerMovement.GetState() == PlayerMovement2.State.Walk
                    || mPlayerMovement.GetState() == PlayerMovement2.State.Air)
                    && mInputManager.GetTriggers().y != 0f)
                {
                    Throw(
                        mFreeThrowSpawn.position,
                        (mFreeThrowFocus.position - mFreeThrowSpawn.position)
                        .normalized
                        * mThrowStrength,
                        true);
                }
            }
        }
	}

    private void Throw(Vector3 spawn, Vector3 velocity, bool destroy)
    {
        GameObject snowball = Instantiate(
            mProjectile,
            spawn,
            Quaternion.identity,
            mProjectileParent);

        snowball.GetComponent<Rigidbody>().velocity = velocity;

        if (destroy)
            Destroy(snowball, mProjectileLifetime);

        mCooldown = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (mIsDebugging)
        {
            Gizmos.color = mDebugAimedColor;
            Gizmos.DrawLine(
                Camera.main.transform.position,
                mAimedThrowSpawn.position);
            Gizmos.color = mDebugFreeColor;
            Gizmos.DrawLine(
                mFreeThrowSpawn.position,
                mFreeThrowFocus.position);
        }
    }
}