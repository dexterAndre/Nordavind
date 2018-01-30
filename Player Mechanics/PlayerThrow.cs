using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerThrow : MonoBehaviour
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
    private float mThrowCooldown = 1f;
    private float mThrowCooldownTimer = 0f;
    [Tooltip("One throw per RT button-down. ")]
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
    private SpriteRenderer mReticle = null;
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private CinemachineFreeLook mCameraStandard = null;
    [SerializeField]
    private CinemachineFreeLook mCameraAim = null;

    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingThrow = true;
    [SerializeField]
    private Color mDebugAimedColor = new Color(255f / 255f, 125f / 255f, 0f / 255f, 255f / 255f);
    [SerializeField]
    private Color mDebugFreeColor = new Color(255f / 255f, 55f / 255f, 55f / 255f, 255f / 255f);



    private void Awake ()
	{
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement>();
        
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

        if (mReticle == null)
            mReticle = mAimedThrowSpawn.GetComponent<SpriteRenderer>();
        mReticle.enabled = false;

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
        // Per-trigger cooldown
        if (mCooldown)
            mCooldown = false;
        //if (mInputManager.GetTriggers().y == 0f)


        // Global cooldown
        if (mThrowCooldownTimer > 0f)
        {
            mThrowCooldownTimer += Time.deltaTime;
            if (mThrowCooldownTimer >= mThrowCooldown)
            {
                mThrowCooldownTimer = 0f;
            }
        }

        // Sending signals
        if (mInputManager.GetTriggers().x != 0f)
        {
            // Walk-to-throw
            if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
            {
                // Debug
                if (mIsDebuggingThrow)
                {
                    print("BUTTON PRESS: \t LT. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Throw);

                // Movement
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier);

                // Enabling aim camera, disabling standard camera
                mCameraStandard.Priority = 1;
                mCameraAim.Priority = 10;

                // Enabling reticle
                mReticle.enabled = true;

                // Prevent standard camera from drifting when changing between cameras
                mCameraStandard.m_XAxis.m_InputAxisValue = 0f;

                // Debug
                if (mIsDebuggingThrow)
                {
                    print("MAN TRANSITION: \t WALK \t -> \t THROW. ");
                }
            }
            
            // Perform aimed throw
            if (!mCooldown)
            {
                if (
                    mPlayerMovement.GetState() == PlayerMovement.State.Throw 
                    && mInputManager.GetTriggers().y != 0f
                    && mThrowCooldownTimer == 0f)
                {
                    // Debug
                    if (mIsDebuggingThrow)
                    {
                        print("BUTTON PRESS: \t RT. ");
                    }

                    Throw(
                        mAimedThrowSpawn.position,
                        (mAimedThrowSpawn.position - Camera.main.transform.position)
                        .normalized
                        * mThrowStrength,
                        true);

                    // Cooldown start
                    mThrowCooldownTimer += Time.deltaTime;
                }
            }
        }
        else
        {
            // Throw-to-walk
            if (mPlayerMovement.GetState() == PlayerMovement.State.Throw)
            {
                // Debug
                if (mIsDebuggingThrow)
                {
                    print("BUTTON RELEASE: \t LT. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Walk);

                // Movement
                mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

                // Enabling standard camera, disabling aim camera
                mCameraStandard.Priority = 10;
                mCameraAim.Priority = 1;

                // Enabling reticle
                mReticle.enabled = false;

                // Debug
                if (mIsDebuggingThrow)
                {
                    print("MAN TRANSITION: \t THROW \t -> \t WALK. ");
                }
            }
            
            // Perform free throw
            if (!mCooldown)
            {
                if (
                    (mPlayerMovement.GetState() == PlayerMovement.State.Walk
                    || mPlayerMovement.GetState() == PlayerMovement.State.Air)
                    && mInputManager.GetTriggers().y != 0f
                    && mThrowCooldownTimer == 0f)
                {
                    Throw(
                        mFreeThrowSpawn.position,
                        (mFreeThrowFocus.position - mFreeThrowSpawn.position)
                        .normalized
                        * mThrowStrength,
                        true);

                    // Cooldown start
                    mThrowCooldownTimer += Time.deltaTime;
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

        //snowball.GetComponent<Rigidbody>().velocity = velocity;
        snowball.GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, velocity.y, velocity.z);

        if (destroy)
            Destroy(snowball, mProjectileLifetime);

        mCooldown = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (mIsDebuggingThrow)
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