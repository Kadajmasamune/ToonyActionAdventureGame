using System;
using UnityEngine; 


public class EntityInputSystem : MonoBehaviour , IEntitySystem
{
    public Vector2 Input { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool SprintHeld { get; private set; }

    public bool isAI;
    public void Init()
    {
        
    }

    public void Update()
    {
        ReadInput();
    }

    void ReadInput()
    {
        if (!isAI)
        {
            Input = new Vector2(
                UnityEngine.Input.GetAxisRaw("Horizontal"),
                UnityEngine.Input.GetAxisRaw("Vertical")
            );

            JumpPressed = UnityEngine.Input.GetKeyDown(KeyCode.Space);
            SprintHeld = UnityEngine.Input.GetKey(KeyCode.LeftControl);
        }
        else
        {

        }
    }

    public Vector3 GetCameraRelativeInput(Transform cameraTransform)
    {
        Vector3 camForward = Vector3.ProjectOnPlane(
            cameraTransform.forward,
            Vector3.up).normalized;

        Vector3 camRight = Vector3.ProjectOnPlane(
            cameraTransform.right,
            Vector3.up).normalized;

        Vector3 dir = camForward * Input.y + camRight * Input.x;

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        return dir;
    }
}