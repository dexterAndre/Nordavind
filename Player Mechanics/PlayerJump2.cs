using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement2))]
public class PlayerJump2 : MonoBehaviour
{
    /*
        To do: 
    */

    #region Jump
    [System.Serializable]
    private enum JumpStyle
    {
        Instant,
        Delay
    };
    [Header("Jump")]
    [SerializeField]
    private JumpStyle mJumpStyle = JumpStyle.Instant;
    [Header("Jump Style: Instant")]
    [SerializeField]
    private float mJumpForceInstant = 10f;
    [Range(0f, 1f)]
    [SerializeField]
    [Tooltip("Directional jump weighting between user input (1) and up (0). ")]
    private float mJumpWeight = 0.75f;
    [Header("Jump Style: Delay")]
    [SerializeField]
    [Tooltip("Initial angle (in degrees) of the jump. ")]
    private float mJumpForceDelay = 10f;
    [SerializeField]
    private float mJumpAngleDelay = 45f;
    private float mJumpAngleDelayRemapped = 0f;
    [SerializeField]
    private float mJumpDelayDuration = 0.25f;
    private float mJumpDelayTimer = 0f;
    #endregion
    #region References
    [Header("References")]
    [SerializeField]
    private PlayerMovement2 mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    [SerializeField]
    private CharacterController mCharacterController = null;
    #endregion
    #region Debug
    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingJump = true;
    [SerializeField]
    private Vector3 mDebugOffset = new Vector3(0f, 2f, 0f);
    [SerializeField]
    private float mDebuggVectorScale = 0.2f;
    [SerializeField]
    private Color mDebugColor = Color.yellow;
    #endregion

    private void Awake ()
	{
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement2>();
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>(); // optimize this! 
        if (mCharacterController == null)
            mCharacterController = GetComponent<CharacterController>();
    }

	private void Update ()
	{
        switch(mJumpStyle)
        {
            case JumpStyle.Instant:
                {
                    break;
                }
            case JumpStyle.Delay:
                {
                    mJumpAngleDelayRemapped = mJumpAngleDelay / 90f;
                    break;
                }
            default:
                {
                    break;
                }
        }

        if (Input.GetButtonDown("Jump")
            && mPlayerMovement.GetState() == PlayerMovement2.State.Walk)
        {
            switch (mJumpStyle)
            {
                case JumpStyle.Instant:
                    {
                        //Jump((PlayerMovement2.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y)) + Vector3.up * mJumpForceInstant).normalized * mJumpForceInstant);
                        Vector3 inputVector = PlayerMovement2.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y));

                        if (inputVector != Vector3.zero)
                            Jump((mJumpWeight * inputVector + (1f - mJumpWeight) * Vector3.up).normalized * mJumpForceInstant);
                        else
                            Jump(Vector3.up * mJumpForceInstant);

                        break;
                    }
                case JumpStyle.Delay:
                    {
                        StartCoroutine(JumpDelay());
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
	}

    private void Jump(Vector3 jumpForce)
    {
        mPlayerMovement.SetState(PlayerMovement2.State.Jump);
        mPlayerMovement.SetJumpTimer(1f);
        mPlayerMovement.SetJumpVector(PlayerMovement2.PlanarMovement(new Vector2(
            mInputManager.GetStickLeft().x,
            mInputManager.GetStickLeft().y)) + jumpForce);
    }

    private IEnumerator JumpDelay()
    {
        // Setting state
        mPlayerMovement.SetState(PlayerMovement2.State.JumpDelay);

        // Apply wait time
        WaitForSeconds wait = new WaitForSeconds(mJumpDelayDuration);
        yield return wait;

        // Remapping mJumpAngleDelay from [0, 90] to [0, 1]. 
        mJumpAngleDelayRemapped = mJumpAngleDelay / 90f;

        // Apply jump
        Jump(Vector3.Slerp(transform.forward, Vector3.up, mJumpAngleDelayRemapped) * mJumpForceDelay);
    }

    private void OnDrawGizmosSelected()
    {
        if (mIsDebuggingJump)
        {
            Gizmos.color = mDebugColor;
            switch (mJumpStyle)
            {
                case JumpStyle.Instant:
                    {
                        Gizmos.DrawLine(
                            transform.position
                            + mDebugOffset,
                            transform.position 
                            + mDebugOffset
                            + (PlayerMovement2.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y)) + Vector3.up * mJumpForceInstant).normalized
                            * mDebuggVectorScale);
                        break;
                    }
                case JumpStyle.Delay:
                    {
                        Gizmos.DrawLine(
                               transform.position
                               + mDebugOffset,
                               transform.position
                               + mDebugOffset
                               + Vector3.Slerp(transform.forward, Vector3.up, mJumpAngleDelayRemapped)
                               * mDebuggVectorScale);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}