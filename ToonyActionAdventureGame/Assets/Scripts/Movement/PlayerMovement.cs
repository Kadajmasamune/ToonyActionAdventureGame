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
        Debug.Log($"{name}: Script enabled={enabled}, activeInHierarchy={gameObject.activeInHierarchy}, velocity={velocity}");
        if (rb == null)
            Debug.Log($"{name}: No Rigidbody found (will use transform movement).");
        else
            Debug.Log($"{name}: Rigidbody found (isKinematic={rb.isKinematic}, constraints={rb.constraints}).");
        Debug.Log($"{name}: Application.isFocused={Application.isFocused}, Time.timeScale={Time.timeScale}");
    }

    void Update()
    {
        // Read raw axis input (no smoothing)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Quick checks
        if (!Application.isFocused)
        {
            // Input.GetAxis requires the Game window focused when running in Editor
            Debug.LogWarning($"{name}: Application not focused. Click Game view or window to send input.");
        }

        Vector3 input = new Vector3(h, 0f, v);
        input = Vector3.ClampMagnitude(input, 1f);

        if (input != Vector3.zero)
        {
            Debug.Log($"{name}: Axis input detected h={h}, v={v}, inputVec={input}");
        }

        if (rb == null)
        {
            if (input != Vector3.zero)
            {
                Vector3 before = transform.position;
                transform.Translate(input * velocity * Time.deltaTime, Space.World);
                Vector3 after = transform.position;
                if (before == after)
                    Debug.LogWarning($"{name}: transform.Translate executed but position unchanged (before={before}). Check for parent constraints or other scripts.");
                else
                    Debug.Log($"{name}: transform moved from {before} to {after}");
            }
        }
        else
        {
            // With Rigidbody use physics move in FixedUpdate
            physicsMoveDir = input;
        }
    }

    void FixedUpdate()
    {
        if (rb == null || physicsMoveDir == Vector3.zero)
            return;

        // Check full freeze
        bool freezeX = (rb.constraints & RigidbodyConstraints.FreezePositionX) != 0;
        bool freezeY = (rb.constraints & RigidbodyConstraints.FreezePositionY) != 0;
        bool freezeZ = (rb.constraints & RigidbodyConstraints.FreezePositionZ) != 0;
        if (freezeX && freezeY && freezeZ)
        {
            Debug.LogWarning($"{name}: Rigidbody position fully constrained. Cannot move. Constraints={rb.constraints}");
            return;
        }

        Vector3 before = rb.position;
        if (rb.isKinematic)
        {
            StartCoroutine(MovementSplashParticle.instance.play());
            rb.position = rb.position + physicsMoveDir * velocity * Time.fixedDeltaTime;
        }
        else
        {
            StartCoroutine(MovementSplashParticle.instance.play());
            rb.MovePosition(rb.position + physicsMoveDir * velocity * Time.fixedDeltaTime);
        }
        Vector3 after = rb.position;

        if (before == after)
            Debug.LogWarning($"{name}: Rigidbody.MovePosition/position assignment ran but position unchanged (before={before}). Check collisions/constraints/other scripts.");
        else
            Debug.Log($"{name}: Rigidbody moved from {before} to {after}");
    }
}
