using UnityEngine;

[CreateAssetMenu(fileName = "Create Attack" , menuName = "Attacks Scriptable Object")]
public class Attack : FrameData
{
    [Header("Attack Data")]
    public float Damage;
    public float forwardMovementImpulse;
    public float speedMultiplierBonus;
    public float hitStunInflicted;
    public float hitStopDuration;
    public bool isAirAttack;
    public InputBufferer.AttackInput AllowedNextAttacks;
    public InputBufferer.AttackInput RequiredInput;
}
