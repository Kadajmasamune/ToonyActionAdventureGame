//using EntityStateMachines;
//using System;
//using Unity.Profiling;
//using UnityEngine; 

//public class CharacterMotor : MonoBehaviour
//{

//    public Vector3 Velocity { get; set; }

//    [Header("Speed")]
//    public float walkSpeed = 2.2f;
//    public float sprintSpeed = 4.8f;
//    public float strafingSpeed = 5f;
//    public float accel = 50f;
//    public float airControl = 0.5f;

//    [Header("Jump")]
//    public float jumpForce = 4f;
//    public float gravity = -9.8f;

//    [Header("Rotation")]
//    public float rotationSharpness = 25f;
//}

//public class GroundedState : State
//{

//    // Fix lerping through walls via Capsule cast and modify destination vector so player doesn't faze through walls 

//    private Vector3 Velocity;


//    public float groundCheckDistance = 0.2f;
//    public LayerMask groundLayer;

//    public GroundedState(Vector3 velocity)
//    {
//        this.Velocity = velocity;
//    }

//    public override void Enter()
//    {

//        if (Velocity.y < 0)
//            Velocity = new Vector3(Velocity.x, 0, Velocity.z);

//    }

//    public override void HandleInput()
//    {
//        if (!IsGrounded())
//        {
//            entityStateMachine.SwitchStates(new JumpState());
//        }

//        if (combatStateMachine.isAttacking)
//        {
//            entityStateMachine.SwitchStates(new AttackingState());
//        }
//        if (cinCam.LockedOn)
//        {
//            entityStateMachine.SwitchStates(new LockOnState());
//        }
//    }

//    public override void Update()
//    {


//        Vector3 moveDir = GetCameraRelativeInput();

//        float speed = SprintHeld && moveDir.sqrMagnitude > 0
//            ? sprintSpeed
//            : walkSpeed;

//        Vector3 desiredVelocity = moveDir * speed;

//        Vector3 horizontal = new Vector3(Velocity.x, 0, Velocity.z);

//        horizontal = Vector3.MoveTowards(
//            horizontal,
//            desiredVelocity,
//            accel * Time.deltaTime
//        );

//        Velocity = new Vector3(horizontal.x, 0, horizontal.z);

//        transform.position += Velocity * Time.deltaTime;
//        UpdateRotation(p);
//        Animator.UpdateMovement(Velocity, strafingSpeed, walkSpeed, sprintSpeed);

//        if (!IsGrounded())
//            entityStateMachine.SwitchStates(new FallState());

//        void UpdateRotation(Player p)
//        {
//            Vector3 horizontal = new Vector3(Velocity.x, 0, Velocity.z);

//            if (horizontal.sqrMagnitude < 0.001f)
//                return;

//            Quaternion target = Quaternion.LookRotation(horizontal);
//            transform.rotation = Quaternion.Slerp(
//                transform.rotation,
//                target,
//                1f - Mathf.Exp(-rotationSharpness * Time.deltaTime)
//            );
//        }
//    }

//    public override void Exit()
//    {
//        // No persistent grounded-only flags currently exist,
//        // - coyote timers
//        // - grounded animation flags
//        // - footstep states

//        // Example future-safe cleanup:
//        // Animator.SetGrounded(false);
//    }


//    public bool IsGrounded()
//    {
//        return Physics.Raycast(
//            transform.position,
//            Vector3.down,
//            groundCheckDistance,
//            groundLayer
//        );
//    }
//}




//public class JumpState : State<Player>
//{
//    public override void Enter(Player p)
//    {
//        Velocity = new Vector3(
//            Velocity.x,
//            jumpForce,
//            Velocity.z
//        );

//        Animator.TriggerJump();
//    }

//    public override void HandleInput(Player p)
//    {
//        if (combatStateMachine.isAttackingInAir)
//            entityStateMachine.SwitchStates(new AttackingState());
//    }

//    public override void Update(Player p)
//    {
//        Animator.UpdateMovement(Velocity, strafingSpeed, walkSpeed, sprintSpeed);

//        ApplyAirMovement(p);
//        ApplyGravity(p);

//        transform.position += Velocity * Time.deltaTime;

//        if (Velocity.y <= 0)
//            entityStateMachine.SwitchStates(new FallState());
//    }

//    public override void Exit(Player p)
//    {
//        // Leaving jump state → ensure jump animation doesn't get stuck
//        Animator.ResetTriggerJump();

//        // Optional safety clamp (prevents weird upward carry)
//        if (Velocity.y > 0)
//        {
//            Velocity = new Vector3(Velocity.x, Velocity.y, Velocity.z);
//        }
//    }
//    void ApplyAirMovement(Player p)
//    {
//        Vector3 moveDir = GetCameraRelativeInput();

//        Vector3 horizontal = new Vector3(Velocity.x, 0, Velocity.z);

//        horizontal = Vector3.MoveTowards(
//            horizontal,
//            moveDir * walkSpeed,
//            accel * airControl * Time.deltaTime
//        );

//        Velocity = new Vector3(horizontal.x, Velocity.y, horizontal.z);
//    }

//    void ApplyGravity(Player p)
//    {
//        Velocity += Vector3.up * gravity * Time.deltaTime;
//    }
//}



//public class FallState : State<Player>
//{
//    public override void Enter(Player p)
//    {
//        // Entering fall state → ensure jump trigger is cleared
//        Animator.ResetTriggerJump();
//    }

//    public override void HandleInput(Player p)
//    {
//        if (combatStateMachine.isAttackingInAir)
//            entityStateMachine.SwitchStates(new AttackingState());
//    }

//    public override void Update(Player p)
//    {
//        Animator.UpdateMovement(Velocity, strafingSpeed, walkSpeed, sprintSpeed);

//        ApplyAirMovement(p);
//        Animator.ResetTriggerJump();
//        Velocity += Vector3.up * gravity * Time.deltaTime;
//        transform.position += Velocity * Time.deltaTime;

//        if (IsGrounded())
//            entityStateMachine.SwitchStates(new GroundedState());
//    }

//    void ApplyAirMovement(Player p)
//    {
//        Vector3 moveDir = GetCameraRelativeInput();

//        Vector3 horizontal = new Vector3(Velocity.x, 0, Velocity.z);

//        horizontal = Vector3.MoveTowards(
//            horizontal,
//            moveDir * walkSpeed,
//            accel * airControl * Time.deltaTime
//        );

//        Velocity = new Vector3(horizontal.x, Velocity.y, horizontal.z);
//    }

//    public override void Exit(Player p)
//    {
//        // Landing cleanup hook

//        // Example: reset vertical velocity if needed
//        if (Velocity.y < 0)
//        {
//            Velocity = new Vector3(Velocity.x, 0, Velocity.z);
//        }

//        // Optional: landing animation trigger
//        // Animator.TriggerLand();
//    }
//}
