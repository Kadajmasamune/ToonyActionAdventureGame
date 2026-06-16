using System.Collections;
using UnityEngine;
using static FrameData;
using static InputBufferer;

public class CombatStateMachine : MonoBehaviour // ----> “Given state + input + context → what happens next?”                                                
// ----> This class Decides Transition logic, not Attacks Themselves, they simply consume the signals given by the Machine and perform the exact Transition. 
{
        // --> Update Animation Combat state machine transition and animation playback logic
        // --> Use crossFade 

        // -- > Transition Logic 
        // -- > Begin Implementing Direction Modifiers 
        // -- > Timing Between inputs Modifiers 


    public Attack[] PossibleAttacks;
    public Attack CurrentAttack;

    [SerializeField] private InputBufferer bufferer;
    private AnimatorController animator;

    private int currentTick;
    private bool isAttacking;

    //private CancelWindowRuntime[] cancelWindows;
    public AttackRuntimeData currentAttackData;

    public struct AttackRuntimeData { public int startUp; public int active; public int recovery; }

    //public struct CancelWindowRuntime { public int start; public int end; }

    private void Start()
    {
        bufferer = GetComponent<InputBufferer>();
        animator = GetComponent<AnimatorController>();
    }

    private void Update()
    {
        if (!isAttacking)
        {
            TryStartAttack();
            return;
        }

        UpdateAttack();
        Debug.Log(currentTick);
    }


    private void TryStartAttack()
    {
        if (bufferer.AttackBuffer.Count == 0)
            return;

        AttackInput input = bufferer.AttackBuffer.Peek();

        Attack next = GetAttackFromInput(input);

        if (next == null)
            return;

        bufferer.AttackBuffer.Dequeue();
        StartAttack(next);
    }

    private Attack GetAttackFromInput(AttackInput input)
    {
        for (int i = 0; i < PossibleAttacks.Length; i++)
        {
            if (PossibleAttacks[i].RequiredInput == input)
                return PossibleAttacks[i];
        }

        return null;
    }


    private void StartAttack(Attack attack)
    {
        CurrentAttack = attack;
        isAttacking = true;
        currentTick = 0;

        //CacheAttackData(attack);

        int hash = Animator.StringToHash(attack.clip.name);
        animator.PlayAttack(hash);
    }

    private void UpdateAttack()
    {
        currentTick++;

        int totalFrames =
            CurrentAttack.StartUpFrames +
            CurrentAttack.ActiveFrames +
            CurrentAttack.RecoveryFrames;

        if (IsInCancelWindow(currentTick , CurrentAttack))
        {
            // combo logic later
            CancelAttack(ref CurrentAttack);
        }

        if (currentTick >= totalFrames)
        {
            EndAttack();
        }
    }


    private void EndAttack()
    {
        int hash = Animator.StringToHash(CurrentAttack.clip.name);
        animator.StopAttack(hash);

        CurrentAttack = null;
        isAttacking = false;
        currentTick = 0;
    }


    public bool IsInCancelWindow(int tick, Attack currentAttack)
    {
        if (currentAttack == null)
            return false;

        for (int i = 0; i < currentAttack.cancelWindows.Length; i++)
        {
            if (tick >= currentAttack.cancelWindows[i].startFrame && tick <= currentAttack.cancelWindows[i].endFrame)
                return true;
        }
        return false;

    }

    public void transitionAttack(ref Attack newAttack)
    {
        return;
    }

    public void CancelAttack(ref Attack currentAttack)
    {
        return;
    }


    public bool CanChain()
    {
        return false;
    }

    ////(Rework)
    //private void CacheAttackData(Attack attack)
    //{
    //    float scale = 60f / attack.clip.frameRate;
        
    //    currentAttackData = new AttackRuntimeData
    //    {
    //        startUp = Scale(attack.StartUpFrames, scale),
    //        active = Scale(attack.ActiveFrames, scale),
    //        recovery = Scale(attack.RecoveryFrames, scale),
    //    };

    //    if (attack.cancelWindows != null)
    //    {
    //        cancelWindows = new CancelWindowRuntime[attack.cancelWindows.Length];

    //        for (int i = 0; i < attack.cancelWindows.Length; i++)
    //        {
    //            cancelWindows[i] = new CancelWindowRuntime
    //            {
    //                start = Scale(attack.cancelWindows[i].startFrame, scale),
    //                end = Scale(attack.cancelWindows[i].endFrame, scale)
    //            };
    //        }
    //    }
    //    else
    //    {
    //        cancelWindows = new CancelWindowRuntime[0];
    //    }
    //}




    private int Scale(int frames, float scale)
    {
        return Mathf.RoundToInt(frames * scale);
    }
}


