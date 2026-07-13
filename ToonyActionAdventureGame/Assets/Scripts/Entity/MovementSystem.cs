using UnityEngine;
using EntityStateMachines;
using UnityEngine.InputSystem;



public class MovementSystem : MonoBehaviour, IEntitySystem
{

    private EntityStateMachine movementFSM;
    private static IMovementInput movementInput;
    private static Camera cam;


    [Header("Movement")]
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float maxSprintSpeed;
    [SerializeField] private float Acceleration;

    public void Init()
    {
        movementFSM = new EntityStateMachine(this);
        movementInput = this.gameObject.GetComponent<IMovementInput>();
        movementFSM.SwitchStates(new Grounded(maxWalkSpeed, maxSprintSpeed, Acceleration, this.gameObject));


        cam = FindFirstObjectByType<Camera>();
    }


    public void Update()
    {
        movementFSM.currentState.HandleInput();
        movementFSM.currentState.Update();
    }


    private class Grounded : State
    {

        //idle , walk , sprint 
        private float acceleration;
        private float maxWalkVel;
        private float maxSprintVel;

        private float currentVel;
        float elapsed; 


        private bool isMoving;
        GameObject gameObject;
        CapsuleCollider capsuleCollider;




        public Grounded(float walkSpeed, float sprintSpeed, float acc, GameObject @object)
        {
            acceleration = acc;
            maxWalkVel = walkSpeed;
            maxSprintVel = sprintSpeed;
            gameObject = @object;
        }

        public override void Enter()
        {
            currentVel = 0f;
            capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        }

        public override void HandleInput()
        {
            if (movementInput.moveAction.ReadValue<Vector2>().magnitude > 0) isMoving = true;
            else isMoving = false;

        }
        public override void Update()
        {
            if (isMoving)
            {
                updateMovement(); 
                updateVelocity();
            }
            else
            {
                elapsed = 0f ; 
                currentVel = 0f;
            }
        }


        private Vector3 calculateDstVector(Vector3 startPos, Vector3 dir, float vel)
        {
            Vector3 dst = startPos + dir * vel;
            Vector3 movementVector = dst - startPos;

            float maxDistance = movementVector.magnitude;

            if (Physics.SphereCast(startPos, capsuleCollider.radius, movementVector.normalized, out RaycastHit hit, maxDistance))
            {
                //Implement Sliding
                const float OFFSET = 0.1f;
                float lambda = Mathf.Clamp01((hit.distance - OFFSET) / maxDistance);
                Vector3 newDestination = startPos + (lambda * movementVector);
                return newDestination;
            }
            return dst;
        }

        private void updateMovement()
        {
            Vector3 startPos = gameObject.transform.position;
            Vector3 dir = movementInput.GetCameraRelativeInput(cam.transform);
            Vector3 dst = calculateDstVector(startPos , dir , maxWalkVel);

            gameObject.transform.position = Vector3.MoveTowards(startPos , dst , currentVel * Time.deltaTime);
        }
        private void updateVelocity()
        {
            currentVel = Mathf.MoveTowards(currentVel, maxWalkVel , acceleration * Time.deltaTime);
            Debug.Log(currentVel);
        }

        public override void Exit() { }

    }

    private class Jump : State
    {
        //Handle Double Jumps 

        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Fall : State
    {
        //Handle fast fall 
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }
    private class SideStep : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }

    }


    private class Dash : State
    {
        // Forward , backward , and diagonal dashes 
        // Jump Dashes too
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Quick180 : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class BackFlip : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

}