//using EntityStateMachines;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using Unity.Profiling;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UIElements;
//using UnityEngine.Windows;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;


//[RequireComponent(typeof(AnimatorController))]
//public class Player : MonoBehaviour, ICombatHandler
//{
//    [Header("Speed")]
//    public float walkSpeed = 2.2f;
//    public float sprintSpeed = 4.8f;
//    public float strafingSpeed = 5f;
//    public float accel = 50f;
//    public float airControl = 0.5f;

//    [Header("Jump")]
//    public float jumpForce = 4f;
//    public float gravity = -9.8f;
//    public float groundCheckDistance = 0.2f;
//    public LayerMask groundLayer;

//    [Header("Rotation")]
//    public float rotationSharpness = 25f;

//    [Header("References")]
//    public Transform cameraTransform;


//    public Vector3 Velocity { get; set; }
//    public Vector2 Input { get; private set; }
//    public bool JumpPressed { get; private set; }
//    public bool SprintHeld { get; private set; }

//    public AnimatorController Animator { get; set; }
//    public EntityStateMachine<Player> entityStateMachine { get; private set; }
//    public CombatStateMachine combatStateMachine { get; private set; }
//    public CameraControllerCinemachine cinCam;
//    private WeaponController playerWeaponControler;

//    public Vector3 AttackDirection
//    {
//        get
//        {
//            if (cinCam.LockedOn)
//                return transform.forward;

//            if (Input == Vector2.zero)
//                return transform.forward;
//            return GetCameraRelativeInput();
//        }
//    }
//    public bool IsLockedOn => cinCam.LockedOn;
//    public bool isInAir => !IsGrounded();

//    public Attack.Context[] currentHandlerContext
//    {
//        get
//        {
//            List<Attack.Context> contexts = new List<Attack.Context>();

//            if (IsLockedOn)
//                contexts.Add(Attack.Context.LockedOn);

//            if (IsGrounded())
//                contexts.Add(Attack.Context.Grounded);
//            else
//                contexts.Add(Attack.Context.InAir);

//            return contexts.ToArray();
//        }
//    }

//    public Weapon currentWeapon { get { return playerWeaponControler.currentWeapon; } }

//    void Awake()
//    {
//        cinCam = FindFirstObjectByType<CameraControllerCinemachine>();
//        Animator = GetComponent<AnimatorController>();
//        if (!cameraTransform)
//            cameraTransform = Camera.main.transform;
//        playerWeaponControler = GetComponent<WeaponController>();
//        combatStateMachine = GetComponent<CombatStateMachine>();
//        combatStateMachine.Initialize(this);
//    }

//    void Start()
//    {
//        entityStateMachine = new EntityStateMachine<Player>(this);
//        entityStateMachine.SwitchStates(new GroundedState());
//    }

//    void Update()
//    {
//        ReadInput();

//        entityStateMachine.currentState.HandleInput(this);
//        entityStateMachine.currentState.Update(this);
//        //playerStateMachine.currentState.Exit(this);
//    }

//    void ReadInput()
//    {
//        Input = new Vector2(
//            UnityEngine.Input.GetAxisRaw("Horizontal"),
//            UnityEngine.Input.GetAxisRaw("Vertical")
//        );

//        JumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space);
//        SprintHeld = UnityEngine.Input.GetKey(KeyCode.LeftControl);

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

//    public Vector3 GetCameraRelativeInput()
//    {
//        Vector3 camForward = Vector3.ProjectOnPlane(
//            cameraTransform.forward,
//            Vector3.up).normalized;

//        Vector3 camRight = Vector3.ProjectOnPlane(
//            cameraTransform.right,
//            Vector3.up).normalized;

//        Vector3 dir = camForward * Input.y + camRight * Input.x;

//        if (dir.sqrMagnitude > 1f)
//            dir.Normalize();

//        return dir;
//    }
//    public class GroundedState : State<Player>
//    {

//        // Fix lerping through walls via Capsule cast and modify destination vector so player doesn't faze through walls 
//        public override void Enter(Player p)
//        {

