using UnityEngine;
using System.Collections.Generic;

public class AnimatorController : MonoBehaviour
{
    //Describes Intent and feeds it to the Animator

    [SerializeField] private Animator animator;
    
    public float moveAmount;
    public float maxMoveAmount = 2f;

    private int MoveAmountHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        MoveAmountHash = Animator.StringToHash("moveAmount");
        moveAmount = animator.GetFloat(MoveAmountHash);
        moveAmount = 0f;
    }

    private void Update()
    {
        animator.SetFloat(MoveAmountHash, moveAmount);  
    }
}
