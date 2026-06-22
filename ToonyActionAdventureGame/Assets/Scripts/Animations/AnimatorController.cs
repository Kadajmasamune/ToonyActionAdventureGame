using Autodesk.Fbx;
using System.Collections;
using System.Collections.Generic;
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

    int currentAnimationHash = -1;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();

        MoveXHash = Animator.StringToHash("moveX");
        MoveYHash = Animator.StringToHash("moveY");
        JumpHash = Animator.StringToHash("Jump");

    }


    public void UpdateMovement(Vector3 worldVelocity, float strafingSpeed,float walkSpeed,float sprintSpeed )
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

    public void PlayAttack(int animationHash, int AttackIndex , bool isTransitioning , bool shouldCrossFade = false)
    {
        animator.SetBool("isAttacking", true);
        animator.SetBool("isTransitioning", isTransitioning);
        animator.SetInteger("AttackIndex", AttackIndex);
        //animator.SetTrigger(animationHash)
        ChangeAnimation(animationHash);
    }

    public void StopAttack(int animationHash)
    {
        animator.SetBool("isAttacking", false);
        animator.SetBool("isTransitioning", false);
        animator.SetInteger("AttackIndex", 0);
        //animator.ResetTrigger(animationHash);
    }


    public void ChangeAnimation(int targetHash, float delay = 0.0f, float crossfade = 0.09f)
    {
        if (currentAnimationHash == targetHash) return;

        if (delay > 0f)
        {
            StartCoroutine(WaitAndPlay());
        }
        else
        {
            Play();
        }

        IEnumerator WaitAndPlay()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, delay - crossfade));
            Play();
        }

        void Play()
        {

            animator.CrossFadeInFixedTime(targetHash, crossfade);
            currentAnimationHash = targetHash;
        }
    }
}