//            if (p.Velocity.y < 0)
//                p.Velocity = new Vector3(p.Velocity.x, 0, p.Velocity.z);
//        }

//        public override void HandleInput(Player p)
//        {
//            if (p.JumpPressed)
//            {
//                p.entityStateMachine.SwitchStates(new JumpState());
//            }

//            if (p.combatStateMachine.isAttacking)
//            {
//                p.entityStateMachine.SwitchStates(new AttackingState());
//            }
//            if (p.cinCam.LockedOn)
//            {
//                p.entityStateMachine.SwitchStates(new LockOnState());
//            }
//        }

//        public override void Update(Player p)
//        {


//            Vector3 moveDir = p.GetCameraRelativeInput();

//            float speed = p.SprintHeld && moveDir.sqrMagnitude > 0
//                ? p.sprintSpeed
//                : p.walkSpeed;

//            Vector3 desiredVelocity = moveDir * speed;

//            Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

//            horizontal = Vector3.MoveTowards(
//                horizontal,
//                desiredVelocity,
//                p.accel * Time.deltaTime
//            );

//            p.Velocity = new Vector3(horizontal.x, 0, horizontal.z);

//            p.transform.position += p.Velocity * Time.deltaTime;
//            UpdateRotation(p);
//            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

//            if (!p.IsGrounded())
//                p.entityStateMachine.SwitchStates(new FallState());

//            void UpdateRotation(Player p)
//            {
//                Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

//                if (horizontal.sqrMagnitude < 0.001f)
//                    return;

//                Quaternion target = Quaternion.LookRotation(horizontal);
//                p.transform.rotation = Quaternion.Slerp(
//                    p.transform.rotation,
//                    target,
//                    1f - Mathf.Exp(-p.rotationSharpness * Time.deltaTime)
//                );
//            }
//        }

//        public override void Exit(Player p)
//        {
//            // No persistent grounded-only flags currently exist,
//            // - coyote timers
//            // - grounded animation flags
//            // - footstep states

//            // Example future-safe cleanup:
//            // p.Animator.SetGrounded(false);
//        }
//    }

//    public class JumpState : State<Player>
//    {
//        public override void Enter(Player p)
//        {
//            p.Velocity = new Vector3(
//                p.Velocity.x,
//                p.jumpForce,
//                p.Velocity.z
//            );

//            p.Animator.TriggerJump();
//        }

//        public override void HandleInput(Player p) 
//        {
//            if (p.combatStateMachine.isAttackingInAir)
//                p.entityStateMachine.SwitchStates(new AttackingState());
//        }

//        public override void Update(Player p)
//        {
//            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

//            ApplyAirMovement(p);
//            ApplyGravity(p);

//            p.transform.position += p.Velocity * Time.deltaTime;

//            if (p.Velocity.y <= 0)
//                p.entityStateMachine.SwitchStates(new FallState());
//        }

//        public override void Exit(Player p)
//        {
//            // Leaving jump state → ensure jump animation doesn't get stuck
//            p.Animator.ResetTriggerJump();

//            // Optional safety clamp (prevents weird upward carry)
//            if (p.Velocity.y > 0)
//            {
//                p.Velocity = new Vector3(p.Velocity.x, p.Velocity.y, p.Velocity.z);
//            }
//        }
//        void ApplyAirMovement(Player p)
//        {
//            Vector3 moveDir = p.GetCameraRelativeInput();

//            Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

//            horizontal = Vector3.MoveTowards(
//                horizontal,
//                moveDir * p.walkSpeed,
//                p.accel * p.airControl * Time.deltaTime
//            );

//            p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
//        }

//        void ApplyGravity(Player p)
//        {
//            p.Velocity += Vector3.up * p.gravity * Time.deltaTime;
//        }
//    }
//    public class FallState : State<Player>
//    {
//        public override void Enter(Player p)
//        {
//            // Entering fall state → ensure jump trigger is cleared
//            p.Animator.ResetTriggerJump();
//        }

