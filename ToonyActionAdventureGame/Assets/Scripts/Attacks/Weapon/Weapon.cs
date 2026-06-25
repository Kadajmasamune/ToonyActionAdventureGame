using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Weapon" , menuName = "Weapons Scriptable Object")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Attributes")]
    public string WeaponName;
    public List<Attack> attacks;
    public GameObject weaponObj;
    public WeaponHandling collisionHandlingScript;



    [ContextMenu("Add Attacks")]
    [Tooltip("BE CAERFUL OF SPACES")]
    public void AddAttacks ()
    {
        if(attacks.Count > 0)
            attacks.Clear();

        Object[] AttacksScriptableObjects = Resources.LoadAll($"Weapons/{WeaponName}" , typeof(Attack));
        foreach (Attack attack in AttacksScriptableObjects)
        {
            //Debug.Log(attack.name + " " + attack.GetType());
            attacks.Add(attack);
        }
    }

    public void SetWeapon(GameObject obj)
    {
        weaponObj = obj;
        collisionHandlingScript = weaponObj.GetComponent<WeaponHandling>();
    }
        
}

