using System.Collections.Generic;
using UnityEngine;


public class WeaponHitbox : MonoBehaviour
{
    [SerializeField]
    private LayerMask enemyLayer;


    private readonly List<Collider> targets = new();


    public IReadOnlyList<Collider> Targets => targets;



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("WEAPON HITBOX TRIGGERED");

        if (!IsEnemy(other))
            return;


        if (!targets.Contains(other))
        {

            Debug.Log("REGISTERED TARGET: " + other.name);
            targets.Add(other);
        }

    }



    private bool IsEnemy(Collider other)
    {
        return (enemyLayer.value &
               (1 << other.gameObject.layer)) != 0;
    }



    public void Clear()
    {
        targets.Clear();
    }
}