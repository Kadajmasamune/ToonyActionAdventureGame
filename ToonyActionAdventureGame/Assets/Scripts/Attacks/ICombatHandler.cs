using EntityStateMachines;
using System.Collections.Generic;
using UnityEngine;


public interface ICombatHandler
{
    Vector3 AttackDirection { get; }

    bool IsLockedOn { get; }
    
    Weapon currentWeapon { get; }

    Attack.Context[] currentHandlerContext { get; }
    //Transform Owner { get; }

    //WeaponInstance Weapon { get; }
}

