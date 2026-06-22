using UnityEngine;

public class InputBufferer : MonoBehaviour
{
    public enum AttackInput { Light, Heavy, None }

    private struct BufferedInput
    {
        public AttackInput input;
        public int tick;
    }

    private BufferedInput lastInput;
    private bool hasInput;

    [Header("Ticker")]
    private Ticker ticker;

    [Header("Buffer Settings")]
    [SerializeField] private int inputBufferWindow = 10;

    private void Start()
    {
        ticker = FindFirstObjectByType<Ticker>();

        if (ticker == null)
            Debug.LogError("Ticker Not Found");
    }

    private void Update()
    {
        ReadInput();
        ExpireInput();
    }

    private void ReadInput()
    {
        if (Input.GetMouseButtonDown((int)AttackInput.Light))
        {
            StoreInput(AttackInput.Light);
        }
        else if (Input.GetMouseButtonDown((int)AttackInput.Heavy))
        {
            StoreInput(AttackInput.Heavy);
        }
    }

    private void StoreInput(AttackInput input)
    {
        lastInput = new BufferedInput
        {
            input = input,
            tick = ticker.CurrentTick
        };

        hasInput = true;
    }

    private void ExpireInput()
    {
        if (!hasInput)
            return;

        if (ticker.CurrentTick - lastInput.tick > inputBufferWindow)
        {
            hasInput = false;
        }
    }

    public bool HasInput => hasInput;

    public AttackInput PeekInput()
    {
        if (!hasInput)
            return AttackInput.None;

        return lastInput.input;
    }

    public AttackInput ConsumeInput()
    {
        if (!hasInput)
            return AttackInput.None;

        hasInput = false;
        return lastInput.input;
    }
}