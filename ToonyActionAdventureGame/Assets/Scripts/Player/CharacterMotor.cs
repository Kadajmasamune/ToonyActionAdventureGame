using EntityStateMachines;
using System;
using Unity.Profiling;
using UnityEngine; 

public class CharacterMotor : MonoBehaviour
{

    public Vector3 Velocity { get; set; }

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

    public bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    public class GroundedState : State<Player>
    {

        // Fix lerping through walls via Capsule cast and modify destination vector so player doesn't faze through walls 
        public override void Enter(Player p)
        {

            if (p.Velocity.y < 0)
                p.Velocity = new Vector3(p.Velocity.x, 0, p.Velocity.z);

        }

        public override void HandleInput(Player p)
        {
            if (p.JumpPressed)
            {
                p.entityStateMachine.SwitchStates(new JumpState());
            }

            if (p.combatStateMachine.isAttacking)
            {
                p.entityStateMachine.SwitchStates(new AttackingState());
            }
            if (p.cinCam.LockedOn)
            {
                p.entityStateMachine.SwitchStates(new LockOnState());
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
                p.entityStateMachine.SwitchStates(new FallState());

            void UpdateRotation(Player p)
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

        public override void HandleInput(Player p)
        {
            if (p.combatStateMachine.isAttackingInAir)
                p.entityStateMachine.SwitchStates(new AttackingState());
        }

        public override void Update(Player p)
        {
            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

            ApplyAirMovement(p);
            ApplyGravity(p);

            p.transform.position += p.Velocity * Time.deltaTime;

            if (p.Velocity.y <= 0)
                p.entityStateMachine.SwitchStates(new FallState());
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

        public override void HandleInput(Player p)
        {
            if (p.combatStateMachine.isAttackingInAir)
                p.entityStateMachine.SwitchStates(new AttackingState());
        }

        public override void Update(Player p)
        {
            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

            ApplyAirMovement(p);
            p.Animator.ResetTriggerJump();
            p.Velocity += Vector3.up * p.gravity * Time.deltaTime;
            p.transform.position += p.Velocity * Time.deltaTime;

            if (p.IsGrounded())
                p.entityStateMachine.SwitchStates(new GroundedState());
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
}
