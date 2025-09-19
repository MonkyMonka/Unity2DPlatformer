using UnityEngine;

[CreateAssetMenu(fileName = "PiranhaPlantSettings", menuName = "Mario/PiranhaPlantSettings")]
public class PiranhaPlantSettings : ScriptableObject
{
    public float PiranhaPlantOffsetY = 1.5f;
    public float PiranhaPlantAnimationDuration = 0.75f;
    public float PiranhaPlantActiveDuration = 2.5f;
    public float PiranhaPlantHiddenDurationMin = 2.0f;
    public float PiranhaPlantHiddenDurationMax = 4.0f;
}
