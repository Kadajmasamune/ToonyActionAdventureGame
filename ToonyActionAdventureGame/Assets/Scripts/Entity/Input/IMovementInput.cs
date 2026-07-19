using UnityEngine.InputSystem;
using UnityEngine;
public interface IMovementInput
{
    public InputAction jumpAction { get; }
    public InputAction sprintAction { get; }
    public InputAction moveAction { get; }  
    public InputAction dashAction { get; }
    public Vector3 GetCameraRelativeInput(Transform cameraTransform);
}