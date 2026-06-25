using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Attack", menuName = "Attacks Scriptable Object")]
public class Attack : FrameData
{
    public enum Context
    {
        LockedOn , 
        Grounded , 
        InAir , 
    }

    [Header("Attack Data")]
    public float Damage;
    public float attackSpeed;
    public float forwardMovementImpulse;
    public float upwardLaunchImpulse; 
    public float hitStunInflicted;
    public float knockBackInflicted;
    public float hitStopDuration;
    public float cameraShakeIntensity;
    public Vector3 DirectionRequired;
    public Context[] contextRequired;
    public Attack[] AllowedAttackTransitions;
    public AnimationCurve lungeCurve;

    public new FrameWindows[] FrameWindows;

    [Header("Attack Inputs")]
    public InputBufferer.AttackInput RequiredInput;

    [Header("Animation")]
    public AnimationClip clip;
    [Tooltip("Attack Index must match the index within the Animator Controller")]
    public int AttackIndexLayer;
   
}