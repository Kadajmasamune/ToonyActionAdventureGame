using UnityEngine;
using EntityStateMachines;
using System;

[System.Serializable]
public class Fall : State
{ 

    [SerializeField]  private FallSettings data;

    private float verticalVelocity;


    private int nextDashTick = 0;
    private bool canDash => Ticker.currentTick >= nextDashTick;


    public Fall()
    {
        
    }

    public override void Enter()
    {
        // If Jump passes us its velocity, keep it.
        // Otherwise start at zero.
    }

    public override void HandleInput()
    {
        if (movementInput.dashAction.IsInProgress() && canDash)
        {
            Emachine.SwitchState<Dash>();
            nextDashTick = Ticker.currentTick + 60;
        }
    }

    public override void Update()
    {
        ApplyGravity();
        Move();

        if (IsGrounded())
            Emachine.SwitchState<Grounded>();
    }

    private void ApplyGravity()
    {
        verticalVelocity += data.gravity * Ticker.deltaTick;

        // Clamp fall speed.
        verticalVelocity = Mathf.Min(verticalVelocity, data.maxFallSpeed);
    }

    private void Move()
    {
        Vector3 moveDir = movementInput.GetCameraRelativeInput(cam.transform);

        Vector3 horizontal = moveDir * data.airSpeed;

        Vector3 velocity = horizontal + Vector3.down * verticalVelocity;


        Vector3 destination = velocity * Ticker.deltaTick;

        Vector3 resolvedVector = collisionHandler.ResolveCollisions(destination);

        gameObj.transform.position = resolvedVector;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast( gameObj.transform.position, Vector3.down, data.rayDistanceCheck, data.GroundLayer);
    }

    public override void Exit()
    {
        verticalVelocity = 0f;
    }
}