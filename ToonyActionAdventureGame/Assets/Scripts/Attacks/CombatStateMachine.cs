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
    public Attack[] PossibleAttacks;
    public Attack CurrentAttack;
    public bool isAttacking = false;
    public bool isTransitioning = false;
    public bool hasChainedThisWindow = false;
    public bool shouldLaunchTarget => (CurrentAttack.upwardLaunchImpulse > 0 && CurrentAttack.knockBackInflicted <= 0);
    public int CurrentAttackTick {  get; private set; }
    public List<Collider> targetsHit => currentWeapon.Targets;
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


    private ICombatHandler handler;

    [Header("Weapon")]
    //public Weapon currentWeapon;
    public WeaponHandling currentWeapon;
    


    [Header("Ticker Config")]
    private int attackStartTick;

    [Header("References")]
    [SerializeField] private InputBufferer bufferer;
    private AnimatorController animator;

    public void Initialize(ICombatHandler handler)
    {
        this.handler = handler;
    }
    private void Start()
    {
        bufferer = GetComponent<InputBufferer>();
        animator = GetComponent<AnimatorController>();
        currentWeapon = FindFirstObjectByType<WeaponHandling>();
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
        currentWeapon.GetComponent<Collider>().enabled = false ;
        hitTargets.Clear();
        currentWeapon.Targets.Clear();

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

        HandleWeaponCollider(tick , CurrentAttack , currentWeapon);
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

   
    private void HandleWeaponCollider(int tick , Attack currentAttack ,WeaponHandling currentWeapon)
    {
        Collider weaponCollider = currentWeapon.GetComponent<Collider>();
        int ActiveEnd = currentAttack.StartUpFrames + currentAttack.ActiveFrames;
        if (tick >= currentAttack.StartUpFrames && tick <= ActiveEnd)
        {
            if (weaponCollider != null && !weaponCollider.enabled)
            {
                Debug.Log("Activating Collider");
                weaponCollider.enabled = true;
            }
        }
        else
        {
            weaponCollider.enabled = false;
            targetsHit.Clear();
            isTargetHit = false;

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
        currentWeapon.GetComponent<Collider>().enabled = false;
        isTargetHit = false;
        hitTargets.Clear();
        currentWeapon.Targets.Clear();
    }

}

