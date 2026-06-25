//using System.Collections.Generic;
//using UnityEngine;
//public class WeaponController : MonoBehaviour , IWeapon
//{
//    private CombatStateMachine combatStateMachine;
//    private int currentWeaponIndex = 0;

//    public Weapon[] weapons;
//    private Weapon currentWeapon; 

//    public List<Collider> Targets { get; }


//    private void Start()
//    {
//        combatStateMachine = GetComponentInParent<CombatStateMachine>();
//        combatStateMachine.Initialize(this);
//    }

//    private void Update()
//    {
//        if (canSwitch() && Input.GetKeyDown(KeyCode.E))
//        {
//            currentWeapon = switchWeapons();
//        }

//        if (canDisable())
//            currentWeapon.DisableCollider();

//        if (canEnable())
//            currentWeapon.EnableCollider();
//    }

//    private bool canSwitch()
//    {
//        return !combatStateMachine.isAttacking;
//    }

//    private bool canEnable()
//    {
//        Attack attack = combatStateMachine.CurrentAttack;
//        int tick = combatStateMachine.CurrentAttackTick;
//        int ActiveEnd = attack.StartUpFrames + attack.ActiveFrames;

//        if (tick >= attack.StartUpFrames && tick <= ActiveEnd)
//            return true;

//        return false;
//    }

//    private bool canDisable()
//    {
//        Attack attack = combatStateMachine.CurrentAttack;
//        int tick = combatStateMachine.CurrentAttackTick;
//        int startUpEnd = attack.StartUpFrames;
//        int ActiveEnd = attack.StartUpFrames + attack.ActiveFrames;
//        int TotalFrames = attack.StartUpFrames + attack.ActiveFrames+ attack.RecoveryFrames;

//        if (tick <= startUpEnd || tick >= ActiveEnd && tick <= TotalFrames)
//            return true;

//        return false;
//    }

//    private Weapon switchWeapons()
//    {
//        if (weapons.Length == 1)
//            return weapons[currentWeaponIndex];

//        for (int i = 0; i < weapons.Length; i++)
//        {
//            if (weapons[i + 1] != null)
//                currentWeaponIndex++;

//            else
//                currentWeaponIndex = 0;
//        }

//        return weapons[currentWeaponIndex];
//    }

//}