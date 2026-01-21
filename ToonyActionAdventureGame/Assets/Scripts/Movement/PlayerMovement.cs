using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float velocity = 5f;

    private Rigidbody rb;
    private Vector3 physicsMoveDir;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        #region Logs
        Debug.Log($"{name}: Script enabled={enabled}, activeInHierarchy={gameObject.activeInHierarchy}, velocity={velocity}");
        if (rb == null)
            Debug.Log($"{name}: No Rigidbody found (will use transform movement).");
        else
            Debug.Log($"{name}: Rigidbody found (isKinematic={rb.isKinematic}, constraints={rb.constraints}).");
        Debug.Log($"{name}: Application.isFocused={Application.isFocused}, Time.timeScale={Time.timeScale}");
        #endregion
    }

    void Update()
    {
        // Read raw axis input (no smoothing)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        physicsMoveDir = new Vector3(h, 0f, v);
        physicsMoveDir = Vector3.ClampMagnitude(physicsMoveDir, 1f);
    }

    void FixedUpdate()
    { 
        Vector3 before = rb.position;  
        
        StartCoroutine(MovementSplashParticle.instance.play());
        rb.MovePosition(rb.position + physicsMoveDir * velocity * Time.fixedDeltaTime);
        
        Vector3 after = rb.position;

        if (before == after)
            Debug.LogWarning($"{name}: Rigidbody.MovePosition/position assignment ran but position unchanged (before={before}). Check collisions/constraints/other scripts.");
        else
            Debug.Log($"{name}: Rigidbody moved from {before} to {after}");
    }
}
