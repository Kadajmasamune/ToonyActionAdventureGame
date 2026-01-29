using UnityEngine;

[RequireComponent(typeof(AnimatorController))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 2.2f;
    [SerializeField] private float sprintSpeed = 4.8f;

    [Header("Acceleration")]
    [SerializeField] private float accel = 50f;

    [Header("Rotation")]
    [SerializeField] private float rotationSharpness = 25f;
    [SerializeField] private float turnInPlaceThreshold = 0.2f;

    [SerializeField] private Transform cameraTransform;

    private AnimatorController animator;
    private Vector3 velocity;
    private Vector3 desiredVelocity;

    private float inputYaw;

    private void Awake()
    {
        animator = GetComponent<AnimatorController>();
        if (!cameraTransform) cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        ReadInput();
        UpdateVelocity(dt);
        ApplyMovement(dt);
        UpdateRotation(dt);
        UpdateAnimator();
    }

    private void ReadInput()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        float magnitude = Mathf.Clamp01(input.magnitude);

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 moveDir = camForward * input.y + camRight * input.x;

        if (moveDir.sqrMagnitude > 0f)
            moveDir.Normalize();

        bool sprinting = magnitude > 0f && Input.GetKey(KeyCode.LeftShift);
        float speed = sprinting ? sprintSpeed : walkSpeed;

        desiredVelocity = moveDir * speed * magnitude;

        if (magnitude < 0.01f)
        {
            inputYaw = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            inputYaw = 0f;
        }
    }

    private void UpdateVelocity(float dt)
    {
        velocity = Vector3.MoveTowards(
            velocity,
            desiredVelocity,
            accel * dt
        );

        if (desiredVelocity.sqrMagnitude < 0.0001f)
            velocity = Vector3.zero;
    }

    private void ApplyMovement(float dt)
    {
        transform.position += velocity * dt;
    }

    private void UpdateRotation(float dt)
    {
        if (velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                1f - Mathf.Exp(-rotationSharpness * dt)
            );
            return;
        }

        if (Mathf.Abs(inputYaw) > turnInPlaceThreshold)
        {
            float turnAngle = inputYaw * 180f * dt;
            transform.rotation *= Quaternion.Euler(0f, turnAngle, 0f);
        }
    }

    private void UpdateAnimator()
    {
        Vector3 localVel = transform.InverseTransformDirection(velocity);

        float x = Mathf.Abs(localVel.x) < 0.01f ? 0f : localVel.x / sprintSpeed;
        x = Mathf.Clamp(x, -1f, 1f);

        float y = 0f;
        if (localVel.z > 0.01f)
        {
            if (localVel.z <= walkSpeed)
            {
                y = (localVel.z / walkSpeed) * 0.5f;
            }
            else
            {
                float runPercent = (localVel.z - walkSpeed) / (sprintSpeed - walkSpeed);
                runPercent = Mathf.Clamp01(runPercent);
                y = 0.5f + runPercent * 0.5f;
            }
        }

        animator.moveX = x;
        animator.moveY = y;
    }


}
