using System.Collections.Generic;
using UnityEngine;
using static FrameData;
using static InputBufferer;

public class CombatStateMachine : MonoBehaviour
{
    [Header("Attack")]
    public Attack CurrentAttack;

    public bool isAttacking;
    public bool isAttackingInAir => isAttacking && handler.isInAir;
    public bool isTransitioning;
    public bool hasChainedThisWindow;

    public int CurrentAttackTick => execution != null ? execution.CurrentTick : 0;


    [Header("References")]
    [SerializeField] private InputBufferer bufferer;

    private AnimatorController animator;

    private IWeapon weaponHandler;
    private ICombatHandler handler;

    private AttackExecution execution;

    private bool isTryingToCancel;

    public void Initialize(ICombatHandler handler)
    {
        this.handler = handler;
    }


    public void Initialize(IWeapon weapon)
    {
        this.weaponHandler = weapon;
    }



    private void Awake()
    {
        bufferer = GetComponent<InputBufferer>();
        animator = GetComponent<AnimatorController>();

        execution = GetComponent<AttackExecution>();

        if (execution == null)
            execution = gameObject.AddComponent<AttackExecution>();

    }



    private void Start()
    {
        execution.Initialize( weaponHandler,animator);
    }


    private void OnEnable()
    {
        Ticker.OnTick += Tick;
    }


    private void OnDisable()
    {
        Ticker.OnTick -= Tick;
    }



    private void Tick()
    {
        isAttacking = execution.IsExecuting;

        if (!isAttacking)
        {
            TryStartAttack();
            return;
        }

        HandleTransitions();
    }

    private void TryStartAttack()
    {
        if (!bufferer.HasInput)
            return;


        AttackInput input = bufferer.ConsumeInput();
        Attack attack = GetAttackFromInput(input);

        if (attack == null)
            return;


        StartAttack(attack);
    }


    private Attack GetAttackFromInput(AttackInput input)
    {
        foreach (Attack attack in handler.currentWeapon.attacks)
        {
            if (attack.RequiredInput != input)
                continue;

            if (!HasContext(attack))
                continue;

            if (attack.DirectionRequired != Vector3.zero)
            {
                float dot = Vector3.Dot( handler.AttackDirection.normalized, attack.DirectionRequired.normalized);

                if (dot < 0.9f)
                    continue;
            }

            return attack;
        }

        return null;
    }



    private bool HasContext(Attack attack)
    {
        foreach (var required in attack.contextRequired)
        {
            bool found = false;

            foreach (var current in handler.currentHandlerContext)
            {
                if (current == required)
                {
                    found = true;
                    break;
                }
            }


            if (!found)
                return false;
        }


        return true;
    }


    private void StartAttack(Attack attack)
    {
        CurrentAttack = attack;

        isTransitioning = false;
        hasChainedThisWindow = false;

        execution.StartAttack(attack);
    }


    private void HandleTransitions()
    {
        if (CurrentAttack == null)
            return;

        if (execution.DeduceCurrentWindow(execution.CurrentTick, CurrentAttack) != WindowType.Interrupt )
        {
            hasChainedThisWindow = false;
            return;
        }

        Attack next = GetNewAttack(CurrentAttack);

        if (next != null)
        {
            hasChainedThisWindow = true;
            TransitionAttack(next);

        }
    }



    public Attack GetNewAttack(Attack currentAttack)
    {
        if (hasChainedThisWindow)
            return null;

        if (currentAttack == null)
            return null;

        if (currentAttack.AllowedAttackTransitions.Length == 0)
            return null;


        AttackInput input = bufferer.PeekInput();

        foreach (Attack transition in currentAttack.AllowedAttackTransitions)
        {

            if ( transition.RequiredInput == input && HasContext(transition) )
            {
                if (transition.DirectionRequired != Vector3.zero)
                {
                    float dot = Vector3.Dot( handler.AttackDirection.normalized, transition.DirectionRequired.normalized );

                    if (dot < 0.9f)
                        continue;
                }

                bufferer.ConsumeInput();
                return transition;
            }

        }


        return null;
    }



    public void TransitionAttack(Attack next)
    {
        if (next == null)
            return;

        CurrentAttack = next;

        isTransitioning = true;


        execution.StartAttack( next, transition:true);
    }

    public void CancelAttack()
    {
        // Future:
        // dodge
        // parry
        // jump cancel
    }
}
