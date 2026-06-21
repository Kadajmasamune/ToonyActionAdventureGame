using UnityEngine;

[CreateAssetMenu(fileName = "Create Attack", menuName = "Attacks Scriptable Object")]
public class Attack : FrameData
{
    [Header("Attack Data")]
    public float Damage;
    public float forwardMovementImpulse;
    public float speedMultiplierBonus;
    public float hitStunInflicted;
    public float hitStopDuration;
    public bool isAirAttack;
    public Attack[] AllowedAttackTransitions;

    public new FrameWindows[] FrameWindows;

    [Header("Attack Inputs")]
    public InputBufferer.AttackInput RequiredInput;

    [Header("Animation")]
    public AnimationClip clip;
    [Tooltip("Attack Index must match the index within the Animator Controller")]
    public int AttackIndexLayer;
    
}