using UnityEngine;

[CreateAssetMenu(fileName = "CameraSettings", menuName = "Mario/CameraSettings")]
public class CameraSettings : ScriptableObject
{
    [Header("Movement")]
    public float maxDeltaMovement = 32.0f;

    [Header("Shakes")]
    public int[] numShakes = { 8, 10, 12 };
    public float[] shakeDurations = { 0.02f, 0.024f, 0.0275f };

    public float[] horizontalShakeMinExtents = { 0.01f, 0.02f, 0.03f };
    public float[] horizontalShakeMaxExtents = { 0.04f, 0.05f, 0.06f };

    public float[] verticalShakeMinExtents = { 0.025f, 0.04f, 0.06f };
    public float[] verticalShakeMaxExtents = { 0.075f, 0.095f, 0.115f };
}
