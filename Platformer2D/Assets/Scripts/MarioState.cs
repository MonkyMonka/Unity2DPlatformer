using UnityEngine;


public enum EMarioState : byte
{
    Idle,
    Walking,
    Jumping,
    Falling,
    Ducking,
    Dead
}

public enum EMarioForm : byte
{
    Small,
    Super
}

public enum EMarioDirection : byte
{
    Right,
    Left
}

public class MarioState : MonoBehaviour
{
    private EMarioState state = EMarioState.Idle;
    private EMarioForm form = EMarioForm.Small;
    private EMarioDirection direction = EMarioDirection.Right;

    private int runningMeter = 0;
    private float invincibilityTimer = 0.0f;

    private bool isRunning = false;
    private bool isOnGround = true;

    public EMarioState State
    {
        get { return state; }
        set { state = value; }
    }

    public EMarioForm Form
    {
        get { return form; }
        set { form = value; }
    }

    public EMarioDirection Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public float DirectionScalar
    {
        get { return Direction == EMarioDirection.Left ? -1.0f : 1.0f; }
    }

    public int RunningMeter
    {
        get { return runningMeter; }
        set { runningMeter = value; }
    }

    public float InvincibilityTimer
    {
        get { return invincibilityTimer; }
        set { invincibilityTimer = value; }
    }

    public bool IsInvincible
    {
        get { return InvincibilityTimer > 0.0f; }
    }

    public bool IsRunning
    {
        get { return isRunning; }
        set { isRunning = value; }
    }

    public bool IsOnGround
    {
        get { return isOnGround; }
        set { isOnGround = value; }
    }
}
