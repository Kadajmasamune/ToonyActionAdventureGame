using EntityStateMachines;
using System;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Grounded : State
{

    [Header("Data")]
    [SerializeField] GroundedSettings data;
    private bool isSprinting => movementInput.sprintAction.IsPressed();
    private bool isMoving;
    private float currentVel;
    private float analogProgression;


    [Header("Dash config")]    
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

            rotationInfo.startpos = startPos; rotationInfo.dst = dst;

            updateMovement(startPos, dst);            
            updateVelocity();

            if (can180(startPos, dst))
                Emachine.SwitchState<Quick180>();                
            


            Debug.Log($"Player's current velocity : {currentVel}");
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
    
    private bool can180(Vector3 startPos , Vector3 dst)
    {
        Vector3 movementDirection = (dst - startPos).normalized;
        float angle = Vector3.Angle(gameObj.transform.forward, movementDirection);
        return angle >= 150;
    }

    public override void Exit()
    {

    }

}

