using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public float moveX;
    public float moveY;
    public bool Jump; 

    private int MoveXHash;
    private int MoveYHash;
    private int JumpHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        MoveXHash = Animator.StringToHash("moveX");
        MoveYHash = Animator.StringToHash("moveY");
        JumpHash = Animator.StringToHash("Jump");

        moveX = 0f;
        moveY = 0f;
        Jump = false;
    }

    private void Update()
    {
        animator.SetFloat(MoveXHash, moveX);
        animator.SetFloat(MoveYHash, moveY);
        animator.SetBool(JumpHash, Jump);
    }
}
