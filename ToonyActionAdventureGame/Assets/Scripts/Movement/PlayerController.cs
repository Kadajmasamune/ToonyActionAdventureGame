using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AnimatorController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float accelerationAmount = 5f;  
    [SerializeField] private float angularSpeed = 15f;       
    [SerializeField] private Transform cameraTransform;     
    private Quaternion targetRotation;

    private AnimatorController playerAnimatorController;
    private bool IsSprinting = false;

    private void Awake()
    {
        playerAnimatorController = GetComponent<AnimatorController>();
        targetRotation = transform.rotation;
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 moveDir = (camForward * v + camRight * h);
        float inputMagnitude = Mathf.Clamp01(moveDir.magnitude);
        moveDir.Normalize();

        IsSprinting = inputMagnitude > 0f && Input.GetKey(KeyCode.LeftShift);

        if (inputMagnitude > 0f)
        {
            float yaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0f, yaw, 0f);
        }

        float targetMoveAmount = inputMagnitude > 0f ? 1f : 0f;
        if (IsSprinting) targetMoveAmount = 2f;

        playerAnimatorController.moveAmount = Mathf.MoveTowards(
            playerAnimatorController.moveAmount,
            targetMoveAmount,
            accelerationAmount * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            angularSpeed * Time.deltaTime
        );
    }
}
