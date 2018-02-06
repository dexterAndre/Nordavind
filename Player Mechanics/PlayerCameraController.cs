using UnityEngine;
using System.Collections;
using Cinemachine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCameraController : MonoBehaviour
{
    /*
        To-do: 
        - Bug: when transitioning from aim to standard, if you rotate avatar, camera behaves wonkily
        - Store camera parameters so that when transitioning, you don't get abruptly rotated
        - When moving sideways while aiming, the camera lerps to catch up. This makes it difficult to aim until it's in resting position. 
        - Store a map of <Enemy, ProjectedVector> while holding LT. This updates until you release LT, after which it clears itself. 
        - Implement automatic throwing while in lock-on

        Upgrade guide: 
        - Will change state name from "Throw" to "Look"
        - Will change state name from "Lockon" to "Target"
        - - Also change the comments and variable names (also in hierarchy). 
    */
    [Header("Camera Settings")]
    [SerializeField]
    private float mTransitionTime = 0.5f;
    public float GetTransitionTime() { return mTransitionTime; }
    public void SetTransitionTime(float time) { mTransitionTime = time; }

    [Header("Movement")]
    [SerializeField]
    private float mStrafeMultiplier;
    private float mWalkSpeedOriginal;

    [Header("Aim")]
    [SerializeField]
    private SpriteRenderer mReticle = null;
    [SerializeField]
    private Transform mReticlePositionInitial = null;

    [Header("Lock-On Targeting")]
    [SerializeField]
    private Transform mLockonTarget;
    public Transform GetLockonTarget() { return mLockonTarget; }
    [SerializeField]
    [Tooltip("How far away from the player the middle point is located. 0: player, 1: enemy. ")]
    [Range(0.0f, 1.0f)]
    private float mLockonParameter = 0.5f;
    [SerializeField]
    private List<Transform> mLockonList = new List<Transform>();
    [SerializeField]
    private float mLockonReticleHeight = 2f;
    [SerializeField]
    [Range(0f, 50f)]
    private float mLockonDistanceMax = 15f;

    [Header("Bounds")]
    [SerializeField]
    private Vector2 mStandardVerticalBounds;
    [SerializeField]
    private Vector2 mAimVerticalBounds;
    [SerializeField]
    [Tooltip("Multiplier bounds based on the starting distance away from the player. ")]
    private Vector2 mRadiusScaleBounds;
    private float mHorizontalParameter = 0f;
    private float mStandardVerticalParameter = 0.25f;
    private float mAimVerticalParameter = 0.5f;
    private float mRadiusScale = 1f;

    // Remembering variables between modes
    private float mCameraStandardYAxis = 0.5f;

    [Header("References")]
    [SerializeField]
    private CinemachineFreeLook mCameraStandard = null;
    [SerializeField]
    private CinemachineFreeLook mCameraAim = null;
    [SerializeField]
    private CinemachineFreeLook mCameraLockon = null;
    [SerializeField]
    private CinemachineBrain mCameraBrain = null;
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private Transform mAimedThrowSpawn = null;
    [SerializeField]
    private Transform mCameraStandardFocus = null;
    [SerializeField]
    private Transform mCameraLockonLookat = null;

    [Header("Debug")]
    [SerializeField]
    private bool mIsDebugging = true;
    [SerializeField]
    private Color mLockonColor = new Color(125f / 255f, 125f / 255f, 255f / 255f, 255f / 255f);
    [SerializeField]
    private Color mLockonSphereColor = new Color(125f / 255f, 125f / 255f, 255f / 255f, 55f / 255f);
    [SerializeField]
    private Color mCameraColor = new Color(255f / 255f, 125f / 255f, 125f / 255f, 255f / 255f);

    // Default value storage
    private float[,] mCameraStandardRigMeasurements = new float[3, 2];
    private float[,] mCameraAimRigMeasurements = new float[3, 2];
    private float[,] mCameraLockonRigMeasurements = new float[3, 2];



	private void Awake ()
	{
        // Quick setups
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement>();
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        // Sets walk speed at awake. Does not respond to real-time changes. 
        mWalkSpeedOriginal = mPlayerMovement.GetWalkSpeed();

        if (mReticle == null)
            mReticle = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(0).GetComponent<SpriteRenderer>();
        mReticle.enabled = false;
        mReticlePositionInitial = mReticle.transform;

        // Aim target
        if (mAimedThrowSpawn == null)
            mAimedThrowSpawn = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(0).transform;

        // Standard third-person camera
        if (mCameraStandard == null)
            mCameraStandard = GameObject.Find("Camera Rig Standard").GetComponent<CinemachineFreeLook>();
        mCameraStandard.Priority = 10;
        // Setting this as follow target
        if (mCameraStandard.Follow == null)
            mCameraStandard.Follow = transform;
        if (mCameraStandardFocus == null)
            mCameraStandardFocus = transform
                .GetChild(3).transform
                .GetChild(0).transform;
        // Setting "Camera Standard Focus" as look-at target
        if (mCameraStandard.LookAt == null)
            mCameraStandard.LookAt = mCameraStandardFocus;

        // Aim mode camera
        if (mCameraAim == null)
            mCameraAim = GameObject.Find("Camera Rig Aim").GetComponent<CinemachineFreeLook>();
        mCameraAim.Priority = 1;
        // Setting "Aimed Throw Spawn Position" as follow target
        if (mCameraAim.Follow == null)
            mCameraAim.Follow = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(0).transform;
        // Setting "Aimed Throw Spawn Position" as look-at target
        if (mCameraAim.LookAt == null)
            mCameraAim.LookAt = mCameraAim.Follow;

        // Lock-on mode camera
        if (mCameraLockon == null)
            mCameraLockon = GameObject.Find("Camera Rig Lockon").GetComponent<CinemachineFreeLook>();
        mCameraLockon.Priority = 1;
        // Setting this as follow target
        if (mCameraLockon.Follow == null)
            mCameraLockon.Follow = transform;
        // Setting "Camera Lockon Target" as look-at target
        if (mCameraLockon.LookAt == null)
            mCameraLockon.LookAt = transform
                .GetChild(3).transform
                .GetChild(2).transform;

        // Camera brain
        if (mCameraBrain == null)
            mCameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        mCameraBrain.m_DefaultBlend.m_Time = mTransitionTime;

        if (mCameraLockonLookat == null)
            mCameraLockonLookat = transform
                .GetChild(3).transform
                .GetChild(3).transform;

        // Storing startup rig measurements
        mCameraStandardRigMeasurements[0, 0] = mCameraStandard.m_Orbits[0].m_Height;
        mCameraStandardRigMeasurements[0, 1] = mCameraStandard.m_Orbits[0].m_Radius;
        mCameraStandardRigMeasurements[1, 0] = mCameraStandard.m_Orbits[1].m_Height;
        mCameraStandardRigMeasurements[1, 1] = mCameraStandard.m_Orbits[1].m_Radius;
        mCameraStandardRigMeasurements[2, 0] = mCameraStandard.m_Orbits[2].m_Height;
        mCameraStandardRigMeasurements[2, 1] = mCameraStandard.m_Orbits[2].m_Radius;

        mCameraAimRigMeasurements[0, 0] = mCameraAim.m_Orbits[0].m_Height;
        mCameraAimRigMeasurements[0, 1] = mCameraAim.m_Orbits[0].m_Radius;
        mCameraAimRigMeasurements[1, 0] = mCameraAim.m_Orbits[1].m_Height;
        mCameraAimRigMeasurements[1, 1] = mCameraAim.m_Orbits[1].m_Radius;
        mCameraAimRigMeasurements[2, 0] = mCameraAim.m_Orbits[2].m_Height;
        mCameraAimRigMeasurements[2, 1] = mCameraAim.m_Orbits[2].m_Radius;

        mCameraLockonRigMeasurements[0, 0] = mCameraLockon.m_Orbits[0].m_Height;
        mCameraLockonRigMeasurements[0, 1] = mCameraLockon.m_Orbits[0].m_Radius;
        mCameraLockonRigMeasurements[1, 0] = mCameraLockon.m_Orbits[1].m_Height;
        mCameraLockonRigMeasurements[1, 1] = mCameraLockon.m_Orbits[1].m_Radius;
        mCameraLockonRigMeasurements[2, 0] = mCameraLockon.m_Orbits[2].m_Height;
        mCameraLockonRigMeasurements[2, 1] = mCameraLockon.m_Orbits[2].m_Radius;
    }

    // Player Input Handling
    private void Update()
    {
        // Aim mode
        if (Input.GetButtonDown("ClickStickR"))
        {
            // Walk-to-throw
            if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
            {
                // Debug
                if (mIsDebugging)
                {
                    print("BUTTON PRESS: \t RS Click. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Throw);

                // Movement
                transform.forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier);

                // Enabling aim camera, disabling standard camera
                mCameraStandard.Priority = 1;
                mCameraLockon.Priority = 1;
                mCameraAim.Priority = 10;

                // Storing the vertical parameter prior to entering throw mode
                mCameraStandardYAxis = mCameraStandard.m_YAxis.Value;

                // Making sure the standard camera behaves like the aim camera to avoid drifting
                mCameraStandard.m_Follow = mAimedThrowSpawn;
                mCameraStandard.m_LookAt = mAimedThrowSpawn;
                StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.LockToTarget, true));
                mCameraStandard.m_XAxis.m_InputAxisValue = 0f;

                // Enabling reticle
                mReticle.enabled = true;

                // Debug
                if (mIsDebugging)
                {
                    print("MAN TRANSITION: \t WALK \t -> \t THROW. ");
                }
            }
            // Throw-to-walk
            else if (mPlayerMovement.GetState() == PlayerMovement.State.Throw)
            {
                // Debug
                if (mIsDebugging)
                {
                    print("BUTTON PRESS: \t RS Click. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Walk);

                // Movement
                mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

                // Enabling standard camera, disabling aim camera
                mCameraStandard.Priority = 10;
                mCameraLockon.Priority = 1;
                mCameraAim.Priority = 1;

                // Setting the vertical parameter to what is was before aim mode
                mCameraStandard.m_YAxis.Value = mCameraStandardYAxis;
                mCameraStandardYAxis = 0.5f;

                // Resetting standard camera follow target
                mCameraStandard.m_Follow = transform;
                mCameraStandard.m_LookAt = mCameraStandardFocus;
                StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));

                // Enabling reticle
                mReticle.enabled = false;

                // Debug
                if (mIsDebugging)
                {
                    print("MAN TRANSITION: \t THROW \t -> \t WALK. ");
                }
            }
        }
        // Lock-on mode
        else if (mInputManager.GetTriggers().x != 0.0f)
        {
            // Walk-to-lock-on
            if (mPlayerMovement.GetState() == PlayerMovement.State.Walk)
            {
                // Debug
                if (mIsDebugging)
                {
                    print("BUTTON PRESS: \t LT. ");
                }

                // Signaling state machine
                mPlayerMovement.SetState(PlayerMovement.State.Lockon);

                // Clean up the enemy list
                mLockonList.Clear();

                // Collision test
                LayerMask enemyLayer = 1 << 30;
                RaycastHit[] enemies = Physics.SphereCastAll(
                    transform.position,
                    mLockonDistanceMax,
                    transform.forward,
                    0,
                    enemyLayer);
                //RaycastHit[] enemies = Physics.SphereCastAll(
                //    transform.position,
                //    mLockonDistanceMax,
                //    transform.forward,
                //    0f,
                //    ~LayerMask.NameToLayer("Enemy"));
                
                // If enemies are nearby and visible...
                if (enemies.Length > 0)
                {
                    Transform closestEnemy = null;

                    // Handling list of enemies
                    foreach (RaycastHit obj in enemies)
                    {
                        // Later on, do selection here, and delete the entire EnemyList. 
                        // Use it for now to debug. 
                        Renderer rend = obj.transform.gameObject.GetComponent<Renderer>();
                        // If renderer exists
                        if (rend != null)
                        {
                            // If the renderer is visible
                            if (rend.isVisible)
                            {
                                // If the object is actually in front (.isVisible sometimes give false positives)
                                if (Vector3.Dot(Camera.main.transform.forward, obj.transform.position - Camera.main.transform.position) > 0.0f)
                                {
                                    mLockonList.Add(obj.transform);
                                }
                            }
                        }
                    }

                    closestEnemy = SelectClosestToRay(
                        transform.position,
                        Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up),
                        mLockonList);

                    Vector3 enemyPos = closestEnemy.position;
                    mCameraLockonLookat.position = transform.position + mLockonParameter * (enemyPos - transform.position);

                    // Setting transforms
                    mLockonTarget = closestEnemy;
                    transform.forward = Vector3.ProjectOnPlane(mLockonTarget.position - transform.position, Vector3.up);
                    mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier); // Do not actualy set strafe. This is just for testing. 

                    // Target visualization
                    mReticle.enabled = true;
                    mReticle.transform.position = mLockonTarget.position + Vector3.up * mLockonReticleHeight;
                }
                // If no enemy within sight...
                else
                {
                    // Setting transforms
                    mCameraLockonLookat.position = transform.position + mLockonParameter * transform.forward * 5.0f;
                    mLockonTarget = null;
                    mPlayerMovement.SetForwardLockonPersistent(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized);

                    // Target visualization
                    mReticle.enabled = false;
                }

                // Resetting mPlayerMovement's mLockonTarget
                mPlayerMovement.SetLockonTarget(mLockonTarget);

                // Enabling aim camera, disabling standard camera
                mCameraStandard.Priority = 1;
                mCameraLockon.Priority = 10;
                mCameraAim.Priority = 1;

                // Setting standard camera to follow aim camera
                mCameraStandard.m_Follow = mCameraLockon.transform;
                StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.LockToTarget, true));

                // Prevent standard camera from drifting when changing between cameras
                mCameraStandard.m_XAxis.m_InputAxisValue = 0f;

                // Debug
                if (mIsDebugging)
                {
                    print("MAN TRANSITION: \t WALK \t -> \t LOCKON. ");
                }
            }
        }
        // Lock-on-to-walk
        else if (mPlayerMovement.GetState() == PlayerMovement.State.Lockon)
        {
            if (mInputManager.GetTriggers().x == 0.0f)
            {
                // Debug
                if (mIsDebugging)
                {
                    print("BUTTON RELEASE: \t LT. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Walk);

                // Movement
                mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

                // Enabling standard camera, disabling aim camera
                mCameraStandard.Priority = 10;
                mCameraLockon.Priority = 1;
                mCameraAim.Priority = 1;

                // Resetting standard camera follow target
                mCameraStandard.m_Follow = transform;
                StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));

                // Resetting lock-on persistent forward direction
                mPlayerMovement.SetForwardLockonPersistent(Vector3.zero);

                // Resetting mPlayerMovement's mLockonTarget
                mPlayerMovement.SetLockonTarget(null);

                // Enabling reticle
                mReticle.enabled = false;
                mReticle.transform.position = mReticlePositionInitial.position;

                // Setting the vertical parameter to what is was before aim mode
                mCameraStandard.m_YAxis.Value = mCameraStandardYAxis;
                mCameraStandardYAxis = 0.5f;

                // Resetting standard camera follow target
                mCameraStandard.m_Follow = transform;
                mCameraStandard.m_LookAt = mCameraStandardFocus;
                StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));

                // Debug
                if (mIsDebugging)
                {
                    print("MAN TRANSITION: \t LOCKON \t -> \t WALK. ");
                }
            }
        }
    }

    // Camera behavior
    private void LateUpdate ()
	{
        // Standard mode
        if (mPlayerMovement.GetState() != PlayerMovement.State.Throw
            && mPlayerMovement.GetState() != PlayerMovement.State.Lockon)
        {
            // Storing input
            mHorizontalParameter
                = mInputManager.GetStickRight().x
                * mInputManager.GetCameraStandardSensitivity().x
                * Time.deltaTime;
            mStandardVerticalParameter
                += mInputManager.GetStickRight().y
                * mInputManager.GetCameraStandardSensitivity().y
                * Time.deltaTime;
            mRadiusScale 
                += mInputManager.GetDPad().y 
                * mInputManager.GetCameraStandardDollySensitivity() 
                * Time.deltaTime;

            // Clamping vertical input within mStandardVerticalBounds
            if (mStandardVerticalParameter < mStandardVerticalBounds.x)
                mStandardVerticalParameter = mStandardVerticalBounds.x;
            else if (mStandardVerticalParameter > mStandardVerticalBounds.y)
                mStandardVerticalParameter = mStandardVerticalBounds.y;

            // Clamping dolly between mDistanceBounds
            if (mRadiusScale < mRadiusScaleBounds.x)
                mRadiusScale = mRadiusScaleBounds.x;
            else if (mRadiusScale > mRadiusScaleBounds.y)
                mRadiusScale = mRadiusScaleBounds.y;

            // Updating standard camera
            mCameraStandard.m_XAxis.m_InputAxisValue = mHorizontalParameter;
            mCameraStandard.m_YAxis.Value = mStandardVerticalParameter;
            for (int i = 0; i < 3; i++)
            {
                mCameraStandard.m_Orbits[i].m_Radius 
                    = mCameraStandardRigMeasurements[i, 1] 
                    * mRadiusScale;
            }
        }
        // Lock-on mode
        else if (mPlayerMovement.GetState() == PlayerMovement.State.Lockon)
        {
            // Storing input
            mRadiusScale
                += mInputManager.GetDPad().y
                * mInputManager.GetCameraLockonDollySensitivity().y
                * Time.deltaTime;

            // Clamping dolly between mDistanceBounds
            if (mRadiusScale < mRadiusScaleBounds.x)
                mRadiusScale = mRadiusScaleBounds.x;
            else if (mRadiusScale > mRadiusScaleBounds.y)
                mRadiusScale = mRadiusScaleBounds.y;

            // Updating lock-on camera
            for (int i = 0; i < 3; i++)
            {
                mCameraLockon.m_Orbits[i].m_Radius
                    = mCameraLockonRigMeasurements[i, 1]
                    * mRadiusScale;
            }

            // Debug
            VisualDebug();
        }
        // Aim mode
        else if (mPlayerMovement.GetState() == PlayerMovement.State.Throw)
        {
            // Storing input
            mAimVerticalParameter
                += mInputManager.GetStickRight().y
                * mInputManager.GetCameraAimSensitivity().y
                * Time.deltaTime;
            mRadiusScale
                += mInputManager.GetDPad().y
                * mInputManager.GetCameraAimDollySensitivity()
                * Time.deltaTime;

            // Clamping vertical input within mStandardVerticalBounds
            if (mAimVerticalParameter < mAimVerticalBounds.x)
                mAimVerticalParameter = mAimVerticalBounds.x;
            else if (mAimVerticalParameter > mAimVerticalBounds.y)
                mAimVerticalParameter = mAimVerticalBounds.y;

            // Clamping dolly between mDistanceBounds
            if (mRadiusScale < mRadiusScaleBounds.x)
                mRadiusScale = mRadiusScaleBounds.x;
            else if (mRadiusScale > mRadiusScaleBounds.y)
                mRadiusScale = mRadiusScaleBounds.y;

            // Updating standard camera
            mCameraAim.m_YAxis.Value = mAimVerticalParameter;
            for (int i = 0; i < 3; i++)
            {
                mCameraAim.m_Orbits[i].m_Radius 
                    = mCameraAimRigMeasurements[i, 1] 
                    * mRadiusScale;
            }
        }
	}

    private IEnumerator SetBindingMode(CinemachineTransposer.BindingMode mode, bool delay)
    {
        if (delay)
        {
            WaitForSeconds wait = new WaitForSeconds(GetTransitionTime());
            yield return wait;
        }

        mCameraStandard.m_BindingMode = mode;
    }

    private void OnDrawGizmosSelected()
    {
        if (mIsDebugging)
        {
            Gizmos.color = mCameraColor;
            Gizmos.DrawLine(
                Camera.main.transform.position,
                Camera.main.transform.position + Camera.main.transform.forward);

            Gizmos.color = mLockonColor;
            if (mCameraLockon.LookAt != null)
                Gizmos.DrawLine(
                    mCameraLockon.transform.position,
                    mCameraLockon.LookAt.position);
            if (mCameraLockon.Follow != null)
                Gizmos.DrawLine(
                    mCameraLockon.transform.position,
                    mCameraLockon.Follow.position);
            if (mLockonTarget != null)
                Gizmos.DrawLine(
                    mCameraLockon.transform.position,
                    mLockonTarget.transform.position);

            Gizmos.color = mLockonSphereColor;
            Gizmos.DrawSphere(transform.position, mLockonDistanceMax);
        }
    }

    private Transform SelectClosestToRay(Vector3 pos, Vector3 forward, List<Transform> objects)
    {
        int closestIndex = 0;
        float closestDistance = 0.0f;
        Vector3 PO;
        Vector3 projected;
        float projectedMagnitude;

        for (int i = 0; i < objects.Count; i++)
        {
            // Player-to-object
            PO = objects[i].position - pos;
            // PO rejected onto foward vector
            projected = PO - (Vector3.Dot(PO, forward) / (forward.magnitude * forward.magnitude)) * forward;
            projectedMagnitude = projected.magnitude;

            if (projectedMagnitude < closestDistance)
            {
                closestDistance = projectedMagnitude;
                closestIndex = i;
            }
        }

        return objects[closestIndex];
    }

    private void VisualDebug()
    {
        if (mIsDebugging)
        {
            // Lock-on target selection
            if (mInputManager.GetTriggers().x < 0.0f)
            {
                Vector3 fwd = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                Debug.DrawLine(transform.position, transform.position + fwd * mLockonDistanceMax, mLockonColor);
                Vector3 PO;
                Vector3 projectedVector;

                if (mLockonList.Count > 0)
                {
                    foreach (Transform obj in mLockonList)
                    {
                        PO = obj.position - transform.position;
                        projectedVector = PO - ((Vector3.Dot(PO, fwd)) / (fwd.magnitude * fwd.magnitude)) * fwd;
                        Debug.DrawLine(obj.position, obj.position - projectedVector, mLockonColor);
                    }
                }
            }
        }
    }
}