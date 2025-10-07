using UnityEngine;
using UnityEngine.Tilemaps;

public enum ECameraShakeSize : sbyte
{
    Unknown = -1,
    Small,
    Medium,
    Large
}

public enum ECameraShakeDirection : sbyte
{
    Unknown = 0,
    Negative = -1,
    Positive = 1
}

public class MarioCamera : MonoBehaviour
{
    public CameraSettings settings;
    public Tilemap tilemap;

    private Vector2 initialLocation = Vector2.zero;
    private Vector2 shakeOffset = Vector2.zero;
    private float shakeTimer = 0.0f;
    private int shakeCount = 0;
    private bool isShaking = false;
    private ECameraShakeSize shakeSize = ECameraShakeSize.Unknown;
    private ECameraShakeDirection shakeDirectionX = ECameraShakeDirection.Unknown;
    private ECameraShakeDirection shakeDirectionY = ECameraShakeDirection.Unknown;

    public Vector2 ViewSize
    {
        get
        {
            float viewHeight = Camera.main.orthographicSize;
            float viewWidth = viewHeight * Camera.main.aspect;
            return new Vector2(viewWidth, viewHeight);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialLocation = transform.position;

        if (tilemap != null)
        {
            // Compress the bounds to ensure accuracy
            tilemap.CompressBounds();

            // Get the tilemap cell bounds
            BoundsInt bounds = tilemap.cellBounds;
            Debug.Log("Tilemap bounds: " + bounds.min + ", " + bounds.max);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.Instance.GetMarioState.State != EMarioState.Dead)
        {
            // Is the Camera shaking?
            if (isShaking)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0.0f)
                {
                    shakeTimer = 0.0f;
                    OnShake();
                }
            }

            // Get mario and the camera locations
            Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
            Vector2 cameraLocation = transform.position;

            // Calculate the X and Y camera location values
            cameraLocation.x = Mathf.MoveTowards(cameraLocation.x, marioLocation.x, Time.deltaTime * settings.maxDeltaMovement) + shakeOffset.x;
            cameraLocation.y = initialLocation.y + shakeOffset.y;

            // Set the camera location
            SetCameraLocation(cameraLocation);
        }
    }

    public void SetCameraLocation(Vector2 location)
    {
        Vector2 viewSize = ViewSize;
        BoundsInt levelBounds = tilemap.cellBounds;

        float maxCameraX = (float)levelBounds.max.x - viewSize.x;
        float minCameraX = (float)levelBounds.min.x + viewSize.x;
        float maxCameraY = (float)levelBounds.max.y - viewSize.y;
        float minCameraY = (float)levelBounds.min.y + viewSize.y;

        Vector3 cameraLocation = Vector3.zero;
        cameraLocation.x = Mathf.Clamp(location.x, minCameraX, maxCameraX);
        cameraLocation.y = Mathf.Clamp(location.y, minCameraY, maxCameraY);
        cameraLocation.z = transform.position.z;

        transform.position = cameraLocation;
    }

    public void ApplyCameraShake(ECameraShakeSize size = ECameraShakeSize.Medium)
    {
        // Ensure that the size parameter isn't unknown
        if (size == ECameraShakeSize.Unknown)
        {
            return;
        }

        // Ensure that the camera isn't already shaking
        if (isShaking == false)
        {
            isShaking = true;
            shakeSize = size;
            shakeTimer = settings.shakeDurations[(sbyte)shakeSize];
            shakeCount = 0;
            shakeOffset = Vector2.zero;
            shakeDirectionX = ECameraShakeDirection.Unknown;
            shakeDirectionY = ECameraShakeDirection.Unknown;
        }
    }

    private void OnShake()
    {
        // Increase the shake count
        shakeCount++;

        // Set the shake time
        shakeTimer = settings.shakeDurations[(sbyte)shakeSize];

        // Randomize the horizontal shake amount and the direction (left or right) of the shake
        shakeDirectionX = GetShakeDirection(shakeDirectionX);
        float horizontalShakeMinExtent = settings.horizontalShakeMinExtents[(sbyte)shakeSize];
        float horizontalShakeMaxExtent = settings.horizontalShakeMaxExtents[(sbyte)shakeSize];
        float shakeX = UnityEngine.Random.Range(horizontalShakeMinExtent, horizontalShakeMaxExtent);
        float signX = (sbyte)shakeDirectionX;
        shakeOffset.x = signX * shakeX;

        // Randomize the vertical shake amount and the direction (up or down) of the shake
        shakeDirectionY = GetShakeDirection(shakeDirectionY);
        float verticalShakeMinExtent = settings.verticalShakeMinExtents[(sbyte)shakeSize];
        float verticalShakeMaxExtent = settings.verticalShakeMaxExtents[(sbyte)shakeSize];
        float shakeY = UnityEngine.Random.Range(verticalShakeMinExtent, verticalShakeMaxExtent);
        float signY = (sbyte)shakeDirectionY;
        shakeOffset.y = signY * shakeY;

        // If the shake count exceeds the max shakes for this shake size, stop shaking
        int maxShakes = settings.numShakes[(sbyte)shakeSize];
        if (shakeCount >= maxShakes)
        {
            isShaking = false;
            shakeTimer = 0.0f;
            shakeCount = 0;
            shakeOffset = Vector2.zero;
            shakeDirectionX = ECameraShakeDirection.Unknown;
            shakeDirectionY = ECameraShakeDirection.Unknown;
        }
    }

    private ECameraShakeDirection GetShakeDirection(ECameraShakeDirection direction)
    {
        ECameraShakeDirection returnDirection = ECameraShakeDirection.Unknown;

        if (direction == ECameraShakeDirection.Unknown)
        {
            returnDirection = UnityEngine.Random.Range(0, 9) % 2 == 0 ? ECameraShakeDirection.Positive : ECameraShakeDirection.Negative;
        }
        else if (direction == ECameraShakeDirection.Positive)
        {
            returnDirection = ECameraShakeDirection.Negative;
        }
        else if (direction == ECameraShakeDirection.Negative)
        {
            returnDirection = ECameraShakeDirection.Positive;
        }

        return returnDirection;
    }
}
