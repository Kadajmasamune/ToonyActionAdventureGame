using UnityEngine;
using EntityStateMachines;
using System;

[System.Serializable]
public class Grounded : State
{
    //Implement Sliding while colliding against surfaces


    private bool isSprinting => movementInput.sprintAction.IsPressed();
    private bool isMoving;


    private float currentVel;
    private float analogProgression;

    [SerializeField] GroundedSettings data;


    private CapsuleCollider collider;
    [NonSerialized] public Jump jumpState;


    public Grounded()
    {
        // this.data = @data;
        collider = (CapsuleCollider)Collider;
    }


    public override void Enter()
    {
        if(collider == null)
            collider = (CapsuleCollider)Collider;
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
            Emachine.SwitchStates(jumpState);

        //Debug.Log($"Analog Stick Movement Progression : {analogProgression}");

    }

   
    public override void Update()
    {
        if (isMoving)
        {
            Vector3 startPos = gameObj.transform.position;
            Vector3 dir = movementInput.GetCameraRelativeInput(cam.transform);
            Vector3 dst = resolveCollisions(startPos, dir);

            updateRotation(startPos, dst);
            updateMovement(startPos, dst);

            updateVelocity();
        }
        else
        {
            currentVel = 0f;
        }
    }

    private Vector3 resolveCollisions(Vector3 startPos, Vector3 dir)
    {
        Vector3 movement = dir * currentVel * Time.deltaTime;
        float distance = movement.magnitude;

        if (distance <= 0f)
            return startPos;

        if (Physics.SphereCast(startPos, collider.radius, movement.normalized, out RaycastHit hit, distance))
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


    private void updateMovement(Vector3 startPos, Vector3 dst)
    {
        gameObj.transform.position = Vector3.MoveTowards(startPos, dst, currentVel * Time.deltaTime);
    }
    private void updateVelocity()
    {
        
        float targetVel = isSprinting ? data.maxSprintVelocity : data.maxVelocity;
        currentVel = Mathf.MoveTowards(currentVel, targetVel * analogProgression, data.acceleration * Time.deltaTime);
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
            1f - Mathf.Exp(50f * Time.deltaTime)
        );
    }

    public override void Exit()
    {

    }

}

