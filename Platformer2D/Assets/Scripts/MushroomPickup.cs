using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPickup : Pickup
{
    public BoxCollider2D frontTrigger;
    public BoxCollider2D backTrigger;
    protected new Rigidbody2D rigidbody;
    protected Vector2 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        pickupType = EPickupType.Mushroom;
    }

    private void FixedUpdate()
    {
        if (pickupState == EPickupState.Active)
        {
            rigidbody.position += velocity * Time.deltaTime * Game.Instance.LocalTimeScale;
        }
    }

    protected override void OnPickupActive()
    {
        base.OnPickupActive();

        velocity.x = (UnityEngine.Random.Range(0, 10) % 2 == 0) ? (settings.MushroomSpeed) : (-settings.MushroomSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickupState == EPickupState.Active)
        {
            if (collision.gameObject.CompareTag("World") || collision.gameObject.CompareTag("Enemy"))
            {
                ContactFilter2D filter = new ContactFilter2D().NoFilter();
                List<Collider2D> results = new List<Collider2D>();
                collision.Overlap(filter, results);

                if (results.Contains(frontTrigger))
                {
                    velocity.x = -settings.MushroomSpeed;
                }
                else if (results.Contains(backTrigger))
                {
                    velocity.x = settings.MushroomSpeed;
                }
            }
        }
    }
}

