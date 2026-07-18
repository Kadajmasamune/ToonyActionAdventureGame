using UnityEngine;
using UnityEngine.InputSystem;
using EntityStateMachines;
using System;


[System.Serializable]
public class Jump : State
{

    //Coyote jumps Impl 

    [SerializeField] private JumpSettings data;

    [NonSerialized] public Fall fallState;

    private float verticalVelocity;


    CapsuleCollider collider;

    public Jump()
    {
        collider = (CapsuleCollider)Collider;
    }

    public override void Enter()
    {
        // Instant launch
        if (collider == null)
            collider = (CapsuleCollider)Collider;
        verticalVelocity = data.jumpVelocity;
    }

    public override void HandleInput()
    {
        // Variable jump height.
        // Releasing the button early kills some upward momentum.
        if (movementInput.jumpAction.phase == InputActionPhase.Canceled &&
            verticalVelocity > 0f)
        {
            verticalVelocity *= data.jumpCutMultiplier;
        }
    }

    public override void Update()
    {
        ApplyGravity();
        Move();

        if (verticalVelocity <= 0f)
            Emachine.SwitchStates(fallState);
    }

    private void ApplyGravity()
    {
        verticalVelocity -= data.gravity * Time.deltaTime;
    }

    private void Move()
    {
        Vector3 moveDir = movementInput.GetCameraRelativeInput(cam.transform);

        Vector3 horizontalVelocity = moveDir * data.airSpeed;
        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

        gameObj.transform.position = ResolveCollisions( gameObj.transform.position, velocity);
    }

    private Vector3 ResolveCollisions(Vector3 startPos, Vector3 velocity)
    {
        Vector3 movement = velocity * Time.deltaTime;
        float distance = movement.magnitude;

        if (distance <= 0f)
            return startPos;

        if (Physics.SphereCast( startPos, collider.radius, movement.normalized, out RaycastHit hit, distance))
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

    public override void Exit()
    {
        verticalVelocity = 0f;
    }
}