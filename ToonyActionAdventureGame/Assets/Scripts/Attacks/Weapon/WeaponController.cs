using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class WeaponController  : MonoBehaviour 
{
    private int currentWeaponIndex = 0;
    [SerializeField] private LayerMask weaponLayer; 
    public Weapon[] weapons;
    public Weapon currentWeapon;

    private void Start()
    {
        weapons = GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            if (1 << (weapon.gameObject.layer & weaponLayer) == 0)
                Debug.LogError("Weapon Object is not in Weapons Layer");
        }
        currentWeapon = weapons[currentWeaponIndex];
    }

    private void Update()
    {
        if (canSwitch())
            currentWeapon  = SwitchWeapons();
    }

    private bool canSwitch()
    {
        return (Input.GetKeyDown(KeyCode.E));
    }

    private Weapon SwitchWeapons()
    {
        if (weapons.Length == 1)
        {
            currentWeaponIndex = 0;
            return weapons[currentWeaponIndex];
        }

        int startIndex = currentWeaponIndex;

        do
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= weapons.Length)
                currentWeaponIndex = 0;

        } while (weapons[currentWeaponIndex] == null && currentWeaponIndex != startIndex);

        return weapons[currentWeaponIndex];
    }
}