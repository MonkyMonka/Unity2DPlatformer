using UnityEngine;

[CreateAssetMenu(fileName = "MarioSettings", menuName = "Mario/MarioSettings")]
public class MarioSettings : ScriptableObject
{
    [Header("Lives")]
    public int DefaultStartingLives = 4;

    [Header("Jump")]
    public float GravityScale = 3.75f;
    public float AirControl = 1.0f;
    public float JumpForce = 26.5f;
    public float JumpMaxHoldTimeWalking = 0.1525f;
    public float JumpMaxHoldTimeRunning = 0.19125f;
    public float JumpIncreasePerSegment = 0.0025f;
    public float BounceForce = 40.0f;

    [Header("Walking")]
    public float MinWalkSpeed = 0.7f;
    public float MaxWalkSpeed = 14.0f;
    public float WalkAcceleration = 19.5f;
    public float Deceleration = 42.0f;
    public float DecelerationSkid = 64.0f;

    [Header("Running")]
    public int MaxRunningMeter = 7;
    public float RunAcceleration = 27.0f;
    public float RunSpeedPerSegment = 1.1f;
    public float RunSegmentIncrementDuration = 0.2f;
    public float RunSegmentDecrementDuration = 0.05f;

    [Header("Dead")]
    public float DeadHoldTime = 1.5f;
    public float DeadForceY = 26.0f;

    [Header("Invincibility")]
    public float InvincibleTime = 1.5f;
    public float InvincibleVisibilityDuration = 0.05f;
}
