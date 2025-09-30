using Unity.VisualScripting;
using UnityEngine;

public enum EItemBoxState : byte
{
    Unknown,
    Active,
    AnimUp,
    AnimDown,
    Spawning,
    Inactive
}

public enum EItemBoxContents : byte
{
    Mushroom

    // TODO: Add additional ItemBox contents here
}

public class ItemBox : MonoBehaviour
{
    public PickupSettings settings;
    public EItemBoxContents contents;

    private EItemBoxState state;
    private Animator animator;
    private Vector2 start;
    private Vector2 target;
    private Vector2 original;
    private float animationTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        SetState(EItemBoxState.Active);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EItemBoxState.AnimUp)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EItemBoxState.AnimDown);
            }
        }
        else if (state == EItemBoxState.AnimDown)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / settings.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EItemBoxState.Spawning);
            }
        }
        else if (state == EItemBoxState.Spawning)
        {
            SetState(EItemBoxState.Inactive);
        }
    }

    public bool IsEmpty()
    {
        return state != EItemBoxState.Active;
    }

    private void SpawnPickup()
    {
        Vector2 location = transform.position;

        if (contents == EItemBoxContents.Mushroom)
        {
            Game.Instance.SpawnMushroomPickup(location);
        }
    }

    private void SetState(EItemBoxState itemBoxState)
    {
        if (state != itemBoxState)
        {
            state = itemBoxState;

            if (state == EItemBoxState.Active)
            {
                animator.Play("ItemBoxActive");
            }
            else if (state == EItemBoxState.AnimUp)
            {
                animator.Play("ItemBoxInactive");

                original = transform.position;
                start = original;
                target = start + new Vector2(0.0f, 0.25f);

                animationTimer = settings.ItemBoxAnimationDuration;
            }
            else if (state == EItemBoxState.AnimDown)
            {
                start = target;
                target = original;

                animationTimer = settings.ItemBoxAnimationDuration;
            }
            else if (state == EItemBoxState.Spawning)
            {
                transform.position = original;
            }
            else if (state == EItemBoxState.Inactive)
            {
                SpawnPickup();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y >= 0.8f)
            {
                if (state == EItemBoxState.Active)
                {
                    SetState(EItemBoxState.AnimUp);
                }
            }
        }
    }
}
