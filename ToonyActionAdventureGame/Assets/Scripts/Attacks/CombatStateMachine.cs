using NUnit.Framework.Constraints;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static FrameData;
using static InputBufferer;

public class CombatStateMachine : MonoBehaviour // ----> “Given state + input + context → what happens next?”                                                
// ----> This class Decides Transition logic, not Attacks Themselves, they simply consume the signals given by the Machine and perform the exact Transition. 
{
        // --> Animation Interrupt Logic 
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
    //public AttackRuntimeData currentAttackData;

    //public struct AttackRuntimeData { public int startUp; public int active; public int recovery; } 
    //public struct CancelWindowRuntime { public int start; public int end; }


    private AnimatorStateInfo AttackAnimationInfo;
    private AnimatorTransitionInfo AttackTransitionInfo; 


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
        //Debug.Log(currentTick);
    }


    private void TryStartAttack()
    {
        if (bufferer.AttackBuffer.Count == 0)
            return;

        Attack next = GetAttackFromInput(bufferer.attackInput);

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
        
        Debug.Log(totalFrames);
        if (IsInCancelWindow(currentTick , CurrentAttack))
        {
            // combo logic later
            CancelAttack(CurrentAttack);
        }

        if(canChain(currentTick , CurrentAttack , out Attack newAttack))
        {
            if(newAttack != null)
                TransitionAttack(newAttack);
        }

        if (currentTick >= totalFrames)
        {
            EndAttack();
        }
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

    public void CancelAttack(Attack currentAttack)
    {
        //Interrupt Attack with a different move (i.e. Jump ) 
        return;
    }

    public bool canChain(int tick , Attack currentAttack , out Attack newAttack)
    {
        newAttack = null;

        if (currentAttack == null) return false;

        if (tick > (currentAttack.StartUpFrames + currentAttack.ActiveFrames) && tick <= (currentAttack.RecoveryFrames + currentAttack.ActiveFrames + CurrentAttack.StartUpFrames))
        {
            foreach(Attack transition in currentAttack.AllowedAttackTransitions)
            {
                if (transition.RequiredInput == bufferer.attackInput)
                {
                    newAttack = transition;
                    //Debug.Log("Chaining");
                    return true;
                }
            }
        }

        return false;
    }

    public void TransitionAttack(Attack newAttack)
    {
        CurrentAttack = newAttack;
        currentTick = 0;
        isAttacking = true;

        int hash = Animator.StringToHash(newAttack.clip.name);
        animator.PlayAttack(hash);
    }

    private void EndAttack()
    {
        int hash = Animator.StringToHash(CurrentAttack.clip.name);
        animator.StopAttack(hash);

        CurrentAttack = null;
        isAttacking = false;
        currentTick = 0;
    }

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


    //private int Scale(int frames, float scale)
    //{
    //    return Mathf.RoundToInt(frames * scale);
    //}


