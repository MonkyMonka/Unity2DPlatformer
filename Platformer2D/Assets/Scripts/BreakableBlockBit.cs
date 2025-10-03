using UnityEngine;

public enum EBreakableBlockBitType : byte
{
    Unknown,
    Left,
    Right
}

public class BreakableBlockBit : MonoBehaviour
{
    public Sprite leftSprite;
    public Sprite rightSprite;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < Game.Instance.settings.DestroyActorAtY)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(EBreakableBlockBitType type, Vector2 impulse)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float direction = 0.0f;

        if (type == EBreakableBlockBitType.Left)
        {
            spriteRenderer.sprite = leftSprite;
            direction = -1.0f;
        }
        else if (type == EBreakableBlockBitType.Right)
        {
            spriteRenderer.sprite = rightSprite;
            direction = 1.0f;
        }

        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 2.5f;
        rigidbody.AddForce(impulse, ForceMode2D.Impulse);
        rigidbody.AddTorque(Mathf.PI * 2.0f * direction, ForceMode2D.Impulse);
    }
}
