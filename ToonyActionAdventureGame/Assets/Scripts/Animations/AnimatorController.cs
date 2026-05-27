using UnityEngine;

public class AnimatorController : MonoBehaviour
{

    [SerializeField] public Animator animator;

    [SerializeField] private float walkSpeed = 2.2f;
    [SerializeField] private float sprintSpeed = 4.8f;

    private int MoveXHash;
    private int MoveYHash;
    private int JumpHash;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        MoveXHash = Animator.StringToHash("moveX");
        MoveYHash = Animator.StringToHash("moveY");
        JumpHash = Animator.StringToHash("Jump");
    }

    public void UpdateMovement(Vector3 worldVelocity, float walkSpeed, float sprintSpeed)
    {
        Vector3 localVel = transform.InverseTransformDirection(worldVelocity);

        // X = strafe
        float x = Mathf.Abs(localVel.x) < 0.01f ? 0f : localVel.x / sprintSpeed;
        x = Mathf.Clamp(x, -1f, 1f);

        // Z = forward/back
        float z = localVel.z;
        float y = 0f;

        if (Mathf.Abs(z) > 0.01f)
        {
            if (Mathf.Abs(z) <= walkSpeed)
            {
                y = (z / walkSpeed) * 0.5f;
            }
            else
            {
                float runPercent = (z - walkSpeed) / (sprintSpeed - walkSpeed);
                runPercent = Mathf.Clamp01(runPercent);
                y = 0.5f + runPercent * 0.5f;
            }
        }

        animator.SetFloat(MoveXHash, x);
        animator.SetFloat(MoveYHash, y);
    }

    public void TriggerJump()
    {
        animator.SetTrigger(JumpHash);
    }

    public void ResetTriggerJump()
    {
        animator.ResetTrigger(JumpHash);
    }

    public void PlayAttack(int comboIndex)
    {
        animator.SetInteger("AttackIndex", comboIndex);   
    }
}