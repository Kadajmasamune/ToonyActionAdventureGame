//using EntityStateMachines;
//using System.Collections.Generic;
//using UnityEngine; 


//public class CombatController
//{
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
//                attackEnd = calculateDstVector(p.transform.position);
//            }

//            attackEnd = calculateDstVector(p.transform.position);

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