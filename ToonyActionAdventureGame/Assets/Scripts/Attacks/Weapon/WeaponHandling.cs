using UnityEngine; 
public class WeaponHandling : MonoBehaviour
{

    public LayerMask EnemyLayer;
    public Collider[] Targets;
    
    private void Awake()
    {
        Collider weaponCollider = GetComponent<Collider>();
        weaponCollider.enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & EnemyLayer) != 0)
        {
            // Implement for multiple enemies 
        }

    }
    
}