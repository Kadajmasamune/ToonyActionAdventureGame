using Common;
using UnityEngine;
using static FrameData;
using static InputBufferer;

public class CombatStateMachine : MonoBehaviour // ----> “Given state + input + context → what happens next?”                                                
{


    // -- > Implement stricter flow of attacks 
    // -- > Remove Spamming input and attack


    // -- > Begin Implementing Direction Modifiers 
    // -- > Timing Between inputs Modifiers 


    [Header("Attack")]
    public Attack[] PossibleAttacks;
    public Attack CurrentAttack;
    private bool isAttacking = false;
    private bool isTransitioning = false;
    private bool hasChainedThisWindow = false;


    private bool isTryingToCancel = false;

    [Header("Ticker Config")]
    private int attackStartTick;

    [Header("References")]
    [SerializeField] private InputBufferer bufferer;
    private AnimatorController animator;

   
    private void Start()
    {
        bufferer = GetComponent<InputBufferer>();
        animator = GetComponent<AnimatorController>();
    }

    private void OnEnable()
    {
        Ticker.OnTick += OnTick;
    }

    private void OnDisable()
    {
        Ticker.OnTick -= OnTick;
    }

    private void OnTick()
    {
        if (!isAttacking && !isTransitioning)
        {
            TryStartAttack();
            return;
        }

        UpdateAttack();
        //Debug.Log($"Ticker:{Ticker.instance.CurrentTick} start:{attackStartTick} tick:{(Ticker.instance.CurrentTick - attackStartTick)} attack:{(CurrentAttack?.clip?.name ?? "null")}");
    }

    private void TryStartAttack()
    {

        if (!bufferer.HasInput) return;
        else
        {
            var input = GetAttackFromInput(bufferer.ConsumeInput());
            StartAttack(input);
        }
     
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
        isTransitioning = false ;
        hasChainedThisWindow = false;

        attackStartTick = Ticker.instance.CurrentTick;
        //CacheAttackData(attack);

        int hash = Animator.StringToHash(attack.clip.name);
        animator.PlayAttack(hash , attack.AttackIndexLayer , isTransitioning);
    }

    private void UpdateAttack()
    {
        int tick = Ticker.instance.CurrentTick - attackStartTick;

        if (isTransitioning)
            isTransitioning = false;


        int totalFrames =
            CurrentAttack.StartUpFrames +
            CurrentAttack.ActiveFrames +
            CurrentAttack.RecoveryFrames;

        //Debug.Log(totalFrames);


        if (DeduceCurrentWindow(tick, CurrentAttack) != WindowType.Interrupt)
        {
            hasChainedThisWindow = false;
        }
        else
        {
            Attack newAttack = GetNewAttack(CurrentAttack);

            if (newAttack != null)
            {
                hasChainedThisWindow = true;
                TransitionAttack(newAttack);
                return;
            }

            if (isTryingToCancel)
                CancelAttack(CurrentAttack);
        }


        if (DeduceCurrentWindow(tick, CurrentAttack) == WindowType.Invulnerability)
        {
            Invulnerabilize();
        }

        if (tick >= totalFrames)
        {
            EndAttack();
            //Debug.Log(tick);
        }
    }
    public WindowType DeduceCurrentWindow(int tick, Attack currentAttack)
    {
        if (currentAttack == null)
            return WindowType.None;

        
        for (int i = 0; i < currentAttack.FrameWindows.Length; i++)
        {
        
            if (currentAttack.FrameWindows[i].windowType == WindowType.Interrupt &&
                (tick >= currentAttack.FrameWindows[i].startFrame &&
                tick <= currentAttack.FrameWindows[i].endFrame))
                return WindowType.Interrupt;


            else if (currentAttack.FrameWindows[i].windowType == WindowType.Invulnerability &&
                (tick >= currentAttack.FrameWindows[i].startFrame &&
                tick <= currentAttack.FrameWindows[i].endFrame))

                return WindowType.Invulnerability;
        }


        return WindowType.None;
    }

    public Attack GetNewAttack(Attack currentAttack)
    {
        if (hasChainedThisWindow) return null;
        if (currentAttack == null) return null;
        if (currentAttack.AllowedAttackTransitions.Length == 0) return null;

        AttackInput input = bufferer.PeekInput();

        foreach (Attack transition in currentAttack.AllowedAttackTransitions)
        {
            if (transition.RequiredInput == input)
            {
                bufferer.ConsumeInput();
                return transition;
            }
        }

        return null;
    }
    public void TransitionAttack(Attack newAttack)
    {
        CurrentAttack = newAttack;
        attackStartTick = Ticker.instance.CurrentTick;
        isAttacking = true;
        isTransitioning = true;
        hasChainedThisWindow = false;
        //Debug.Log($"Chaining into  {CurrentAttack.name}");
        int hash = Animator.StringToHash(newAttack.clip.name);
        animator.PlayAttack(hash , newAttack.AttackIndexLayer , isTransitioning);
    }


    public void CancelAttack(Attack currentAttack)
    {
        //Interrupt Attack with a different move (i.e. Jump  , parry etc) 
        return;
    }


    private void Invulnerabilize()
    {
        // Invincible during these Frames
    }

    private void EndAttack()
    {
        int hash = Animator.StringToHash(CurrentAttack.clip.name);
        animator.StopAttack(hash);

        CurrentAttack = null;
        isAttacking = false;
        isTransitioning = false;
        hasChainedThisWindow = false;
        attackStartTick = 0;

    }

}
