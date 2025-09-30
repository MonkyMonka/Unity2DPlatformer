using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum EGoombaState : byte
{
    Unknown,
    Walking,
    Squished
}

public enum EGoombaInitialDirection : byte
{
    Random,
    Left,
    Right
}

public class Goomba : Enemy
{
    public EnemySettings settings;

    public BoxCollider2D primaryCollider;
    public BoxCollider2D frontTrigger;
    public BoxCollider2D backTrigger;
    public EGoombaInitialDirection initialDirection = EGoombaInitialDirection.Random;

    private new Rigidbody2D rigidbody;
    private Animator animator;

    private EGoombaState state = EGoombaState.Unknown;
    private float squishedDuration = 0.0f;
    private Vector2 velocity = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the rigidbody and animator components
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Set the enemy type
        enemyType = EEnemyType.Goomba;

        // Set the state to walking
        SetState(EGoombaState.Walking);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EGoombaState.Squished)
        {
            if (squishedDuration > 0.0f)
            {
                squishedDuration -= Time.deltaTime * Game.Instance.LocalTimeScale;

                if (squishedDuration < 0.0f)
                {
                    squishedDuration = 0.0f;
                    Destroy(gameObject);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (state == EGoombaState.Walking)
        {
            Vector2 location = rigidbody.position;
            location += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
            rigidbody.position = location;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Is the Goomba colliding with the Mario GameObject?
        if (collision.gameObject.CompareTag("Mario"))
        {
            // Get the Mario component from the GameObject
            Mario mario = collision.gameObject.GetComponent<Mario>();

            // Check if there's a contact object, in the contacts array
            if (collision.contacts.Length > 0)
            {
                // Get the normal from the first contact object
                Vector2 normal = collision.contacts[0].normal;

                // Ensure the Goomba's state is walking
                if (state == EGoombaState.Walking)
                {
                    if (normal.x <= -0.8f || normal.x >= 0.8f)
                    {
                        // Goomba collided with Mario on the side
                        mario.HandleDamage();
                    }
                    else if (normal.y >= 0.7f)
                    {
                        // Goomba landed on Mario
                        mario.HandleDamage();
                    }
                    else if (normal.y <= -0.7f)
                    {
                        // Mario landed on the Goomba, make Mario bounce off the enemy
                        MarioMovement marioMovement = collision.gameObject.GetComponent<MarioMovement>();
                        marioMovement.ApplyBounce();

                        SetState(EGoombaState.Squished);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool flipMovementDirection = false;

        if (collision.gameObject.CompareTag("World"))
        {
            // The Goomba collided with the World
            flipMovementDirection = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // The Goomba collided with another Enemy
            EEnemyType enemyType = collision.gameObject.GetComponent<Enemy>().EnemyType;

            if (enemyType == EEnemyType.Goomba)
            {
                flipMovementDirection = true;
            }
        }
        else if (collision.gameObject.CompareTag("Pickup"))
        {
            flipMovementDirection = true;
        }

        // DO we need to flip the Goomba's direction
        if (flipMovementDirection)
        {
            // Get the Goomba's overlapping collider
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> results = new List<Collider2D>();
            collision.Overlap(filter, results);

            // Set the Goomba's velocity based on which collider was triggered 
            if (results.Contains(frontTrigger))
            {
                velocity.x = -settings.GoombaSpeed;
            }
            else if (results.Contains(backTrigger))
            {
                velocity.x = settings.GoombaSpeed;
            }
        }
    }

    public void SetState(EGoombaState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EGoombaState.Walking)
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                primaryCollider.isTrigger = false;
                ApplyInitialVelocity();
            }
            else if (state == EGoombaState.Squished)
            {
                squishedDuration = settings.GoombaSquishedDuration;
                rigidbody.bodyType = RigidbodyType2D.Static;
                primaryCollider.isTrigger = true;
            }

            UpdateAnimator();
        }
    }

    private void UpdateAnimator()
    {
        if (state == EGoombaState.Walking)
        {
            animator.Play("GoombaWalk");
        }
        else if (state == EGoombaState.Squished)
        {
            animator.Play("GoombaSquished");
        }
    }

    private void ApplyInitialVelocity()
    {
        if (initialDirection == EGoombaInitialDirection.Random)
        {
            int index = UnityEngine.Random.Range(0, 10) % 2;
            float[] speeds = { settings.GoombaSpeed, -settings.GoombaSpeed };
            velocity.x = speeds[index];
        }
        else if (initialDirection == EGoombaInitialDirection.Right)
        {
            velocity.x = settings.GoombaSpeed;
        }
        else if (initialDirection == EGoombaInitialDirection.Left)
        {
            velocity.x = -settings.GoombaSpeed;
        }
    }
}
