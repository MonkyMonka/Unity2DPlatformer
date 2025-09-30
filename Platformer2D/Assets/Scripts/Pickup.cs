using UnityEngine;

public enum EPickupType : byte
{
    Unknown,
    Mushroom
}

public enum EPickupState : byte
{
    Unknown,
    Spawning,
    Active
}

public class Pickup : MonoBehaviour
{
    public PickupSettings settings;

    protected EPickupType pickupType = EPickupType.Unknown;
    protected EPickupState pickupState = EPickupState.Unknown;
    private Vector2 start;
    private Vector2 target;
    private float spawnTime;

    public EPickupType PickupType { get { return pickupType; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pickupState == EPickupState.Spawning)
        {
            if (spawnTime > 0.0f)
            {
                spawnTime -= Time.deltaTime;

                float pct = 1.0f - (spawnTime / settings.ItemBoxSpawningDuration);
                float x = Mathf.Lerp(start.x, target.x, pct);
                float y = Mathf.Lerp(start.y, target.y, pct);

                transform.position = new Vector3(x, y, 1.0f);

                if (spawnTime <= 0.0f)
                {
                    spawnTime = 0.0f;

                    transform.position = new Vector3(target.x, target.y, 1.0f);
                    pickupState = EPickupState.Active;
                    OnPickupActive();
                }
            }
        }
    }

    public void Spawn()
    {
        pickupState = EPickupState.Spawning;
        spawnTime = settings.ItemBoxSpawningDuration;

        start = transform.position;
        target = start + new Vector2(0.0f, 1.0f);
    }

    protected virtual void OnPickupActive()
    {
        // Override this method in derived classes, if needed
    }
}
