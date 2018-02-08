using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerCameraController))]
public class PlayerThrow : MonoBehaviour
{
    /*
        To do: 
        - Get lock-on to work! 
        - Move camera-related stuff to PlayerCameraController
        - Actually throw along a parabola, instead of just straight
    */

    //[Header("Movement")]
    //[SerializeField]
    //private float mStrafeMultiplier;
    //private float mWalkSpeedOriginal;

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

    //[Header("Lock-On Targeting")]
    //[SerializeField]
    //private Vector3 mLockonTarget;
    //[SerializeField]
    //private Transform mLockonLookat;
    //[SerializeField]
    //private float mLockonReticleHeight = 2f;
    //[SerializeField]
    //[Range(0f, 50f)]
    //private float mLockonDistanceMax = 15f;

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
    //[SerializeField]
    //private Transform mLockonFocus = null;
    //[SerializeField]
    //private SpriteRenderer mReticle = null;
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;
    //[SerializeField]
    //private CharacterController mCharacterController = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private PlayerCameraController mCameraController = null;
    //[SerializeField]
    //private CinemachineFreeLook mCameraStandard = null;
    //[SerializeField]
    //private CinemachineFreeLook mCameraAim = null;
    //[SerializeField]
    //private CinemachineFreeLook mCameraLockon = null;

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
        //if (mCharacterController == null)
        //    mCharacterController = GetComponent<CharacterController>();
        if (mCameraController == null)
            mCameraController = GetComponent<PlayerCameraController>();

        //// Sets walk speed at awake. Does not respond to real-time changes. 
        //mWalkSpeedOriginal = mPlayerMovement.GetWalkSpeed();

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

        //if (mLockonFocus == null)
        //{
        //    mLockonFocus = transform
        //        .GetChild(3).transform
        //        .GetChild(2).transform;
        //}

        //if (mReticle == null)
        //    mReticle = mAimedThrowSpawn.GetComponent<SpriteRenderer>();
        //mReticle.enabled = false;

        if (mProjectileParent == null)
            mProjectileParent = GameObject.Find("Projectiles").transform;

        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        //if (mCameraStandard == null)
        //    mCameraStandard = GameObject.Find("Camera Rig Standard").GetComponent<CinemachineFreeLook>();

        //if (mCameraAim == null)
        //    mCameraAim = GameObject.Find("Camera Rig Aim").GetComponent<CinemachineFreeLook>();

        //if (mCameraLockon == null)
        //    mCameraLockon = GameObject.Find("Camera Rig Lockon").GetComponent<CinemachineFreeLook>();

        //if (mReticlePositionInitial == null)
        //    mReticlePositionInitial = mReticle.gameObject.transform;

