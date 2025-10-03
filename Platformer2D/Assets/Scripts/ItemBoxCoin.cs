using UnityEngine;

public class ItemBoxCoin : MonoBehaviour
{
    private new Rigidbody2D rigidbody;
    private Animator animator;
    private Vector2 start;

    private float GravityScale = 4.0f;
    private float ImpulseY = 28.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = GravityScale;
        rigidbody.AddForce(new Vector2(0.0f, ImpulseY), ForceMode2D.Impulse);

        animator = GetComponent<Animator>();
        animator.Play("ItemBoxCoin");

        start = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < start.y)
        {
            Destroy(gameObject);
        }
    }
}
