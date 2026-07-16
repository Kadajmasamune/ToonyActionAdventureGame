using UnityEngine;
using EntityStateMachines;
public class Grounded : State
{
    //Implement Sliding while colliding against surfaces


    private bool isSprinting => movementInput.sprintAction.IsPressed();
    private bool isMoving;


    private float currentVel;
    private float analogProgression;

    [SerializeField] GroundedSettings data;


    private CapsuleCollider collider;
    public Jump jumpState;


    public Grounded(GroundedSettings @data)
    {
        this.data = @data;
        collider = (CapsuleCollider)Collider;
    }


    public override void Enter()
    {
        currentVel = 0f;
        collider = (CapsuleCollider)Collider;
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
            Vector3 dst = calculateDstVector(startPos, dir, data.maxVelocity);

            updateRotation(startPos, dst);
            updateMovement(startPos, dst);
            resolveCollisions(dst.magnitude , dir);
            updateVelocity();
        }
        else
        {
            currentVel = 0f;
        }
    }

    private Vector3 calculateDstVector(Vector3 startPos, Vector3 dir, float vel)
    {
        Vector3 dst = startPos + dir * vel;
        Vector3 movementVector = dst - startPos;

        float maxDistance = movementVector.magnitude;

        if (Physics.SphereCast(startPos, collider.radius, movementVector.normalized, out RaycastHit hit, maxDistance))
        {   
            const float OFFSET = 0.1f;
            float lambda = Mathf.Clamp01((hit.distance - OFFSET) / maxDistance);
            Vector3 newDestination = startPos + (lambda * movementVector);
            return newDestination;
        }
     
        return dst;
    }
    
    private void resolveCollisions(float maxDistance , Vector3 dir)
    {        
        if (Physics.SphereCast(gameObj.transform.position, collider.radius , dir , out RaycastHit hit , maxDistance))
        {
            Vector3 moveDir = movementInput.moveAction.ReadValue<Vector2>();
            Debug.Log(moveDir);
        }
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

