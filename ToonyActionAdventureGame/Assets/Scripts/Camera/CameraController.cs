using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform LockOnTarget;

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

    [Header("Damping (response time in seconds)")]
    [SerializeField] private float positionResponse = 0.08f;
    [SerializeField] private float rotationResponse = 0.05f;

    private float yaw, pitch;
    private float yawVel, pitchVel;

    private Vector3 positionVelocity;

    private int scrollWheel = 2; 
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 euler = transform.rotation.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    private void Update()
    {
        HandleInput();
    }

    private void LateUpdate()
    {
        Vector3 pivot = GetPivot(target);

        yaw = Damp(yaw, targetYaw, ref yawVel, rotationResponse);
        pitch = Damp(pitch, targetPitch, ref pitchVel, rotationResponse);

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);


        Vector3 desiredPos = pivot - rotation * Vector3.forward * radius;
        desiredPos = ResolveCollision(pivot, desiredPos);

        transform.position = Vector3Damp(
            transform.position,
            desiredPos,
            ref positionVelocity,
            positionResponse
        );

        transform.rotation = rotation;
    }

    // =========================
    // INPUT
    // =========================
    private float targetYaw;
    private float targetPitch;

    private void HandleInput()
    {
        targetYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        targetPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
    }

    // =========================
    // PIVOT
    // =========================
    private Vector3 GetPivot(Transform target)
    {
        return target.position + Vector3.up * verticalPivotOffset;
    }

    // =========================
    // COLLISION
    // =========================
    private Vector3 ResolveCollision(Vector3 pivot, Vector3 desiredPos)
    {
        Vector3 dir = desiredPos - pivot;
        float dist = dir.magnitude;
        dir.Normalize();

        if (Physics.Raycast(pivot, dir, out RaycastHit hit, dist, collisionMask))
        {
            return pivot + dir * (hit.distance - collisionPadding);
        }

        return desiredPos;
    }

    private float Damp(float current, float target, ref float velocity, float response)
    {
        float omega = 4.6f / response;
        float x = current - target;

        float accel = -omega * omega * x - 2f * omega * velocity;
        velocity += accel * Time.deltaTime;
        current += velocity * Time.deltaTime;

        return current;
    }

    private Vector3 Vector3Damp(Vector3 current, Vector3 target, ref Vector3 velocity, float response)
    {
        float omega = 4.6f / response;
        Vector3 x = current - target;

        Vector3 accel = -omega * omega * x - 2f * omega * velocity;
        velocity += accel * Time.deltaTime;
        current += velocity * Time.deltaTime;

        return current;
    }

    private bool isLockingOn()
    {
        if(Input.GetMouseButtonDown(scrollWheel))
        {
            return true;
        }
        return false;
    }
    private void LockOn(Transform lockOnTarget)
    {
        //Make Camera Rotate about new Pivot 
        // Where the Player's Transform Changes dictate Camera's Transform Changes

        Vector3 newPivot = GetPivot(lockOnTarget);
        Quaternion rotation;
        float LockingOnRadius;    
            
        if (newPivot != null && isLockingOn())
        {
            
        }
    }
}
