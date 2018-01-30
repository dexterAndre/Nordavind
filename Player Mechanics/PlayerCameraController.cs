using UnityEngine;
using System.Collections;
using Cinemachine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCameraController : MonoBehaviour
{
    /*
        To-do: 
        - Bug: when transitioning from aim to standard, if you rotate avatar, camera behaves wonkily
    */
    [Header("Camera Settings")]
    [SerializeField]
    private float mTransitionTime = 0.5f;
    public float GetTransitionTime() { return mTransitionTime; }
    public void SetTransitionTime(float time) { mTransitionTime = time; }

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
    private float mAimVerticalParameter = 0.25f;
    private float mRadiusScale = 1f;

    [Header("References")]
    [SerializeField]
    private CinemachineFreeLook mCameraStandard = null;
    [SerializeField]
    private CinemachineFreeLook mCameraAim = null;
    [SerializeField]
    private CinemachineBrain mCameraBrain = null;
    [SerializeField]
    private PlayerMovement mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;

    // Default value storage
    private float[,] mCameraStandardRigMeasurements = new float[3, 2];
    private float[,] mCameraAimRigMeasurements = new float[3, 2];



	private void Awake ()
	{
        // Quick setups
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement>();
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        // Standard third-person camera
        if (mCameraStandard == null)
            mCameraStandard = GameObject.Find("Camera Rig Standard").GetComponent<CinemachineFreeLook>();
        mCameraStandard.Priority = 10;
        // Setting this as follow target
        if (mCameraStandard.Follow == null)
            mCameraStandard.Follow = transform;
        // Setting "Camera Standard Focus" as look-at target
        if (mCameraStandard.LookAt == null)
            mCameraStandard.LookAt = transform
                .GetChild(3).transform
                .GetChild(0).transform;

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

        // Camera brain
        if (mCameraBrain == null)
            mCameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        mCameraBrain.m_DefaultBlend.m_Time = mTransitionTime;

        // Rig measurements
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
    }

	private void LateUpdate ()
	{
        // Standard mode
        if (mPlayerMovement.GetState() != PlayerMovement.State.Throw)
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
        else
        // Aim mode
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
}