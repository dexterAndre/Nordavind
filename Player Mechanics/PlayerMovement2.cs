using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement2 : MonoBehaviour
{
    /*
        General (all scripts) to do: 
        - Use properties instead of getters and setters
        - Use UnityEvent and Delegates
        - Remove [SerializeField] on #region References code. Not interesting. 
    */

    /*
        To do: 
        - Delete unused getter / setter functions
    */

    #region State
    public enum State
    {
        Walk,
        Jump,
        JumpDelay,
        Air,
        Hang,
        Balance,
        Roll,
        Throw,
        Slide
    };
    [Header("State")]
    [SerializeField]
    private State mState = State.Walk;
    public State GetState() { return mState; }
    public void SetState(State state) { mState = state; }
    #endregion
    #region Walk
    [Header("Walking")]
    [SerializeField]
    private float mWalkSpeed = 7.5f;
    public float GetWalkSpeed() { return mWalkSpeed; }
    public void SetWalkSpeed(float walkSpeed) { mWalkSpeed = walkSpeed; }
    [SerializeField]
    private float mAirInfluence = 0.5f;
    public float GetAirInfluence() { return mAirInfluence; }
    public void SetAirInfluence(float airInfluence) { mAirInfluence = airInfluence; }
    private Vector3 mMovementVector = Vector3.zero;
    public Vector3 GetMovementVector() { return mMovementVector; }
    public void SetMovementVector(Vector3 vector) { mMovementVector = vector; }
    [SerializeField]
    [Range(0f, 1f)]
    private float mRotationSpeed = 0.3f;
    #endregion
    #region Gravity
    [SerializeField]
    private float mGravityScale = 1f;
    private Vector3 mVerticalMovement;
    public Vector3 GetVerticalMovement() { return mVerticalMovement; }
    public void SetVerticalMovement(Vector3 verticalMovement) { mVerticalMovement = verticalMovement; }
    #endregion
    #region Jump
    private float mJumpTimer = 0f;
    public void SetJumpTimer(float jumpTimer) { mJumpTimer = jumpTimer; }
    private Vector3 mJumpVector;
    public Vector3 GetJumpVector() { return mJumpVector; }
    public void SetJumpVector(Vector3 jumpVector) { mJumpVector = jumpVector; }
    #endregion
    #region Hang
    private bool mCanHang = true; // is set to false if dropping down (or after set amount of time). Resets when hitting ground. 
    #endregion
    #region Other
    // Other mechanics
    private Vector3 mRollVector = Vector3.zero;
    #endregion
    #region References
    [Header("References")]
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private CharacterController mCharacterController = null;
    #endregion
    #region Debug
    #region Movement
    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingMovement = true;
    [SerializeField]
    private Vector3 mDebugMovementOffset = new Vector3(0f, 2f, 0f);
    [SerializeField]
    private Color mDebugMovementColor = Color.red;
    #endregion
    [Space(10)]
    #region Hanging
    [SerializeField]
    private bool mIsDebuggingHanging = true;
    [SerializeField]
    private Vector3 mDebugHangingOffset = new Vector3(0f, 2f, 0f);
    [SerializeField]
    private Color mDebugHangingColor = new Color(255f / 255f, 125f / 255f, 0f / 255f, 1f);
    #endregion
    #region Sliding
    #endregion
    #endregion



    private void Awake ()
	{
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>(); // optimize this! 
        if (mCharacterController == null)
            mCharacterController = GetComponent<CharacterController>();
	}

	private void FixedUpdate ()
	{
        #region Automatic State Detection
        // Check if airborne
        if (!mCharacterController.isGrounded
            && mState != State.Hang
            && mState != State.Balance)
        {
            mState = State.Air;
        }

        // Airborne transitions
        if (mState == State.Air)
        {
            // Check for hang
            if (CheckHang())
            {
                
            }
            // Check for balance
            if (CheckBalance())
            {
                
            }

            // Jump vector
            if (mJumpVector != Vector3.zero)
            {
                mJumpTimer += Time.fixedDeltaTime;
            }

            // Check for grounded
            if (mState == State.Air
                && mCharacterController.isGrounded)
            {
                mState = State.Walk;
                mVerticalMovement = Vector3.zero;
                mJumpVector = Vector3.zero;
                mJumpTimer = 0f;
            }
            else
            {
                // Else apply gravity
                mVerticalMovement += Physics.gravity * mGravityScale * Time.fixedDeltaTime;
            }
        }
        #endregion

        Movement(mState);
	}

    private void Movement(State state)
    {
        switch (state)
        {
            case State.Walk:
                {
                    // Storing movement vector
                    mMovementVector = PlanarMovement(new Vector2(
                        mInputManager.GetStickLeft().x, 
                        mInputManager.GetStickLeft().y));

                    // Applying rotation
                    transform.forward = Vector3.Slerp(transform.forward, mMovementVector, mRotationSpeed);

                    // Applying constant downwards force
                    mMovementVector += new Vector3(0f, Physics.gravity.y, 0f) * Time.fixedDeltaTime;

                    // Applying movement
                    mCharacterController.Move(
                        mMovementVector 
                        * mWalkSpeed 
                        * Time.fixedDeltaTime);

                    break;
                }
            case State.Air:
                {
                    // Jump vector
                    if (mJumpVector != Vector3.zero)
                    {
                        mJumpVector *= 1f / (1f + mJumpTimer * 0.1f);
                    }

                    // Storing movement vector
                    mMovementVector = PlanarMovement(new Vector2(
                        mInputManager.GetStickLeft().x,
                        mInputManager.GetStickLeft().y));

                    // Applying rotation
                    transform.forward = Vector3.Slerp(transform.forward, mMovementVector, mRotationSpeed);

                    // Collecting influence vectors
                    mMovementVector += mVerticalMovement;
                    mMovementVector += mJumpVector;

                    // Applying downwards and player-controlled movement
                    mCharacterController.Move(
                        mMovementVector 
                        * mWalkSpeed 
                        * mAirInfluence 
                        * Time.fixedDeltaTime);

                    break;
                }
            // One frame before jumping, this hack bypasses the isGrounded issue. 
            case State.Jump:
                {
                    // Applying rotation
                    transform.forward = Vector3.ProjectOnPlane(
                        Vector3.Slerp(
                            transform.forward, 
                            mMovementVector, 
                            mRotationSpeed), 
                        Vector3.up).normalized;

                    // Applying downwards and player-controlled movement
                    mCharacterController.Move(
                        (mMovementVector + mJumpVector)
                        * mWalkSpeed
                        * Time.fixedDeltaTime);

                    break;
                }
            case State.JumpDelay:
                {
                    // Applying rotation
                    transform.forward = Vector3.ProjectOnPlane(
                        Vector3.Slerp(
                            transform.forward,
                            mMovementVector,
                            mRotationSpeed),
                        Vector3.up).normalized;



                    break;
                }
            case State.Roll:
                {
                    break;
                }
            case State.Throw:
                {
                    // Storing movement vector
                    mMovementVector = PlanarMovement(new Vector2(
                        mInputManager.GetStickLeft().x,
                        mInputManager.GetStickLeft().y));

                    // Applying rotation
                    transform.Rotate(
                        0f, 
                        mInputManager.GetStickRight().x 
                        * mInputManager.GetCameraAimSensitivity().x 
                        * 2f
                        * Time.deltaTime, 
                        0f);

                    // Applying constant downwards force
                    mMovementVector += new Vector3(0f, Physics.gravity.y, 0f) * Time.fixedDeltaTime;

                    // Applying movement
                    mCharacterController.Move(
                        mMovementVector 
                        * mWalkSpeed 
                        * Time.fixedDeltaTime);

                    break;
                }
            case State.Hang:
                {
                    break;
                }
            case State.Balance:
                {
                    break;
                }
            case State.Slide:
                {
                    break;
                }
            default:
                {
                    print("This should never trigger!");
                    break;
                }
        }
    }



    /// <summary>
    /// Projects vector onto xz-plane. 
    /// <para>Accepts hoizontal and vertical input in the form of a Vector2. </para>
    /// </summary>
    static public Vector3 PlanarMovement(Vector2 input)
    {
        Vector3 output = new Vector3(input.x, 0f, input.y);
        Quaternion camDir = Camera.main.transform.rotation;
        output = camDir * output;
        output = Vector3.ProjectOnPlane(output, Vector3.up);
        return output;
    }

    private bool CheckHang()
    {
        // Temporary!!!
        return false;

        // Are you allowed to hang? 
        if (mCanHang)
        {
            // If upper raycast hits nothing...
            if (mCanHang)
            {
                // If inset raycast hits something...
                if (mCanHang)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }

    private bool CheckBalance()
    {
        // Temporary!!!
        return false;
    }
}