//        public override void HandleInput(Player p)
//        {
//            if (p.combatStateMachine.isAttackingInAir)
//                p.entityStateMachine.SwitchStates(new AttackingState());
//        }

//        public override void Update(Player p)
//        {
//            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);

//            ApplyAirMovement(p);
//            p.Animator.ResetTriggerJump();
//            p.Velocity += Vector3.up * p.gravity * Time.deltaTime;
//            p.transform.position += p.Velocity * Time.deltaTime;

//            if (p.IsGrounded())
//                p.entityStateMachine.SwitchStates(new GroundedState());
//        }

//        void ApplyAirMovement(Player p)
//        {
//            Vector3 moveDir = p.GetCameraRelativeInput();

//            Vector3 horizontal = new Vector3(p.Velocity.x, 0, p.Velocity.z);

//            horizontal = Vector3.MoveTowards(
//                horizontal,
//                moveDir * p.walkSpeed,
//                p.accel * p.airControl * Time.deltaTime
//            );

//            p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
//        }

//        public override void Exit(Player p)
//        {
//            // Landing cleanup hook

//            // Example: reset vertical velocity if needed
//            if (p.Velocity.y < 0)
//            {
//                p.Velocity = new Vector3(p.Velocity.x, 0, p.Velocity.z);
//            }

//            // Optional: landing animation trigger
//            // p.Animator.TriggerLand();
//        }
//    }


//    public class LockOnState : State<Player>
//    {
//        public override void Enter(Player p)
//        {
//            Debug.Log("LOCKON ENTERED");
//        }
//        public override void HandleInput(Player p)
//        {
//            if (p.JumpPressed)
//            {
//                p.entityStateMachine.SwitchStates(new JumpState());
//            }

//            if (p.combatStateMachine.isAttacking &&
//     p.combatStateMachine.CurrentAttack != null)
//            {
//                p.entityStateMachine.SwitchStates(new AttackingState());
//            }
//        }

//        public override void Update(Player p)
//        {
//            if (!p.cinCam.LockedOn)
//            {
//                p.entityStateMachine.SwitchStates(new GroundedState());
//                return;
//            }
//            UpdateMovement(p);
//            p.Animator.UpdateMovement(p.Velocity, p.strafingSpeed, p.walkSpeed, p.sprintSpeed);
//            UpdateRotation(p);
//        }



//        public override void Exit(Player p)
//        {
//            p.Velocity = Vector3.zero;
//        }

//        private void UpdateRotation(Player p)
//        {
//            Vector3 toEnemyDir = (p.cinCam.Enemy.Object.position - p.transform.position).normalized;

//            if (toEnemyDir.sqrMagnitude < 0.001f)
//                return;

//            Quaternion target = Quaternion.LookRotation(toEnemyDir);
//            Quaternion lookAtTarget = new Quaternion(target.x, 0, target.z, target.w);
//            p.transform.rotation = Quaternion.Slerp(
//                p.transform.rotation,
//                target,
//                1f - Mathf.Exp(-p.rotationSharpness * Time.deltaTime)
//            );

//        }

//        private void UpdateMovement(Player p)
//        {
//            if (!p.IsGrounded())
//            {
//                p.entityStateMachine.SwitchStates(new FallState());
//                return;
//            }

//            Transform enemy = p.cinCam.Enemy.Object;

//            Vector3 offset = p.transform.position - enemy.position;
//            offset.y = 0f;

//            if (offset.sqrMagnitude < 0.001f)
//                return;

//            Vector3 radial = offset.normalized;
//            Vector3 tangent = new Vector3(-radial.z, 0f, radial.x);

//            Vector3 moveDir = p.GetCameraRelativeInput();

//            float strafe = Vector3.Dot(moveDir, tangent);
//            float inOut = Vector3.Dot(moveDir, radial);

//            Vector3 desiredVelocity =
//                (tangent * strafe + radial * inOut) * p.strafingSpeed;

//            Vector3 horizontal = new Vector3(p.Velocity.x, 0f, p.Velocity.z);

