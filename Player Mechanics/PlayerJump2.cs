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
        if (Input.GetButtonDown("Jump")
            && mPlayerMovement.GetState() == PlayerMovement2.State.Walk)
        {
            switch (mJumpStyle)
            {
                case JumpStyle.Instant:
                    {
                        Jump(Vector3.up * mJumpForceInstant);
                        print("Jump!");
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
        mPlayerMovement.SetMovementVector(jumpForce);
    }

    private IEnumerator JumpDelay()
    {
        // Setting state
        mPlayerMovement.SetState(PlayerMovement2.State.Jump);

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
                            + (mCharacterController.velocity + Vector3.up * mJumpForceInstant)
                            .normalized 
                            * mDebuggVectorScale);
                        break;
                    }
                case JumpStyle.Delay:
                    {
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