using UnityEngine;
using System.Collections.Generic;
public class WeaponHandling : MonoBehaviour
{

    public LayerMask EnemyLayer;
    public List<Collider>Targets;
    
    private void Awake()
    {
        Collider weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & EnemyLayer) != 0)
        {
            Targets.Add(other);
        }
    }

    private void Update()
    {
        //if (Targets.Count > 0)
        //{
        //    foreach (Collider target in Targets)
        //    {
        //        Debug.Log(target.gameObject.name, target);
        //    }
        //}

        Debug.Log(Targets.Count);
    }

}