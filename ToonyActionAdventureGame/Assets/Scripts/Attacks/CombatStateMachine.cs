using Common;
using Mono.Cecil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;
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
    //public Attack[] PossibleAttacks;
    public Attack CurrentAttack;
    public bool isAttacking = false;
    public bool isTransitioning = false;
    public bool hasChainedThisWindow = false;
    public bool shouldLaunchTarget => (CurrentAttack.upwardLaunchImpulse > 0 && CurrentAttack.knockBackInflicted <= 0);
    public int CurrentAttackTick {  get; private set; }
    private IReadOnlyList<Collider> targetsHit => weaponHanlder.Targets;
    private HashSet<Collider> hitTargets = new HashSet<Collider>();
    private class ImpactData
    {
        public Vector3 start;
        public Vector3 end;
        public float timer;
        public float duration;
        public AnimationCurve curve;
    }
    private Dictionary<Collider, ImpactData> impacts = new();
    private List<Collider> finishedImpacts = new();
    public bool isTargetHit;
    private bool isTryingToCancel = false;


    [Header("Weapon")]
    //public Weapon currentWeapon;
    //public WeaponHandling currentWeapon;
    private IWeapon weaponHanlder;
    private ICombatHandler handler;
    public Attack.Context[] currentContext;
    


    [Header("Ticker Config")]
    private int attackStartTick;

    [Header("References")]
    [SerializeField] private InputBufferer bufferer;
    private AnimatorController animator;

    public void Initialize(ICombatHandler handler)
    {
        this.handler = handler;
    }
    public void Initialize(IWeapon handler)
    {
        this.weaponHanlder = handler;
    }


    private void Start()
    {
        bufferer = GetComponent<InputBufferer>();
        animator = GetComponent<AnimatorController>();
        //currentWeapon = FindFirstObjectByType<WeaponHandling>();
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
        finishedImpacts.Clear();

        foreach (var impact in impacts)
        {
            UpdateImpact(impact.Key, impact.Value);

            if (impact.Value.timer >= impact.Value.duration)
            {
                finishedImpacts.Add(impact.Key);
            }
        }


        foreach (Collider target in finishedImpacts)
        {
            impacts.Remove(target);
        }


        if (!isAttacking && !isTransitioning)
        {
            TryStartAttack();
            return;
        }

        UpdateAttack();
    }

    private void TryStartAttack()
    {

        if (!bufferer.HasInput) return;


        var input = bufferer.ConsumeInput();
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
                if (Vector3.Dot(
                    handler.AttackDirection.normalized,
                    attack.DirectionRequired.normalized
                    ) < 0.9f)
                    continue;
            }


            return attack;
        }

        return null;
    }

    private bool HasContext(Attack attack)
    {
        foreach (Attack.Context required in attack.contextRequired)
        {
            bool found = false;

            foreach (Attack.Context current in handler.currentHandlerContext)
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
        isAttacking = true;
        isTransitioning = false ;
        hasChainedThisWindow = false;
        //currentWeapon.GetComponent<Collider>().enabled = false ;
        weaponHanlder.DisableHitbox();
        weaponHanlder.ClearTargets();
        hitTargets.Clear();
        //currentWeapon.Targets.Clear();

        attackStartTick = Ticker.instance.CurrentTick;
        //CacheAttackData(attack);

        int hash = Animator.StringToHash(attack.clip.name);
        animator.PlayAttack(hash , attack.AttackIndexLayer , isTransitioning);
    }

    private void UpdateAttack()
    {
        int tick = Ticker.instance.CurrentTick - attackStartTick;
        CurrentAttackTick = tick;
        if (isTransitioning)
            isTransitioning = false;


        int totalFrames =
            CurrentAttack.StartUpFrames +
            CurrentAttack.ActiveFrames +
            CurrentAttack.RecoveryFrames;

        //Debug.Log(totalFrames);

        UpdateWeaponState(tick);
        HandleAttackProperties(CurrentAttack);

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
        int i = 0;
        foreach (Attack transition in currentAttack.AllowedAttackTransitions)
        {
            if (
                transition.RequiredInput == input &&
                HasContext(transition) &&
                Vector3.Dot(
                    handler.AttackDirection.normalized,
                    transition.DirectionRequired.normalized
                ) > 0.9f
            )
            {
                bufferer.ConsumeInput();
                return transition;
            }
            i++;
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


    private void UpdateWeaponState(int tick)
    {
        int activeStart = CurrentAttack.StartUpFrames;
        int activeEnd = activeStart + CurrentAttack.ActiveFrames;


        if (tick >= activeStart && tick <= activeEnd)
        {
            weaponHanlder.EnableHitbox();
        }
        else
        {
            weaponHanlder.DisableHitbox();
        }
    }


    private void HandleAttackProperties(Attack currentAttack)
    {
        foreach (Collider target in targetsHit)
        {
            if (hitTargets.Contains(target))
                continue;

            hitTargets.Add(target);

            if (shouldLaunchTarget)
                StartLaunch(target, currentAttack);

            if (!shouldLaunchTarget)
                ApplyKnockBack(target, currentAttack);

            Health health = target.GetComponent<Health>();

            if (health != null)
                health.TakeDamage(currentAttack.Damage);

            StartCoroutine(ApplyHitStop(currentAttack));
        }
    }

    private void StartLaunch(Collider target, Attack attack)
    {
        Vector3 start = target.transform.position;

        Vector3 end = start + Vector3.up * attack.upwardLaunchImpulse;

        impacts[target] = new ImpactData
        {
            start = start,
            end = end,
            timer = 0,
            duration = attack.attackSpeed,
            curve = attack.lungeCurve
        };
    }
    private void ApplyKnockBack(Collider target, Attack attack)
    {
        Vector3 start = target.transform.position;

        Vector3 direction =
            (target.transform.position - transform.position).normalized;


        Vector3 end =
            start + direction * attack.knockBackInflicted;


        impacts[target] = new ImpactData
        {
            start = start,
            end = end,
            timer = 0,
            duration = attack.attackSpeed,
            curve = attack.lungeCurve
        };
    }

    private void ApplyHitStun(Collider target , Attack attack)
    {

    }


    private void UpdateImpact(Collider target, ImpactData data)
    {
        if (target == null)
            return;


        data.timer += Time.deltaTime;


        float t =
            Mathf.Clamp01(data.timer / data.duration);


        float eased =
            data.curve.Evaluate(t);


        target.transform.position =
            Vector3.Lerp(
                data.start,
                data.end,
                eased
            );

    }

    private IEnumerator ApplyHitStop(Attack attack)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(attack.hitStopDuration);
        Time.timeScale = 1f;
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
        isTargetHit = false;
        weaponHanlder.DisableHitbox();
        weaponHanlder.ClearTargets();
        hitTargets.Clear();
        //currentWeapon.GetComponent<Collider>().enabled = false;
        //currentWeapon.Targets.Clear();
    }

}

