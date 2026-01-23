using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    public List<AnimationClip> clips = new List<AnimationClip>();

    private int velocityHash;
    public float acceleration = 0.1f;
    private float velocity = 0f;
    private float maxVelocity = 1f;
    private void Awake()
    {
        animator = GetComponent<Animator>();

        var runtimeController = animator != null ? animator.runtimeAnimatorController : null;
        if (runtimeController != null)
        {
            foreach (var clip in runtimeController.animationClips)
            {
                clips.Add(clip);
            }
        }

        velocityHash = Animator.StringToHash("velocity");
    }

    private void Update()
    {
        Run();
    }

    private void Run()
    {
        float delta = acceleration * Time.deltaTime;

        if (Input.GetKey(KeyCode.W))
        {
            velocity = Mathf.Clamp(velocity + delta, 0f, maxVelocity);
        }
        else
        {
            velocity = Mathf.Clamp(velocity - delta, 0f, maxVelocity);
        }

        if (animator != null)
        {
            animator.SetFloat(velocityHash, velocity);
        }
    }

}
