using UnityEngine;
using EntityStateMachines;
using System;

[System.Serializable]
public class Grounded : State
{
    //jump dash

    private bool isSprinting => movementInput.sprintAction.IsPressed();
    private bool isMoving;


    private float currentVel;
    private float analogProgression;

    [SerializeField] GroundedSettings data;
    
    private int nextDashTick = 0;
    private bool canDash => Ticker.currentTick >= nextDashTick;

    public override void Enter()
    {
        
        currentVel = 0f;
        
    }

    public override void HandleInput()
    {
    
        
        if (movementInput.moveAction.ReadValue<Vector2>().magnitude > 0)
        {
            isMoving = true;
            analogProgression = movementInput.moveAction.GetControlMagnitude();

        }
        else isMoving = false;


        if (movementInput.jumpAction.IsPressed())
            Emachine.SwitchState<Jump>() ;

        if (movementInput.dashAction.IsInProgress() && canDash)
        {
            Emachine.SwitchState<Dash>();
            nextDashTick = Ticker.currentTick + 60;
        }

        //Debug.Log($"Analog Stick Movement Progression : {analogProgression}");

    }

   
    public override void Update()
    {
        if (isMoving)
        {
            Vector3 startPos = gameObj.transform.position;
            Vector3 dir = movementInput.GetCameraRelativeInput(cam.transform);
            Vector3 dst = dir * currentVel * Ticker.deltaTick;

            dst = collisionHandler.ResolveCollisions(dst);

            updateRotation(startPos, dst);
            updateMovement(startPos, dst);

            updateVelocity();
        }
        else
        {
            currentVel = 0f;
        }
    }

 
    private void updateMovement(Vector3 startPos, Vector3 dst)
    {
        gameObj.transform.position = Vector3.MoveTowards(startPos, dst, currentVel * Ticker.deltaTick);
    }
    private void updateVelocity()
    {
        
        float targetVel = isSprinting ? data.maxSprintVelocity : data.maxVelocity;
        currentVel = Mathf.MoveTowards(currentVel, targetVel * analogProgression, data.acceleration * Ticker.deltaTick);


        // Debug.Log(currentVel);
    }


    private void updateRotation(Vector3 startPos, Vector3 dst)
    {
        Vector3 movementDirection = (dst - startPos).normalized;
        //Debug.Log($"Movement Vector : {movementDirection}");

        if (movementDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        //Debug.Log($"Target rot : {targetRotation.eulerAngles}");


        gameObj.transform.rotation = Quaternion.RotateTowards(
            gameObj.transform.rotation,
            targetRotation,
            1f - Mathf.Exp(50f * Ticker.deltaTick)
        );
    }

    public override void Exit()
    {

    }

}

