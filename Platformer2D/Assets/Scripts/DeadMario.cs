using UnityEngine;

public class DeadMario : MonoBehaviour
{
    public MarioSettings settings;

    private new Rigidbody2D rigidbody;
    private float holdInPlaceTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;

        holdInPlaceTimer = settings.DeadHoldTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (holdInPlaceTimer > 0.0f)
        {
            holdInPlaceTimer -= Time.deltaTime;
            if (holdInPlaceTimer <= 0.0f)
            {
                holdInPlaceTimer = 0.0f;

                rigidbody.gravityScale = 2.0f;
                rigidbody.AddForce(new Vector2(0.0f, settings.DeadForceY), ForceMode2D.Impulse);
            }
        }
    }
}
