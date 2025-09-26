using UnityEngine;

public enum EPickupType : byte
{
    Unknowwn,
    Mushroom
}

public class Pickup : MonoBehaviour
{
    protected EPickupType pickupType = EPickupType.Unknowwn;

    public EPickupType PickupType { get { return pickupType; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
