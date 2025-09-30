using UnityEngine;

[CreateAssetMenu(fileName = "PickupSettings", menuName = "Mario/PickupSettings")]
public class PickupSettings : ScriptableObject
{
    [Header("ItemBox")]
    public float ItemBoxSpawningDuration = 0.5f;
    public float ItemBoxAnimationDuration = 0.075f;

    [Header("Mushroom")]
    public float MushroomSpeed = 4.0f;
}
