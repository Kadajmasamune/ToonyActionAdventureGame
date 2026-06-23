using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenu(fileName = "Create Weapon" , menuName = "Weapons Scriptable Object")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Attributes")]
    public string WeaponName;
    public GameObject WeaponPrefab;
    public List<Attack> attacks;

    [ContextMenu("Add Attacks")]
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
}