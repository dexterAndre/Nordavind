using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerMovement2))]
public class PlayerRoll2 : MonoBehaviour
{
    /* 
        To do: 
    */


    #region Roll
    [Header("Roll")]
    [SerializeField]
    private float mRollSpeed = 10f;
    [SerializeField]
    private float mRollDuration = 0.05f;
    private float mRollTimer = 0f;
    [SerializeField]
    private float mRollDelayDuration = 0.035f;
    private float mRollDelayTimer = 0f;
    [Header("Cooldown")]
    [SerializeField]
    private float mRollCooldown = 2f;
    private float mRollCooldownTimer = 0f;
    #endregion
    #region References
    [Header("References")]
    [SerializeField]
    private PlayerMovement2 mPlayerMovement = null;
    [SerializeField]
    private InputManager mInputManager = null;
    #endregion
    #region Debug
    [Header("Debug")]
    [SerializeField]
    private bool mIsDebuggingRoll = true;
    [SerializeField]
    private Vector3 mDebugOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField]
    private Color mDebugColor = new Color(255f, 125f, 255f, 255f);
    #endregion



    private void Awake()
    {
        if (mPlayerMovement == null)
            mPlayerMovement = GetComponent<PlayerMovement2>();
        if (mInputManager == null)
            mInputManager = GameObject.Find("Input Manager").GetComponent<InputManager>(); // optimize this! 
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") 
            && mRollCooldownTimer == 0f 
            && mRollDelayTimer == 0f)
        {
            Roll();
        }
    }

    private void FixedUpdate ()
	{
        // Cooldown
        if (mRollCooldownTimer > 0f)
        {
            mRollCooldownTimer += Time.fixedDeltaTime;
            if (mRollCooldownTimer >= mRollCooldown)
            {
                mRollCooldownTimer = 0f;
            }
        }

        // Roll timer
        if (mRollTimer > 0f)
        {
            mRollTimer += Time.fixedDeltaTime;
            if (mRollTimer >= mRollDuration)
            {
                mRollTimer = 0f;
                mPlayerMovement.SetState(PlayerMovement2.State.RollDelay);
            }
        }

        // After rolling (some delay time to prevent spamming)
        if (mRollDelayTimer > 0f)
        {
            mRollDelayTimer += Time.fixedDeltaTime;
            if (mRollDelayTimer >= mRollDelayDuration)
            {
                mRollDelayTimer = 0f;
                mPlayerMovement.SetState(PlayerMovement2.State.Walk);
            }
        }
        //else if (mRollDelayTimer == 0f && mPlayerMovement.GetState() == PlayerMovement2.State.RollDelay)
        //{
        //    if (mRollDelayTimer > 0f)
        //    {
        //        mRollDelayTimer += Time.fixedDeltaTime;
        //    }
        //}
    }

    private void Roll()
    {
        // Executing roll
        mPlayerMovement.SetState(PlayerMovement2.State.Roll);
        Vector3 inputVector = PlayerMovement2.PlanarMovement(new Vector2(mInputManager.GetStickLeft().x, mInputManager.GetStickLeft().y));
        mPlayerMovement.SetMovementVector(inputVector * mRollSpeed);
    }
}