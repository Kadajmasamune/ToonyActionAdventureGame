using UnityEngine;
using UnityEngine.Animations;
using System.Collections.Generic;
public class AnimatorController : MonoBehaviour
{
    private Animator animator;
    private AnimatorController animatorController;
    List<AnimationClip> clips;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animatorController = GetComponent<AnimatorController>();
        foreach(var clip in animatorController.clips)
        {
            clips.Add(clip);
        }
    }


}
