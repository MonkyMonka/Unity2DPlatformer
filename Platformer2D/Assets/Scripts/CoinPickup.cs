using UnityEngine;

public class CoinPickup : Pickup
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pickupType = EPickupType.Coin;
        Animator animator = GetComponent<Animator>();
        animator.Play("CoinPickup");
        
    }
}
