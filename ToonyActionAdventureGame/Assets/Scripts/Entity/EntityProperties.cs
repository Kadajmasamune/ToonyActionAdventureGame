using EntityStateMachines;
using UnityEngine;


public interface ICombatHandler
{
    Vector3 AttackDirection { get; }
    bool IsLockedOn { get; }
    Transform Transform { get; }

    Attack.Context[] Context { get; }
}