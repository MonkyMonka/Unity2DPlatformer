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
    Mushroom,
    Coin1,
    Coin5,
    Coin10
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
    private int coinCount;

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
                if (IsCoinItemBox())
                {
                    if (CoinsLeft() > 0)
                    {
                        SetState(EItemBoxState.Active);
                    }
                    else
                    {
                        SetState(EItemBoxState.Inactive);
                    }
                }
                else
                {
                    SetState(EItemBoxState.Spawning);
                }
            }
        }
        else if (state == EItemBoxState.Spawning)
        {
            SetState(EItemBoxState.Inactive);
        }
    }

    public bool IsEmpty()
    {
        if (contents == EItemBoxContents.Coin1)
        {
            return coinCount >= 1;
        }
        else if (contents == EItemBoxContents.Coin5)
        {
            return coinCount >= 5;
        }
        else if (contents == EItemBoxContents.Coin10)
        {
            return coinCount >= 10;
        }

        return state != EItemBoxState.Active;
    }
    private bool IsCoinItemBox()
    {
        return contents == EItemBoxContents.Coin1 || contents == EItemBoxContents.Coin5 || contents == EItemBoxContents.Coin10;
    }

    private int CoinsLeft()
    {
        if (contents == EItemBoxContents.Coin1)
        {
            return 1 - coinCount;
        }
        else if (contents == EItemBoxContents.Coin5)
        {
            return 5 - coinCount;
        }
        else if (contents == EItemBoxContents.Coin10)
        {
            return 10 - coinCount;
        }

        return 0;
    }

    private void SpawnPickup()
    {
        Vector2 location = transform.position;

        if (IsCoinItemBox())
        {
            Game.Instance.SpawnItemBoxCoin(location);
            coinCount++;

            Game.Instance.GetMarioState.Coins++;
        }
        else
        {
            if (contents == EItemBoxContents.Mushroom)
            {
                Game.Instance.SpawnMushroomPickup(location);
            }
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
                if (IsCoinItemBox() == false || (IsCoinItemBox() && CoinsLeft() <= 1))
                {
                    animator.Play("ItemBoxInactive");
                }

                original = transform.position;
                start = original;
                target = start + new Vector2(0.0f, 0.25f);

                animationTimer = settings.ItemBoxAnimationDuration;
            }
            else if (state == EItemBoxState.AnimDown)
            {
                if (IsCoinItemBox() && CoinsLeft() > 0)
                {
                    SpawnPickup();
                }

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
                if (IsCoinItemBox() == false)
                {
                    SpawnPickup();
                }
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
