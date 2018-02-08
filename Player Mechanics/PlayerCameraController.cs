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
        - Make separate state machine for camera
        - Obscure objects that are between player and camera (shader)

        Upgrade guide: 
        - Will change state name from "Throw" to "Look"
        - Will change state name from "Lockon" to "Target"
        - - Also change the comments and variable names (also in hierarchy). 
        - Add sphere collider to player's chile "Player Camera"
    */
    [System.Serializable]
    public enum State
    {
        Standard, 
        Look,
        Target
    };
    [Header("State")]
    [SerializeField]
    private State mState = State.Standard;
    public State GetState() { return mState; }
    public void SetState(State state) { mState = state; }

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
    public List<Transform> mLockonList = new List<Transform>();
    [SerializeField]
    public Dictionary<Transform, Vector3> mLockonDictionary = new Dictionary<Transform, Vector3>();
    [SerializeField]
    private float mLockonReticleHeight = 2f;
    [SerializeField]
    private float mLockonReticleScale = 5f;
    [SerializeField]
    [Range(0f, 50f)]
    private float mLockonDistanceMax = 15f;
    [SerializeField]
    private float mLockonTargetsUpdateCycle = 0.1f;
    private float mLockonTargetUpdateTimer = 0f;
    [SerializeField]
    private float mLockonTargetSwitchCooldown = 0.5f;
    private float mLockonTargetSwitchTimer = 0f;

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

        // Aim reticle
        if (mReticle == null)
            mReticle = transform
                .GetChild(3).transform
                .GetChild(1).transform
                .GetChild(0).GetComponent<SpriteRenderer>();
        mReticlePositionInitial = mReticle.transform;
        mReticle.enabled = false;

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
        // Cooldowns
        if (mLockonTargetSwitchTimer > 0.0f)
        {
            mLockonTargetSwitchTimer += Time.deltaTime;
            if (mLockonTargetSwitchTimer >= mLockonTargetSwitchCooldown)
            {
                mLockonTargetSwitchTimer = 0f;
            }
        }

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

                // Retrieving list of nearby enemies (enemy layer is at 30)
                FindCloseObjects(transform.position, mLockonDistanceMax, 1 << 30, true);
                
                // If enemies are nearby and visible...
                if (/*mLockonList.Count > 0*/ mLockonDictionary.Count > 0)
                {
                    Transform closestEnemy = SelectClosestToRay(
                        Camera.main.transform.position,
                        Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up),
                        /*mLockonList*/ mLockonDictionary);

                    Vector3 enemyPos = closestEnemy.position;
                    mCameraLockonLookat.position = transform.position + mLockonParameter * (enemyPos - transform.position);

                    // Setting transforms
                    mLockonTarget = closestEnemy;
                    transform.forward = Vector3.ProjectOnPlane(mLockonTarget.position - transform.position, Vector3.up);
                    mPlayerMovement.SetWalkSpeed(mPlayerMovement.GetWalkSpeed() * mStrafeMultiplier); // Do not actualy set strafe. This is just for testing. 

                    // Target visualization
                    mReticle.enabled = true;
                    mReticle.transform.position = mLockonTarget.position + Vector3.up * mLockonReticleHeight;
                    mReticle.transform.localScale = new Vector3(mLockonReticleScale, mLockonReticleScale, mLockonReticleScale);
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
        // From lock-on
        if (mPlayerMovement.GetState() == PlayerMovement.State.Lockon)
        {
            // Lock-on-to-walk
            if (mInputManager.GetTriggers().x == 0.0f)
            {
                // Debug
                if (mIsDebugging)
                {
                    print("BUTTON RELEASE: \t LT. ");
                }

                mPlayerMovement.SetState(PlayerMovement.State.Walk);

                LockonToWalk();

                // Debug
                if (mIsDebugging)
                {
                    print("MAN TRANSITION: \t LOCKON \t -> \t WALK. ");
                }
            }
            
            // Switch targets
            if (mInputManager.GetStickRight().x != 0.0f && mLockonDictionary.Count > 0 && mLockonTargetSwitchTimer == 0f)
            {
                int i = 0;
                float closestDistance = 0.0f;
                Transform closestEnemy = mLockonTarget;

                foreach (KeyValuePair<Transform, Vector3> obj in mLockonDictionary)
                {
                    // Cheap way of testing if the object is in the direction you're pointing the right stick
                    if (obj.Key == mLockonTarget)
                    {
                        closestDistance = obj.Value.sqrMagnitude;
                        closestEnemy = obj.Key;
                    }

                    float dot = Vector3.Dot(Camera.main.transform.right * mInputManager.GetStickRight().x, obj.Value);
                    if (dot < 0.0f)
                    {
                        if (obj.Value.sqrMagnitude < closestDistance)
                        {
                            closestDistance = obj.Value.sqrMagnitude;
                            closestEnemy = obj.Key;
                        }
                    }
                    i++;
                }

                // Change target
                if (closestEnemy != null)
                {
                    Vector3 enemyPos = closestEnemy.position;
                    //mCameraLockonLookat.position = transform.position + mLockonParameter * (enemyPos - transform.position);
                    mLockonTarget = closestEnemy;
                    mPlayerMovement.SetLockonTarget(mLockonTarget);
                    //mCameraLockonLookat.position = mLockonTarget.position;
                    //transform.forward = Vector3.ProjectOnPlane(mLockonTarget.position - transform.position, Vector3.up);
                }
                mLockonTargetSwitchTimer += Time.deltaTime;
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

            // Updating targets
            UpdateTargetList();

            // Position camera look-at position
            mCameraLockonLookat.position = transform.position + mLockonParameter * (mLockonTarget.position - transform.position);
            mCameraLockon.LookAt = mCameraLockonLookat;
            mReticle.transform.position = mLockonTarget.position + Vector3.up * mLockonReticleHeight;

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

    private Transform SelectClosestToRay(Vector3 pos, Vector3 forward, /*List<Transform> objects*/ Dictionary<Transform, Vector3> objects)
    {
        Transform closestTransform = null;
        float closestDistance = 0.0f;
        Vector3 PO;
        Vector3 projected;
        float projectedMagnitude;

        int i = 0;
        foreach (KeyValuePair<Transform, Vector3> obj in objects)
        {
            // Player-to-object
            PO = obj.Key.position - pos;

            // PO rejected onto forward vector
            projected = PO - (Vector3.Dot(PO, forward) / (forward.sqrMagnitude)) * forward;
            projectedMagnitude = projected.sqrMagnitude;
            if (i == 0)
            {
                closestDistance = projectedMagnitude;
                closestTransform = obj.Key;
            }

            if (projectedMagnitude < closestDistance)
            {
                closestDistance = projectedMagnitude;
                closestTransform = obj.Key;
            }
            i++;
        }
        return closestTransform;

        //for (int i = 0; i < objects.Count; i++)
        //{
        //    // Player-to-object
        //    PO = objects[].position - pos;
        //    // PO rejected onto foward vector
        //    projected = PO - (Vector3.Dot(PO, forward) / (forward.sqrMagnitude)) * forward;
        //    projectedMagnitude = projected.sqrMagnitude;

        //    if (projectedMagnitude < closestDistance)
        //    {
        //        closestDistance = projectedMagnitude;
        //        closestIndex = i;
        //    }
        //}

        //return objects[closestIndex];
    }

    private void FindCloseObjects(Vector3 pos, float distance, int layerMask, bool frustumCull)
    {
        // Collision test
        RaycastHit[] enemies = Physics.SphereCastAll(
            pos,
            distance,
            transform.forward,
            0,
            layerMask);

        // If enemies are nearby and visible...
        if (enemies.Length > 0)
        {
            // Handling list of enemies
            foreach (RaycastHit obj in enemies)
            {
                // Used for storing the rejected vectors in dictionary
                Vector3 fwd = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);

                if (frustumCull)
                {
                                    
                    // If renderer exists
                    Renderer rend = obj.transform.gameObject.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        // If the renderer is visible
                        if (rend.isVisible)
                        {
                            // If the object is actually in front (.isVisible sometimes give false positives)
                            if (Vector3.Dot(Camera.main.transform.forward, obj.transform.position - transform.position) > 0.0f)
                            {
                                // Assigns the transform along with its rejected vector
                                Vector3 PO;
                                Vector3 projected;

                                // Player-to-object
                                PO = obj.transform.position - pos;
                                // PO rejected onto foward vector
                                projected = -(PO - (Vector3.Dot(PO, fwd) / (fwd.sqrMagnitude)) * fwd);

                                //mLockonList.Add(obj.transform);
                                mLockonDictionary.Add(obj.transform, projected);
                            }
                        }
                    }
                }
                else
                {
                    // Assigns the transform along with its rejected vector
                    Vector3 PO;
                    Vector3 projected;

                    // Player-to-object
                    PO = obj.transform.position - pos;
                    // PO rejected onto foward vector
                    projected = PO - (Vector3.Dot(PO, fwd) / (fwd.sqrMagnitude)) * fwd;

                    //mLockonList.Add(obj.transform);
                    mLockonDictionary.Add(obj.transform, projected);
                }
            }
        }
    }

    private void UpdateTargetList()
    {
        // Handling timers
        mLockonTargetUpdateTimer += Time.deltaTime;
        if (mLockonTargetUpdateTimer >= mLockonTargetsUpdateCycle)
        {
            // Clearing list before adding new elements
            //mLockonList.Clear();
            mLockonDictionary.Clear();

            FindCloseObjects(transform.position, mLockonDistanceMax, 1 << 30, true);
            mLockonTargetUpdateTimer = 0f;
        }
    }

    private void VisualDebug()
    {
        if (mIsDebugging)
        {
            // Lock-on target selection
            if (mInputManager.GetTriggers().x < 0.0f)
            {
                //Vector3 fwd = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                //Debug.DrawLine(transform.position, transform.position + fwd * mLockonDistanceMax, mLockonColor);
                //Vector3 PO;
                //Vector3 projectedVector;

                //if (/*mLockonList.Count > 0*/mLockonDictionary.Count > 0)
                //{
                //    //foreach (Transform obj in mLockonList)
                //    //{
                //    //    PO = obj.position - transform.position;
                //    //    projectedVector = PO - ((Vector3.Dot(PO, fwd)) / (fwd.sqrMagnitude)) * fwd;
                //    //    Debug.DrawLine(obj.position, obj.position - projectedVector, mLockonColor);
                //    //}
                //    foreach (KeyValuePair<Transform, Vector3> obj in mLockonDictionary)
                //    {
                //        PO = obj.Key.position - transform.position;
                //        projectedVector = PO - ((Vector3.Dot(PO, fwd)) / (fwd.sqrMagnitude)) * fwd;
                //        Debug.DrawLine(obj.Key.position, obj.Key.position - projectedVector, mLockonColor);
                //    }
                //}

                if (mLockonDictionary.Count > 0)
                {
                    foreach (KeyValuePair<Transform, Vector3> obj in mLockonDictionary)
                    {
                        Debug.DrawLine(obj.Key.position, obj.Key.position + obj.Value, mLockonColor);
                    }
                }
            }
        }
    }

    public void LockonToWalk()
    {
        // Movement
        mPlayerMovement.SetWalkSpeed(mWalkSpeedOriginal);   // might fix later

        // Enabling standard camera, disabling aim camera
        mCameraStandard.Priority = 10;
        mCameraLockon.Priority = 1;
        mCameraAim.Priority = 1;

        // Resetting standard camera follow target
        mCameraStandard.m_Follow = transform;
        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, true));

        // Resetting lock-on persistent forward direction
        mPlayerMovement.SetForwardLockonPersistent(Vector3.zero);

        // Resetting mPlayerMovement's mLockonTarget
        mPlayerMovement.SetLockonTarget(null);

        // Disabling reticle
        mReticle.enabled = false;
        mReticle.transform.position = mReticlePositionInitial.position;
        mReticle.transform.localScale = Vector3.one;

        // Clearing lockon target list
        //mLockonList.Clear();
        mLockonDictionary.Clear();

        // Clearing lockon target
        mLockonTarget = null;
        mPlayerMovement.SetLockonTarget(mLockonTarget);

        // Setting the vertical parameter to what is was before aim mode
        mCameraStandard.m_YAxis.Value = mCameraStandardYAxis;
        mCameraStandardYAxis = 0.5f;

        // Resetting standard camera follow target
        mCameraStandard.m_Follow = transform;
        mCameraStandard.m_LookAt = mCameraStandardFocus;
        StartCoroutine(SetBindingMode(CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp, false));
    }
}