using System;
using UnityEngine; 


public class CollisionHandlerSystem : MonoBehaviour , IEntitySystem
{

    private CapsuleCollider Collider;

    public void Init ()
    {
        Collider = GetComponent<CapsuleCollider>();
    }


    public void Tick()
    {
        return;
    }

    public Vector3 ResolveCollisions(Vector3 destination)
    {
        Vector3 movement = destination;
        Vector3 startPos = gameObject.transform.position;

        float distance = movement.magnitude;

        if (distance <= 0f)
            return startPos;

        Vector3 center = startPos + Collider.center;

        float halfHeight = Collider.height * 0.5f;
        float offset = halfHeight - Collider.radius;

        Vector3 p1 = center + Vector3.up * offset;
        Vector3 p2 = center - Vector3.up * offset;


        if (Physics.CapsuleCast(p1 , p2 , Collider.radius, movement.normalized, out RaycastHit hit, distance))
        {
            const float skinWidth = 0.02f;

            Vector3 position = startPos + movement.normalized * Mathf.Max(hit.distance - skinWidth, 0f);
            float remainingDistance = distance - hit.distance;

            if (remainingDistance > 0f)
            {
                Vector3 remainingMove = movement.normalized * remainingDistance;
                Vector3 slide = Vector3.ProjectOnPlane(remainingMove, hit.normal);

                position += slide;
            }
            return position;
        }
        return startPos + movement;
    }

}