        //// Setting lock-on max targeting distance
        //GetComponent<SphereCollider>().radius = mLockonDistanceMax;
    }

	private void Update ()
	{
        // Per-trigger cooldown
        if (mCooldown)
            if (mInputManager.GetTriggers().y == 0f)
                mCooldown = false;

        // Global cooldown
        if (mThrowCooldownTimer > 0f)
        {
            mThrowCooldownTimer += Time.deltaTime;
            if (mThrowCooldownTimer >= mThrowCooldown)
            {
                mThrowCooldownTimer = 0f;
            }
        }

        //// Sending signals
        //if (Input.GetButtonDown("ClickStickR"))
        //{
        //    // Walk-to-throw
        //    if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
        //    {
        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("BUTTON PRESS: \t RS Click. ");
        //        }

        //        mPlayerMovement.SetState(PlayerMovement.State.Throw);

        //        // Movement
        //        transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        //        mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier);

        //        // Enabling aim camera, disabling standard camera
        //        mCameraStandard.Priority = 1;
        //        mCameraAim.Priority = 10;

        //        // Setting standard camera to follow aim camera
        //        mCameraStandard.m_Follow = mCameraAim.transform;
        //        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.LockToTarget, true));

        //        // Enabling reticle
        //        mReticle.enabled = true;

        //        // Prevent standard camera from drifting when changing between cameras
        //        mCameraStandard.m_XAxis.m_InputAxisValue = 0f;

        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("MAN TRANSITION: \t WALK \t -> \t THROW. ");
        //        }
        //    }
        //    // Throw-to-walk
        //    else if (mPlayerMovement.GetState() == PlayerMovement.State.Throw)
        //    {
        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("BUTTON PRESS: \t RS Click. ");
        //        }

        //        mPlayerMovement.SetState(PlayerMovement.State.Walk);

        //        // Movement
        //        mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

        //        // Enabling standard camera, disabling aim camera
        //        mCameraStandard.Priority = 10;
        //        mCameraAim.Priority = 1;

        //        // Resetting standard camera follow target
        //        mCameraStandard.m_Follow = transform;
        //        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));

        //        // Enabling reticle
        //        mReticle.enabled = false;

        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("MAN TRANSITION: \t THROW \t -> \t WALK. ");
        //        }
        //    }
        //}
        //else if (mInputManager.GetTriggers().x != 0.0f)
        //{
        //    // Walk-to-lock-on
        //    if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
        //    {
        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("BUTTON PRESS: \t LT. ");
        //        }

        //        mPlayerMovement.SetState(PlayerMovement.State.Lockon);
        //        RaycastHit[] enemies = Physics.SphereCastAll(
        //            transform.position,
        //            mLockonDistanceMax,
        //            transform.forward,
        //            LayerMask.NameToLayer("Enemy"));
        //        mLockonTarget = GameObject.FindGameObjectsWithTag("Enemy")[0].transform.position - transform.position;
        //        mLockonFocus = GameObject.FindGameObjectsWithTag("Enemy")[0].transform;
        //        mLockonLookat = GameObject.FindGameObjectsWithTag("Enemy")[0].transform;

        //        // Movement
        //        transform.forward = Vector3.ProjectOnPlane(mLockonTarget - transform.position, Vector3.up);
        //        mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier);

        //        // Enabling aim camera, disabling standard camera
        //        mCameraStandard.Priority = 1;
        //        mCameraLockon.Priority = 10;

        //        // Setting standard camera to follow aim camera
        //        mCameraStandard.m_Follow = mCameraLockon.transform;
        //        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.LockToTarget, true));

        //        // Enabling reticle
        //        mReticle.enabled = true;
        //        mReticle.transform.position = mLockonTarget + Vector3.up * mLockonReticleHeight;

        //        // Prevent standard camera from drifting when changing between cameras
        //        mCameraStandard.m_XAxis.m_InputAxisValue = 0f;

        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("MAN TRANSITION: \t WALK \t -> \t LOCKON. ");
        //        }
        //    }
        //    // Throw-to-walk
        //    else if (mPlayerMovement.GetState() == PlayerMovement.State.Lockon)
        //    {
        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("BUTTON RELEASE: \t LT. ");
        //        }

        //        mPlayerMovement.SetState(PlayerMovement.State.Walk);

        //        // Movement
        //        mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

        //        // Enabling standard camera, disabling aim camera
        //        mCameraStandard.Priority = 10;
        //        mCameraLockon.Priority = 1;

        //        // Resetting standard camera follow target
        //        mCameraStandard.m_Follow = transform;
        //        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));

        //        // Enabling reticle
        //        mReticle.enabled = false;
        //        mReticle.transform.position = mReticlePositionInitial.position;

        //        // Debug
        //        if (mIsDebuggingThrow)
        //        {
        //            print("MAN TRANSITION: \t LOCKON \t -> \t WALK. ");
        //        }
        //    }
        //}

        // Throw
        if (!mCooldown)
        {
            // Aimed throw
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
                    true, 
                    false);

                // Cooldown start
                mThrowCooldownTimer += Time.deltaTime;
            }
            // Free throw
            else if (
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
                    true, 
                    false);

                // Cooldown start
                mThrowCooldownTimer += Time.deltaTime;
            }
            // Lock-on throw
            else if (
                mPlayerMovement.GetState() == PlayerMovement.State.Lockon
                && mInputManager.GetTriggers().y != 0f
                && mThrowCooldownTimer == 0f)
            {
                // Model a 2nd degree polynomial, and use its derivative to 
                // set the velocity. Use this velocity as parameter in Throw(). 
                Throw(
                    mFreeThrowSpawn.position,
                    (mCameraController.GetLockonTarget().position - mFreeThrowSpawn.position).normalized * mThrowStrength,
                    true,
                    true);
            }
        }
	}

    private void Throw(Vector3 spawn, Vector3 velocity, bool destroy, bool disableGravity)
    {
        GameObject snowball = Instantiate(
            mProjectile,
            spawn,
            Quaternion.identity,
            mProjectileParent);

        snowball.GetComponent<Rigidbody>().velocity = velocity;

        if (destroy)
            Destroy(snowball, mProjectileLifetime);
        if (disableGravity)
            snowball.GetComponent<Rigidbody>().useGravity = false;

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

    //private IEnumerator SetBindingMode(CinemachineTransposer.BindingMode mode, bool delay)
    //{
    //    if (delay)
    //    {
    //        WaitForSeconds wait = new WaitForSeconds(mCameraController.GetTransitionTime());
    //        yield return wait;
    //    }

    //    mCameraStandard.m_BindingMode = mode;
    //}
}