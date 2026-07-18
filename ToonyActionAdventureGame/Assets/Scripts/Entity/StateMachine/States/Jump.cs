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


    public override void Enter()
    {
        verticalVelocity = data.jumpVelocity;
    }

    public override void HandleInput()
    {
        // Variable jump height.
        // Releasing the button early kills some upward momentum.

        if (movementInput.jumpAction.phase == InputActionPhase.Canceled && verticalVelocity > 0f)
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
        verticalVelocity -= data.gravity * Ticker.deltaTick;
    }

    private void Move()
    {
        Vector3 moveDir = movementInput.GetCameraRelativeInput(cam.transform);

        Vector3 horizontalVelocity = moveDir * data.airSpeed;
        Vector3 velocity = horizontalVelocity + Vector3.up * verticalVelocity;

        Vector3 destination = velocity * Ticker.deltaTick;

        Vector3 resolvedVector = collisionHandler.ResolveCollisions(destination);

        gameObj.transform.position = resolvedVector;
    }


    public override void Exit()
    {
        verticalVelocity = 0f;
    }
}