//            horizontal = Vector3.MoveTowards(
//                horizontal,
//                desiredVelocity,
//                p.accel * Time.deltaTime
//            );

//            p.Velocity = new Vector3(horizontal.x, p.Velocity.y, horizontal.z);
//            p.transform.position += p.Velocity * Time.deltaTime;
//        }


//    }

//    public class AttackingState : State<Player>
//    {
       
//        private Attack CurrentAttack;

//        private Vector3 AttackDir;

//        private float attackTimer;
//        private Vector3 attackStart;
//        private Vector3 attackEnd;

//        public override void Enter(Player p)
//        {
//            InitializeAttack(p);
//        }


//        public override void Update(Player p)
//        {
//            int ActiveEnd = CurrentAttack.StartUpFrames + CurrentAttack.ActiveFrames;

//            if (p.combatStateMachine.isTransitioning)
//            {
//                InitializeAttack(p);
//                attackEnd = calculateDstVector(p.transform.position );
//            }
    
//            attackEnd = calculateDstVector(p.transform.position );

//            if (p.combatStateMachine.CurrentAttackTick <= ActiveEnd)
//            {
//                MovePlayer(p);
//                UpdateRotation(p);

//            }
//            if (p.combatStateMachine.isAttackingInAir)
//            {
//                ApplyGravity(p);
//            }
//        }

//        public override void HandleInput(Player p)
//        {
//            if (!p.combatStateMachine.isAttacking) p.entityStateMachine.SwitchStates(new GroundedState());
//        }

//        public void InitializeAttack(Player p)
//        {
//            CurrentAttack = p.combatStateMachine.CurrentAttack;

//            AttackDir = p.cinCam.LockedOn
//                ? p.transform.forward
//                : p.GetCameraRelativeInput();


//            if (AttackDir == Vector3.zero)
//                AttackDir = p.transform.forward;

//            p.Velocity = AttackDir;

//            attackTimer = 0;

//            attackStart = p.transform.position;
//            attackEnd = calculateDstVector(attackStart);
//        }



//        private void MovePlayer(Player p)
//        {
//            attackTimer += Time.deltaTime;


//            float t =
//                attackTimer /
//                CurrentAttack.attackSpeed;


//            float easedT =
//                CurrentAttack.lungeCurve.Evaluate(t);



//            Vector3 attackPosition =
//                Vector3.Lerp(
//                    attackStart,
//                    attackEnd,
//                    easedT);



//            // keep vertical physics
//            attackPosition.y =
//                p.transform.position.y;


//            p.transform.position =
//                attackPosition;
//        }

//        private void ApplyGravity(Player p)
//        {
//            p.Velocity +=
//                Vector3.down *
//                p.gravity *
//                Time.deltaTime;


//            Vector3 pos = p.transform.position;


//            // preserve attack movement height
//            pos.y -= p.Velocity.y * Time.deltaTime;


//            p.transform.position = pos;
//        }


//        private Vector3 calculateDstVector(Vector3 startPos)
//        {
//            Vector3 dst = startPos + AttackDir * CurrentAttack.forwardMovementImpulse;
//            Vector3 movementVector = dst - startPos;

//            float maxDistance = movementVector.magnitude;

//            if (Physics.Raycast(startPos, movementVector.normalized, out RaycastHit hit, maxDistance))
//            {
//                const float OFFSET = 0.1f;
//                float lambda = Mathf.Clamp01((hit.distance - OFFSET) / maxDistance);
//                Vector3 newDestination = startPos + (lambda * movementVector);
//                return newDestination;
//            }
//            return dst;
//        }
//        void UpdateRotation(Player p)
//        {

//            if (AttackDir.sqrMagnitude < 0.001f)
//                return;

//            Quaternion target = Quaternion.LookRotation(AttackDir);

//            p.transform.rotation = Quaternion.Slerp(
//                p.transform.rotation,
//                target,
//                1f - Mathf.Exp(-p.rotationSharpness * Time.deltaTime)
//            );
//        }

//        public override void Exit(Player p)
//        {

//        }
//    }
//}
