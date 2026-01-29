using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public float moveX;
    public float moveY;

    private int MoveXHash;
    private int MoveYHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        MoveXHash = Animator.StringToHash("moveX");
        MoveYHash = Animator.StringToHash("moveY");

        moveX = 0f;
        moveY = 0f;
    }

    private void Update()
    {
        animator.SetFloat(MoveXHash, moveX);
        animator.SetFloat(MoveYHash, moveY);
    }
}
