using EntityStateMachines;
using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AnimatorController))]
public class Player : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 2.2f;
    public float sprintSpeed = 4.8f;
    public float strafingSpeed = 5f;
    public float accel = 50f;
    public float airControl = 0.5f;

    [Header("Jump")]
    public float jumpForce = 4f;
    public float gravity = -9.8f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Rotation")]
    public float rotationSharpness = 25f;

    [Header("References")]
    public Transform cameraTransform;


    public Vector3 Velocity { get; set; }
    public Vector2 Input { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }

    public AnimatorController Animator { get; set; }

    public EntityStateMachine<Player> playerStateMachine;

    public CombatStateMachine combatStateMachine;

    public CameraControllerCinemachine cinCam;

   

    void Awake()
    {
        cinCam = FindFirstObjectByType<CameraControllerCinemachine>();
        Animator = GetComponent<AnimatorController>();
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
        
        
    }

    void Start()
    {
        playerStateMachine = new EntityStateMachine<Player>(this);
        combatStateMachine = GetComponent < CombatStateMachine>();        
        playerStateMachine.SwitchStates(new GroundedState());
    }

    void Update()
    {
        ReadInput();

        playerStateMachine.currentState.HandleInput(this);
        playerStateMachine.currentState.Update(this); 

    }

    void ReadInput()
    {
        Input = new Vector2(
            UnityEngine.Input.GetAxisRaw("Horizontal"),
            UnityEngine.Input.GetAxisRaw("Vertical")
        );

        JumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space);
        SprintHeld = UnityEngine.Input.GetKey(KeyCode.LeftShift);

        //if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
        //    inputBufferer.Buffer.Enqueue(InputBufferer.AttackInput.Light);

        //if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse1))
        //    inputBufferer.Buffer.Enqueue(InputBufferer.AttackInput.Heavy);
    

    }
    public bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    public Vector3 GetCameraRelativeInput()
    {
        Vector3 camForward = Vector3.ProjectOnPlane(
            cameraTransform.forward,
            Vector3.up).normalized;

        Vector3 camRight = Vector3.ProjectOnPlane(
            cameraTransform.right,
            Vector3.up).normalized;

        Vector3 dir = camForward * Input.y + camRight * Input.x;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        return dir;
    }

}
public class GroundedState : State<Player>
{
    public override void Enter(Player p)
    {

        if (p.Velocity.y < 0)
            p.Velocity = new Vector3(p.Velocity.x, 0, p.Velocity.z);
    }

    public override void HandleInput(Player p)
    {
        if (p.JumpPressed)
        {
            p.playerStateMachine.SwitchStates(new JumpState());
        }

        if(p.cinCam.LockedOn)
        {
            p.playerStateMachine.SwitchStates(new LockOnState());
        }
    }

    public override void Update(Player p)
    {


        Vector3 moveDir = p.GetCameraRelativeInput();

        float speed = p.SprintHeld && moveDir.sqrMagnitude > 0
            ? p.sprintSpeed
            : p.walkSpeed;

        Vector3 desiredVelocity = moveDir * speed;

        Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

        horizontal = Vector3.MoveTowards(
            horizontal,
            desiredVelocity,
            p.accel * Time.deltaTime
        );

        p.Velocity = new Vector3(horizontal.x, 0, horizontal.z);

        p.transform.position += p.Velocity * Time.deltaTime;
        UpdateRotation(p);
        p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

        if (!p.IsGrounded())
            p.playerStateMachine.SwitchStates(new FallState());

        void UpdateRotation(Player p )
        {
            Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

            if (horizontal.sqrMagnitude < 0.001f)
                return;

            Quaternion target = Quaternion.LookRotation(horizontal);
            p.transform.rotation = Quaternion.Slerp(
                p.transform.rotation,
                target,
                1f - Mathf.Exp(-p.rotationSharpness * Time.deltaTime)
            );
        }
    }

    public override void Exit(Player p)
    {
        // No persistent grounded-only flags currently exist,
        // - coyote timers
        // - grounded animation flags
        // - footstep states

        // Example future-safe cleanup:
        // p.Animator.SetGrounded(false);
    }
}

public class JumpState : State<Player>
{
    public override void Enter(Player p)
    {
        p.Velocity = new Vector3(
            p.Velocity.x,
            p.jumpForce,
            p.Velocity.z
        );

        p.Animator.TriggerJump();
    }

    public override void HandleInput(Player p) { }

    public override void Update(Player p)
    {
        p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

        ApplyAirMovement(p);
        ApplyGravity(p);

        p.transform.position += p.Velocity * Time.deltaTime;

        if (p.Velocity.y <= 0)
            p.playerStateMachine.SwitchStates(new FallState());
    }

