using UnityEngine;
using UnityEngine.InputSystem;
using EntityStateMachines;
public class Jump : State
{
    //Handle Double Jumps 
    //use CalculateDistance and make the jump feel normal



    private float jumpProgress;
    private float currentVel;
    private bool isJumping = false;

    [SerializeField] private JumpSettings data;
    public Fall fallState;

    public Jump(JumpSettings @data)
    {
        this.data = @data;

    }

    public override void Enter() { }

    public override void HandleInput()
    {
        if (movementInput.jumpAction.phase == InputActionPhase.Started)
        {
            jumpProgress = movementInput.jumpAction.GetTimeoutCompletionPercentage();
            isJumping = true;
        }
    }

    public override void Update()
    {
        if (isJumping)
        {
            updateMovement();
            updateVelocity();
        }
    }

    private float calculateDistanceToTravel()
    {
        float distance = data.maxJumpDisplacement * jumpProgress;
        if (distance >= data.maxJumpDisplacement)
            distance = data.maxJumpDisplacement;

        return distance;
    }

    private void updateVelocity()
    {
        currentVel = Mathf.MoveTowards(currentVel, data.maxJumpVelocity, data.acceleration * Time.deltaTime);
    }

    private void updateMovement()
    {
        Vector3 startPos = gameObj.transform.position;
        Vector3 dst = startPos + Vector3.up * data.maxJumpDisplacement;
        gameObj.transform.position = Vector3.MoveTowards(startPos, dst, currentVel * Time.deltaTime);

        if (startPos.y >= data.maxJumpDisplacement)
            Emachine.SwitchStates(fallState);
    }

    public override void Exit()
    {
        isJumping = false;
    }

}