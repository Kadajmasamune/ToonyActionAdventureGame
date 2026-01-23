using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Orbit")]
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float verticalPivotOffset = 1.6f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 50f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float collisionPadding = 0.2f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmoothTime = 0.05f;

    private float yaw;
    private float pitch;

    private Vector3 positionVelocity;
    private Vector3 rotationVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleInput();
    }

    private void LateUpdate()
    {
        Vector3 pivot = GetPivot();
        Vector3 desiredPos = CalculateOrbitPosition(pivot);
        Vector3 finalPos = ResolveCameraCollision(pivot, desiredPos);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPos,
            ref positionVelocity,
            positionSmoothTime
        );

        Vector3 desiredDir = (pivot - transform.position).normalized;

        Vector3 smoothDir = Vector3.SmoothDamp(
            transform.forward,
            desiredDir,
            ref rotationVelocity,
            rotationSmoothTime
        ).normalized;

        transform.rotation = Quaternion.LookRotation(smoothDir, Vector3.up);
    }

    // =========================
    // INPUT
    // =========================
    private void HandleInput()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    // =========================
    // PIVOT
    // =========================
    private Vector3 GetPivot()
    {
        return new Vector3(
            target.position.x,
            target.position.y + verticalPivotOffset,
            target.position.z
        );
    }

    // =========================
    // ORBIT MATH
    // =========================
    private Vector3 CalculateOrbitPosition(Vector3 pivot)
    {
        float yawRad = yaw * Mathf.Deg2Rad;
        float pitchRad = pitch * Mathf.Deg2Rad;

        return new Vector3(
            pivot.x + radius * Mathf.Cos(pitchRad) * Mathf.Sin(yawRad),
            pivot.y + radius * Mathf.Sin(pitchRad),
            pivot.z + radius * Mathf.Cos(pitchRad) * Mathf.Cos(yawRad)
        );
    }

    // =========================
    // COLLISION
    // =========================
    private Vector3 ResolveCameraCollision(Vector3 pivot, Vector3 desiredPos)
    {
        Vector3 direction = desiredPos - pivot;
        float distance = direction.magnitude;
        direction.Normalize();

        if (Physics.Raycast(pivot, direction, out RaycastHit hit, distance, collisionMask))
        {
            return pivot + direction * (hit.distance - collisionPadding);
        }

        return desiredPos;
    }

}
