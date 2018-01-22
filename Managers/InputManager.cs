using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    [Header("Input Containers")]
    [SerializeField]
    private Vector2 mStickLeft = Vector2.zero;
    public Vector2 GetStickLeft() { return mStickLeft; }
    [SerializeField]
    private Vector2 mStickRight = Vector2.zero;
    public Vector2 GetStickRight() { return mStickRight; }
    [SerializeField]
    private Vector2 mDPad = Vector2.zero;
    public Vector2 GetDPad() { return mDPad; }
    [SerializeField]
    private Vector2 mTriggers = Vector2.zero;
    public Vector2 GetTriggers() { return mTriggers; }
    


    [Header("Context Sensitivity")]
    [SerializeField]
    private Vector2 mMovementSensitivity = Vector2.one;
    public Vector2 GetMovementSensitivity() { return mMovementSensitivity; }
    [SerializeField]
    [Tooltip("x: yaw rotation input. y: vertical parameter change per second. ")]
    private Vector2 mCameraStandardSensitivity = Vector2.one;
    public Vector2 GetCameraStandardSensitivity() { return mCameraStandardSensitivity; }
    [SerializeField]
    [Tooltip("Change in radius multiplier per second. ")]
    private float mCameraStandardDollySensitivity = 1f;
    public float GetCameraStandardDollySensitivity() { return mCameraStandardDollySensitivity; }
    [SerializeField]
    [Tooltip("x: avatar yaw rotation per second. y: vertical parameter change per second. ")]
    private Vector2 mCameraAimSensitivity = Vector2.one;
    public Vector2 GetCameraAimSensitivity() { return mCameraAimSensitivity; }
    [SerializeField]
    [Tooltip("Change in radius multiplier per second. ")]
    private float mCameraAimDollySensitivity = 1f;
    public float GetCameraAimDollySensitivity() { return mCameraAimDollySensitivity; }

    [Header("Controllers")]
    [SerializeField]
    // Placeholder thing. Do more like these, but meaningful! 
    private float mSensitivityBeforeMouseDetected = 1f;



    private void Update()
    {
        // Left stick
        mStickLeft.x = Input.GetAxisRaw("Horizontal");
        mStickLeft.y = Input.GetAxisRaw("Vertical");

        // Right stick
        mStickRight.x = Input.GetAxisRaw("RHorizontal");
        mStickRight.y = Input.GetAxisRaw("RVertical");

        // D-Pad
        mDPad.x = Input.GetAxisRaw("DPadX");
        mDPad.y = Input.GetAxisRaw("DPadY");

        // Triggers
        mTriggers.x = Input.GetAxisRaw("TriggerL");
        mTriggers.y = Input.GetAxisRaw("TriggerR");
    }
}