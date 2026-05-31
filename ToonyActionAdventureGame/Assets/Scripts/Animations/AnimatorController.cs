using Autodesk.Fbx;
using Unity.VisualScripting;
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

    //--- > Fix Y Axis Animation Blend 
    public void UpdateMovement(
       Vector3 worldVelocity,
       float strafingSpeed,
       float walkSpeed,
       float sprintSpeed
   )
    {
        Vector3 localVel = transform.InverseTransformDirection(worldVelocity);

        // -------------------------
        // X (STRAFE)
        // -------------------------
        float x = localVel.x / Mathf.Max(strafingSpeed, 0.01f);
        x = Mathf.Clamp(x, -1f, 1f);
        x *= 0.5f;

        // -------------------------
        // Y (FORWARD / SPEED ZONE)
        // -------------------------

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
                float runPercent =
                    (z - walkSpeed) / (sprintSpeed - walkSpeed);

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

    public void PlayAttack(int animationHash)
    {
        animator.SetBool("isAttacking", true);
        animator.SetTrigger(animationHash);
    }

    public void StopAttack(int animationHash)
    {
        animator.SetBool("isAttacking", false);
        animator.ResetTrigger(animationHash);
    }


}


public class AnimatorStateMachine : MonoBehaviour
{
    public Animator aniamtor; 

    public int currentAnimationHash;

    private void Awake()
    {
        aniamtor = GetComponent<Animator>();    
    }

    public void switchAnimation(int newAnimationHash)
    {
        currentAnimationHash = newAnimationHash;
    }


}
