using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
   
    private InputBufferer inputBufferer;

    private Ticker ticker;
    private int currentAttackTick;

    [Header("Combo State Machine")]
    public CombatStateMachine combatStateMachine { get; private set; }


    private void Start()
    {
        ticker = FindFirstObjectByType<Ticker>();
        inputBufferer = GetComponent<InputBufferer>();
        combatStateMachine = GetComponent<CombatStateMachine>();
    }

    private void Update()
    {
        if (combatStateMachine.CurrentAttack == null)
            return;

        currentAttackTick = ticker.CurrentTick;

        if (combatStateMachine.IsInCancelWindow(currentAttackTick))
        {
            // combo logic hook
            combatStateMachine.CancelAttack();  // --> Chain ? 
        }
    }


    
    public void SetAttack(Attack attack)
    {
        combatStateMachine.CurrentAttack = attack;
        CacheAttackData(attack);
    }




    private void CacheAttackData(Attack attack)
    {
        float scale = 60f / attack.clip.frameRate;

        combatStateMachine.currentAttackData = new CombatStateMachine.AttackRuntimeData
        {
            startUp = Scale(attack.StartUpFrames, scale),
            active = Scale(attack.ActiveFrames, scale),
            recovery = Scale(attack.RecoveryFrames, scale),
        };

        if (attack.cancelWindows != null)
        {
            combatStateMachine.cancelWindows = new CombatStateMachine.CancelWindowRuntime[attack.cancelWindows.Length];

            for (int i = 0; i < attack.cancelWindows.Length; i++)
            {
                combatStateMachine.cancelWindows[i] = new CombatStateMachine.CancelWindowRuntime
                {
                    start = Scale(attack.cancelWindows[i].startFrame, scale),
                    end = Scale(attack.cancelWindows[i].endFrame, scale)
                };
            }
        }
        else
        {
            combatStateMachine.cancelWindows = new CombatStateMachine.CancelWindowRuntime[0];
        }
    }




    private int Scale(int frames, float scale)
    {
        return Mathf.RoundToInt(frames * scale);
    }
}