using UnityEngine;
using EntityStateMachines;
using System;

[System.Serializable]
public class Fall : State
{
    //Impl sphere capsule cast across all states
    //Make a collisions controller class to decouple 
    //Begin Dash and side steps (mostly similar ) 

    [SerializeField]  private FallSettings data;

    [NonSerialized] public Grounded groundState;

    private float verticalVelocity;

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

    }

    public override void Update()
    {
        ApplyGravity();
        Move();

        if (IsGrounded())
            Emachine.SwitchStates(groundState);
    }

    private void ApplyGravity()
    {
        verticalVelocity += data.gravity * Time.deltaTime;

        // Clamp fall speed.
        verticalVelocity = Mathf.Min(verticalVelocity, data.maxFallSpeed);
    }

    private void Move()
    {
        Vector3 moveDir = movementInput.GetCameraRelativeInput(cam.transform);

        Vector3 horizontal = moveDir * data.airSpeed;

        Vector3 velocity = horizontal + Vector3.down * verticalVelocity;

        gameObj.transform.position += velocity * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast( gameObj.transform.position, Vector3.down, data.rayDistanceCheck, data.GroundLayer);
    }

    public void SetVerticalVelocity(float velocity)
    {
        verticalVelocity = velocity;
    }

    public override void Exit()
    {
        verticalVelocity = 0f;
    }
}