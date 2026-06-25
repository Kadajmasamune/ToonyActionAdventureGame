using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
    [SerializeField]
    private Collider weaponCollider;
    private CombatStateMachine combatStateMachine;

    private WeaponHitbox hitbox;
    public IReadOnlyList<Collider> Targets => hitbox.Targets;
    

    private void Awake()
    {
        if (!weaponCollider)
            weaponCollider = GetComponent<Collider>();

        hitbox = GetComponent<WeaponHitbox>();
        combatStateMachine = GetComponentInParent<CombatStateMachine>();
        combatStateMachine.Initialize(this);
        DisableHitbox();
    }


    public void EnableHitbox()
    {
        weaponCollider.enabled = true;
    }


    public void DisableHitbox()
    {
        weaponCollider.enabled = false;
    }


    public void ClearTargets()
    {
        hitbox.Clear();
    }
}