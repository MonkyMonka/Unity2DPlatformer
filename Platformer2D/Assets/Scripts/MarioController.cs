using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MarioController : MonoBehaviour
{
    private new Rigidbody2D rigidbody;

    private float direction;
    private const float speed = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigidbody.linearVelocity = new Vector2(direction * speed, rigidbody.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<float>();

        if (direction < 0.0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1.0f;
            transform.localScale = scale;
        }
        else if (direction > 0.0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1.0f;
            transform.localScale = scale;
        }
    }
}
