using EntityStateMachines;
using System;
using UnityEngine;


[System.Serializable]
public class Dash : State
{
    [SerializeField] private DashSettings data;


    private Vector3 LockedDir = Vector3.zero;
    Vector3 targetPos = Vector3.zero;
    
    private float currentVel;
    private bool dashComplete = false;

    public override void Enter() 
    {
        currentVel = data.dashVelocity;
        LockedDir = movementInput.GetCameraRelativeInput(cam.transform);
        targetPos = collisionHandler.ResolveCollisions((LockedDir * data.maxDashDisplacement));

    }

    public override void HandleInput() { }

    public override void Update() 
    {
        updateMovement();

        if (dashComplete)
        {
            if(Emachine.lastState == typeof(Grounded))
                Emachine.SwitchState<Grounded>();

            else if (Emachine.lastState == typeof(Jump))
                Emachine.SwitchState<Jump>();

            else if(Emachine.lastState == typeof(Fall))
                Emachine.SwitchState<Fall>();
        }

    }

    private void updateMovement()
    {
        gameObj.transform.position = Vector3.MoveTowards(gameObj.transform.position, targetPos, currentVel * Ticker.deltaTick);

        if (Vector3.Distance(gameObj.transform.position , targetPos) <  0.01f)
            dashComplete = true;

    }

    public override void Exit() 
    {
        dashComplete = false;
        currentVel = 0f;
        LockedDir = Vector3.zero;
        targetPos = Vector3.zero;
    }
}
