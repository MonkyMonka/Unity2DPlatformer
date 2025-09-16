using UnityEngine;

public class Mario : MonoBehaviour
{
    public MarioSettings settings;

    private MarioController marioController;
    private MarioMovement marioMovement;
    private MarioState marioState;

    private float runningSegmentTimer = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        marioController = GetComponent<MarioController>();
        marioMovement = GetComponent<MarioMovement>();
        marioState = GetComponent<MarioState>();

        // Movement properties
        marioMovement.MinWalkSpeed = settings.MinWalkSpeed;
        marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed;
        marioMovement.Acceleration = settings.WalkAcceleration;
        marioMovement.Deceleration = settings.Deceleration;
        marioMovement.DecelerationSkid = settings.DecelerationSkid;

        // Jump properties
        marioMovement.GravityScale = settings.GravityScale;
        marioMovement.AirControl = settings.AirControl;
        marioMovement.BounceForce = settings.BounceForce;
        marioMovement.JumpForce = settings.JumpForce;
        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeWalking;

        // Delegates
        marioMovement.FallingDelegate = OnFalling;
        marioMovement.JumpedDelegate = OnJumped;
        marioMovement.JumpApexDelegate = OnJumpApex;
        marioMovement.JumpLandedDelegate = OnJumpLanded;
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure that the player isn't Dead
        if (marioState.State != EMarioState.Dead)
        {
            // If Mario falls of the edge, he is no longer on the ground, the movement component 
            // catches this situation, update the IsOnGround value
            if (marioMovement.IsFalling() && marioState.IsOnGround)
            {
                marioState.IsOnGround = false;
            }

            // Handle the situation where Mario is Running 
            if (marioState.IsRunning && marioMovement.IsMovementBlocked == false)
            {
                if (marioController.GetMoveValue() != 0.0f && marioState.IsOnGround)
                {
                    // Increment the segment timer
                    runningSegmentTimer += Time.deltaTime;

                    // Is the segment timer greater than the duration constant?
                    if (runningSegmentTimer > settings.RunSegmentIncrementDuration)
                    {
                        // Reset the segment timer and increment the running meter
                        runningSegmentTimer = 0.0f;
                        marioState.RunningMeter++;

                        // Cap the running meter to the max constant and update the flipbook so that Mario's arms are out
                        if (marioState.RunningMeter >= settings.MaxRunningMeter)
                        {
                            marioState.RunningMeter = settings.MaxRunningMeter;
                        }

                        // Set the jump max time to factor in the running meter, running faster means Mario jumps higher and thus farther
                        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeRunning + settings.JumpIncreasePerSegment * (float)marioState.RunningMeter;

                        // Set character movement's max walk speed to factor in the running meter
                        marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed + (marioState.RunningMeter * settings.RunSpeedPerSegment);
                    }
                }
            }
            else
            {
                // If there's still some running meter built up, but the player is 
                // not running, gradually decrease the running meter
                if (marioState.RunningMeter > 0)
                {
                    // Increment the segment timer
                    runningSegmentTimer += Time.deltaTime;

                    // Is the segment timer greater than the duration constant?
                    if (runningSegmentTimer > settings.RunSegmentDecrementDuration)
                    {
                        // Reset the segment timer and decrement the running meter
                        runningSegmentTimer = 0.0f;
                        marioState.RunningMeter--;

                        // Ensure the running meter doesn't go lower than zero
                        if (marioState.RunningMeter <= 0)
                        {
                            marioState.RunningMeter = 0;

                            // Reset the Jump max time and the character's max walk speed to the walking default
                            marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeWalking;
                            marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed;
                        }
                    }
                    else
                    {
                        // Set the jump max time to factor in the running meter, running faster means Mario jumps higher and thus farther
                        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeRunning + settings.JumpIncreasePerSegment * (float)marioState.RunningMeter;

                        // Set character movement's max walk speed to factor in the running meter
                        marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed + (marioState.RunningMeter * settings.RunSpeedPerSegment);
                    }
                }
            }

            if (marioState.RunningMeter > 0)
            {
                Debug.Log("Running meter: " + marioState.RunningMeter);
            }
        }
    }

    public void ApplyStateChange(EMarioState newState)
    {
        // Ensure the new mario state is different than the current state
        if (marioState.State == newState)
        {
            return;
        }

        // Assign the new state
        marioState.State = newState;

        if (newState == EMarioState.Jumping)
        {
            marioMovement.CheckJumpApex = true;
        }
    }

    public void Run()
    {
        marioState.IsRunning = true;

        // Reset the the Running segment timer to zero, this 
        // timer determines when to increase the running meter
        runningSegmentTimer = 0.0f;

        // Set the jump max time to the running constant
        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeRunning;
    }

    public void StopRunning()
    {
        // Set the PlayerState's IsRunning flag to false
        marioState.IsRunning = false;

        // Reset the the Running segment timer to zero
        runningSegmentTimer = 0.0f;
    }

    private void OnFalling()
    {
        marioState.IsOnGround = false;
        ApplyStateChange(EMarioState.Falling);
    }

    private void OnJumped()
    {
        marioState.IsOnGround = false;
        ApplyStateChange(EMarioState.Jumping);
    }

    private void OnJumpApex()
    {
        if (marioState.IsOnGround == false)
        {
            ApplyStateChange(EMarioState.Falling);
        }
    }

    private void OnJumpLanded()
    {
        marioState.IsOnGround = true;

        if (marioController.IsDuckPressed())
        {
            ApplyStateChange(EMarioState.Ducking);
        }
        else
        {
            if (marioController.GetMoveValue() == 0.0f)
            {
                ApplyStateChange(EMarioState.Idle);
            }
            else
            {
                ApplyStateChange(EMarioState.Walking);
            }
        }
    }
}
