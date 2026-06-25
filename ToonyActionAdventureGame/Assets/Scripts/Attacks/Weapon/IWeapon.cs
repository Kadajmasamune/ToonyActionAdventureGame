using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    void EnableHitbox();
    void DisableHitbox();

    IReadOnlyList<Collider> Targets { get; }
    void ClearTargets();
}