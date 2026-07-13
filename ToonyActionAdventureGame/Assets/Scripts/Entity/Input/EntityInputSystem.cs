using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntityInputSystem : MonoBehaviour, IEntitySystem, ICameraInput , IMovementInput
{
    public InputAction jumpAction { get; private set; }
    public InputAction sprintAction { get; private set; }
    public InputAction moveAction { get; private set; }
    public InputAction LockOn { get; private set; }

    public void Init()
    {
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        moveAction = InputSystem.actions.FindAction("Move");
        LockOn = InputSystem.actions.FindAction("LockOn");
        
    }

    public void Update()
    {
        
        return ;    
    }

    public Vector3 GetCameraRelativeInput(Transform cameraTransform)
    {
        Vector2 Input = moveAction.ReadValue<Vector2>();
        Vector2.ClampMagnitude(Input, 1f);


        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 dir = camForward * Input.y + camRight * Input.x;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        return dir;
    }
}