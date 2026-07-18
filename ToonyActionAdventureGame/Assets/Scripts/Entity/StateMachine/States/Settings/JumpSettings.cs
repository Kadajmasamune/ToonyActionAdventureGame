using UnityEngine;

[System.Serializable]
public class JumpSettings
{
    public float jumpVelocity = 8f;
    public float gravity = 25f;
    public float airSpeed = 6f;

    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.45f;
}
