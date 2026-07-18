
using UnityEngine;

[System.Serializable]
public class FallSettings
{
    public float gravity = 30f;
    public float maxFallSpeed = 25f;
    public float airSpeed = 6f;

    public float rayDistanceCheck = 0.25f;
    public LayerMask GroundLayer;
}