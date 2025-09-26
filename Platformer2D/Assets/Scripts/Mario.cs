using UnityEngine;

public class Mario : MonoBehaviour
{
    public MarioSettings settings;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private new CapsuleCollider2D collider;

    private MarioController marioController;
    private MarioMovement marioMovement;
    private MarioState marioState;

    private float runningSegmentTimer = 0.0f;
    private float damagedTimer = 0.0f;

    private float previousAnimatorSpeed = 1.0f;
    private bool transformOrDamageAnimationIsRunning = false;
    private bool isMarioShowing = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();

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
        UpdateAnimator();

        // Update the Movement component's local time scale
        marioMovement.LocalTimeScale = Game.Instance.LocalTimeScale;

        // Ensure that the player isn't Dead
        if (marioState.State != EMarioState.Dead)
        {
            if (transformOrDamageAnimationIsRunning)
            {
                return;
            }

            // If Mario falls of the edge, he is no longer on the ground, the movement component 
            // catches this situation, update the IsOnGround value
            if (marioMovement.IsFalling() && marioState.IsOnGround)
            {
                marioState.IsOnGround = false;
            }

            // Handle the situation where Mario is Invincible, this
            // occurs right after Mario takes damage
            if (marioState.IsInvincible)
            {
                marioState.InvincibilityTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

                if (marioState.InvincibilityTimer <= 0.0f)
                {
                    marioState.InvincibilityTimer = 0.0f;
                    gameObject.layer = LayerMask.NameToLayer("Mario");
                    spriteRenderer.enabled = true;
                }
                else
                {
                    damagedTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;
                    if (damagedTimer <= 0.0f)
                    {
                        damagedTimer = settings.InvincibleVisibilityDuration;
                        spriteRenderer.enabled = !spriteRenderer.enabled;
                    }
                }
            }

            // Handle the situation where Mario is Running 
            if (marioState.IsRunning && marioMovement.IsMovementBlocked == false)
            {
                if (marioController.GetMoveValue() != 0.0f && marioState.IsOnGround)
                {
                    // Increment the segment timer
                    runningSegmentTimer += Time.deltaTime * Game.Instance.LocalTimeScale;

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
                            UpdateAnimator();
                        }

                        // Set the jump max time to factor in the running meter, running faster means Mario jumps higher and thus farther
                        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeRunning + settings.JumpIncreasePerSegment * (float)marioState.RunningMeter;

                        // Set character movement's max walk speed to factor in the running meter
                        marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed + (marioState.RunningMeter * settings.RunSpeedPerSegment);

                        // Set the flipbook's playback rate based on the running meter
                        float playRate = 1.0f + marioState.RunningMeter * 0.75f;
                        animator.speed = playRate;
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
                    runningSegmentTimer += Time.deltaTime * Game.Instance.LocalTimeScale;

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

                            // Reset the flipbook's playback rate to 1.0f
                            animator.speed = 1.0f;
                        }
                    }
                    else
                    {
                        // Set the jump max time to factor in the running meter, running faster means Mario jumps higher and thus farther
                        marioMovement.JumpMaxHoldTime = settings.JumpMaxHoldTimeRunning + settings.JumpIncreasePerSegment * (float)marioState.RunningMeter;

                        // Set character movement's max walk speed to factor in the running meter
                        marioMovement.MaxWalkSpeed = settings.MaxWalkSpeed + (marioState.RunningMeter * settings.RunSpeedPerSegment);

                        // Set the flipbook's playback rate based on the running meter
                        float playRate = 1.0f + marioState.RunningMeter * 0.75f;
                        animator.speed = playRate;
                    }
                }
            }

            // Mario has fallen off the edge of the level and has died
            if (transform.position.y < Game.Instance.Settings.DestroyActorAtY)
            {
                MarioHasDied(false);
            }

            if (marioState.RunningMeter > 0)
            {
                Debug.Log("Running meter: " + marioState.RunningMeter);
            }
        }
    }

    public void ResetMario(Vector2 location)
    {
        gameObject.SetActive(true);

        transform.position = location;

        if (marioState.State == EMarioState.Dead)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            transform.localScale = scale;

            marioState.Direction = EMarioDirection.Right;
        }

        // Set the state back to Idle
        ApplyStateChange(EMarioState.Idle);

        // Clear the movement forces
        marioMovement.ClearAccumulatedForces();
    }

    public void ApplyStateChange(EMarioState newState)
    {
        // Ensure the new mario state is different than the current state
        if (marioState.State == newState)
        {
            return;
        }

        // Assign the new state
        EMarioState oldState = marioState.State;
        marioState.State = newState;

        if (newState == EMarioState.Jumping)
        {
            marioMovement.CheckJumpApex = true;
        }

        // Handle the duck resize and reposition when mario is big
        if (marioState.Form == EMarioForm.Super)
        {
            if (newState == EMarioState.Ducking)
            {
                // Update the collider's size and offset 
                collider.offset = new Vector2(-0.03f, 0.487f);
                collider.size = new Vector2(0.765f, 0.93f);
            }
            else if (oldState == EMarioState.Ducking)
            {
                // Update the collider's size and offset
                collider.offset = new Vector2(-0.03f, 0.85f);
                collider.size = new Vector2(0.765f, 1.66f);
            }
        }
            // Lastly, update the animator
            UpdateAnimator();
    }

    public void ApplyTransformChange(EMarioForm newForm)
    {
        // Ensure the new mario form is different than the current form
        if (marioState.Form == newForm)
        {
            return;
        }

        // Assign the new form and then set the IsTransforming flag to true
        EMarioForm oldForm = marioState.Form;
        marioState.Form = newForm;

        if (oldForm == EMarioForm.Small && newForm == EMarioForm.Super)
        {
            Game.Instance.PauseActors();

            transformOrDamageAnimationIsRunning = true;
            previousAnimatorSpeed = animator.speed;
            animator.speed = 1.0f;
            animator.Play("MarioTransform");
        }
        else if (oldForm == EMarioForm.Super && newForm == EMarioForm.Small)
        {
            transformOrDamageAnimationIsRunning = true;
            previousAnimatorSpeed = animator.speed;
            animator.speed = 1.0f;
            animator.Play("MarioDamage");
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

    public void Duck()
    {
        ApplyStateChange(EMarioState.Ducking);
    }

    public void StopDucking()
    {
        MarioController marioController = GetComponent<MarioController>();
        if (marioController)
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

    public void HandleDamage()
    {
        if (marioState.Form == EMarioForm.Small)
        {
            MarioHasDied(true);
        }
        else if (marioState.Form == EMarioForm.Super)
        {
            damagedTimer = settings.InvincibleVisibilityDuration;
            marioState.InvincibilityTimer = settings.InvincibleTime;
            gameObject.layer = LayerMask.NameToLayer("MarioInvincible");

            ApplyTransformChange(EMarioForm.Small);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EEnemyType enemyType = collision.gameObject.GetComponent<Enemy>().EnemyType;

            if (enemyType == EEnemyType.PiranhaPlant)
            {
                PiranhaPlant piranhaPlant = collision.gameObject.GetComponent<PiranhaPlant>();
                if (piranhaPlant.State != EPiranhaPlantState.Hiding)
                {
                    HandleDamage();
                }
            }
        }
        else if (collision.gameObject.CompareTag("Pickup"))
        {
            EPickupType pickupType = collision.gameObject.GetComponent<Pickup>().PickupType;

            if (pickupType == EPickupType.Mushroom)
            {
                if (marioState.Form == EMarioForm.Small)
                    ApplyTransformChange(EMarioForm.Super);
            }

            // Destroy the pickup gameObject
            Destroy(collision.gameObject);
        }
    }


    private void MarioHasDied(bool spawnDeadMario)
    {
        // Ensure that mario isn't dead
        if (marioState.State != EMarioState.Dead)
        {
            // Set the state change to Dead
            ApplyStateChange(EMarioState.Dead);

            // Deactivate the gameObject
            gameObject.SetActive(false);

            // Spawn the Dead mario
            Game.Instance.MarioHasDied(spawnDeadMario);
        }
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

    private void UpdateAnimator()
    {
        // Don't update the animator if mario is transforming or damaged
        if (transformOrDamageAnimationIsRunning)
        {
            return;
        }

        if (marioState.Form == EMarioForm.Small)
        {
            if (marioState.State == EMarioState.Idle || marioState.State == EMarioState.Ducking)
            {
                animator.Play("MarioSmallIdle");
            }
            else if (marioState.State == EMarioState.Walking)
            {
                if (marioMovement.IsSkidding() == false)
                {
                    if (marioState.IsRunning && marioState.RunningMeter == settings.MaxRunningMeter)
                    {
                        animator.Play("MarioSmallRun");
                    }
                    else
                    {
                        animator.Play("MarioSmallWalk");
                    }
                }
                else
                {
                    animator.Play("MarioSmallTurn");
                }
            }
            else if (marioState.State == EMarioState.Jumping || marioState.State == EMarioState.Falling)
            {
                if (marioState.IsRunning && marioState.RunningMeter == settings.MaxRunningMeter)
                {
                    animator.Play("MarioSmallRunJump");
                }
                else
                {
                    animator.Play("MarioSmallJump");
                }
            }
        }
        else if (marioState.Form == EMarioForm.Super)
        {
            if (marioState.State == EMarioState.Idle)
            {
                animator.Play("MarioSuperIdle");
            }
            else if (marioState.State == EMarioState.Walking)
            {
                if (marioMovement.IsSkidding() == false)
                {
                    if (marioState.IsRunning && marioState.RunningMeter == settings.MaxRunningMeter)
                    {
                        animator.Play("MarioSuperRun");
                    }
                    else
                    {
                        animator.Play("MarioSuperWalk");
                    }
                }
                else
                {
                    animator.Play("MarioSuperTurn");
                }
            }
            else if (marioState.State == EMarioState.Jumping)
            {
                if (marioState.IsRunning && marioState.RunningMeter == settings.MaxRunningMeter)
                {
                    animator.Play("MarioSuperRunJump");
                }
                else
                {
                    animator.Play("MarioSuperJump");
                }
            }
            else if (marioState.State == EMarioState.Falling)
            {
                if (marioState.IsRunning && marioState.RunningMeter == settings.MaxRunningMeter)
                {
                    animator.Play("MarioSuperRunJump");
                }
                else
                {
                    animator.Play("MarioSuperJumpApex");
                }
            }
            else if (marioState.State == EMarioState.Ducking)
            {
                animator.Play("MarioSuperDuck");
            }
        }
    }

    private void EnabledMario(bool enabled)
    {
        if (enabled)
        {

        }
    }

    private void OnTransformAnimationFinished()
    {
        Game.Instance.UnpauseActors();

        transformOrDamageAnimationIsRunning = false;
        animator.speed = previousAnimatorSpeed;
        previousAnimatorSpeed = 1.0f;

        UpdateAnimator();

        // Update the collider's size and offset
        collider.offset = new Vector2(-0.03f, 0.85f);
        collider.size = new Vector2(0.765f, 1.66f);
    }

    private void OnDamageAnimationFinished()
    {
        transformOrDamageAnimationIsRunning = false;
        animator.speed = previousAnimatorSpeed;
        previousAnimatorSpeed = 1.0f;

        UpdateAnimator();

        // Update the collider's size and offset
        collider.offset = new Vector2(-0.03f, 0.487f);
        collider.size = new Vector2(0.765f, 0.93f);
    }
}