    public override void Exit(Player p)
    {
        // Leaving jump state → ensure jump animation doesn't get stuck
        p.Animator.ResetTriggerJump();

        // Optional safety clamp (prevents weird upward carry)
        if (p.Velocity.y > 0)
        {
            p.Velocity = new Vector3(p.Velocity.x, p.Velocity.y, p.Velocity.z);
        }
    }
    void ApplyAirMovement(Player p)
    {
        Vector3 moveDir = p.GetCameraRelativeInput();

        Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

        horizontal = Vector3.MoveTowards(
            horizontal,
            moveDir * p.walkSpeed,
            p.accel * p.airControl * Time.deltaTime
        );

        p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
    }

    void ApplyGravity(Player p)
    {
        p.Velocity += Vector3.up * p.gravity * Time.deltaTime;
    }
}
public class FallState : State<Player>
{
    public override void Enter(Player p)
    {
        // Entering fall state → ensure jump trigger is cleared
        p.Animator.ResetTriggerJump();
    }

    public override void HandleInput(Player p) { }

    public override void Update(Player p)
    {
        p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

        ApplyAirMovement(p);
        p.Animator.ResetTriggerJump();
        p.Velocity += Vector3.up * p.gravity * Time.deltaTime;
        p.transform.position += p.Velocity * Time.deltaTime;

        if (p.IsGrounded())
            p.playerStateMachine.SwitchStates(new GroundedState());
    }

    void ApplyAirMovement(Player p)
    {
        Vector3 moveDir = p.GetCameraRelativeInput();

        Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

        horizontal = Vector3.MoveTowards(
            horizontal,
            moveDir * p.walkSpeed,
            p.accel * p.airControl * Time.deltaTime
        );

        p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
    }

    public override void Exit(Player p)
    {
        // Landing cleanup hook

        // Example: reset vertical velocity if needed
        if (p.Velocity.y < 0)
        {
            p.Velocity = new Vector3(p.Velocity.x, 0, p.Velocity.z);
        }

        // Optional: landing animation trigger
        // p.Animator.TriggerLand();
    }
}


public class LockOnState : State<Player>
{
    public override void Enter(Player p)
    {
        Debug.Log("LOCKON ENTERED");
    }
    public override void HandleInput(Player p)  
    {
        if (p.JumpPressed)
        {
            p.playerStateMachine.SwitchStates(new JumpState());
        }
    }

    public override void Update(Player p)
    {
        if(!p.cinCam.LockedOn)
        {
            p.playerStateMachine.SwitchStates(new GroundedState());
            return;
        }
        UpdateMovement(p);
        p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);
        UpdateRotation(p);
    }



    public override void Exit(Player p)
    {
        p.Velocity = Vector3.zero;
    }

    private void UpdateRotation(Player p)
    {
        Vector3 toEnemyDir = (p.cinCam.Enemy.Object.position - p.transform.position).normalized;

        if (toEnemyDir.sqrMagnitude < 0.001f)
            return;

        Quaternion target = Quaternion.LookRotation(toEnemyDir);
        p.transform.rotation = Quaternion.Slerp(
            p.transform.rotation,
            target,
            1f - Mathf.Exp(-p.rotationSharpness * Time.deltaTime)
        );

    }

    private void UpdateMovement(Player p)
    {
        if (!p.IsGrounded())
        {
            p.playerStateMachine.SwitchStates(new FallState());
            return;
        }

        Transform enemy = p.cinCam.Enemy.Object;

        Vector3 offset = p.transform.position - enemy.position;
        offset.y = 0f;

        if (offset.sqrMagnitude < 0.001f)
            return;

        Vector3 radial = offset.normalized;
        Vector3 tangent = new Vector3(-radial.z, 0f, radial.x);

        Vector3 moveDir = p.GetCameraRelativeInput();

        float strafe = Vector3.Dot(moveDir, tangent);
        float inOut = Vector3.Dot(moveDir, radial);

        Vector3 desiredVelocity =
            (tangent * strafe + radial * inOut) * p.strafingSpeed;

        Vector3 horizontal = new Vector3(p.Velocity.x, 0f, p.Velocity.z);

        horizontal = Vector3.MoveTowards(
            horizontal,
            desiredVelocity,
            p.accel * Time.deltaTime
        );

        p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
        p.transform.position += p.Velocity * Time.deltaTime;
    }


}

public class AttackingState : State<Player>
{
    // ONLY FOR ATTACK MOVEMENT 
    // Attack Data is handled by the combat state machine 

    private bool isGroundAttack = false;
    private bool isAirAttack = false;

    private Attack CurrentAttack = null;
    Vector3 AttackDir = Vector3.zero; 

    
    public override void Enter(Player p)
    {
        CurrentAttack = p.combatStateMachine.CurrentAttack; 
        isAirAttack = CurrentAttack.isAirAttack;
        isGroundAttack = isAirAttack ? false : true; 
    }

    public override void HandleInput(Player p) { }

    public override void Update(Player p)
    {
        if (!p.cinCam.LockedOn)
        {
            AttackDir = p.Input.normalized;
        }

        else
        {
            AttackDir = p.transform.forward;
        }


    }
    public override void Exit(Player p)
    {
        throw new NotImplementedException();
    }
}