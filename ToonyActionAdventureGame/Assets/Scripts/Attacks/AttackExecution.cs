using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FrameData;

public class AttackExecution : MonoBehaviour
{
    private Attack currentAttack;
    private IWeapon weaponHandler;
    private AnimatorController animator;

    private HashSet<Collider> hitTargets = new();

    private int attackStartTick;


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


    public bool IsExecuting { get; private set; }
    public bool IsTransitioning { get; set; }


    public int CurrentTick
    {
        get
        {
            return Ticker.instance.CurrentTick - attackStartTick;
        }
    }


    public Attack CurrentAttack => currentAttack;


    public void Initialize(IWeapon weapon, AnimatorController anim)
    {
        weaponHandler = weapon;
        animator = anim;
    }


    private void OnEnable()
    {
        Ticker.OnTick += Tick;
    }


    private void OnDisable()
    {
        Ticker.OnTick -= Tick;
    }



    public void StartAttack(Attack attack, bool transition = false)
    {
        currentAttack = attack;

        IsExecuting = true;
        IsTransitioning = transition;

        attackStartTick = Ticker.instance.CurrentTick;

        hitTargets.Clear();

        weaponHandler.DisableHitbox();
        weaponHandler.ClearTargets();

        int hash = Animator.StringToHash(attack.clip.name );

        animator.PlayAttack(hash, attack.AttackIndexLayer, transition );
    }


    private void Tick()
    {
        if (!IsExecuting)
            return;


        UpdateImpacts();
        int tick = CurrentTick;


        UpdateWeapon(tick);
        ProcessHits();

        if (tick >= TotalFrames())
        {
            End();
        }
    }



    private int TotalFrames()
    {
        return
            currentAttack.StartUpFrames +
            currentAttack.ActiveFrames +
            currentAttack.RecoveryFrames;
    }


    private void UpdateWeapon(int tick)
    {
        int start = currentAttack.StartUpFrames;
        int end = start + currentAttack.ActiveFrames;

        if (tick >= start && tick <= end)
            weaponHandler.EnableHitbox();
        else
            weaponHandler.DisableHitbox();
    }




    private void ProcessHits()
    {
        foreach (Collider target in weaponHandler.Targets)
        {
            if (hitTargets.Contains(target))
                continue;

            hitTargets.Add(target);


            Health health =  target.GetComponent<Health>();
            if (health)
                health.TakeDamage(currentAttack.Damage);


            if (currentAttack.upwardLaunchImpulse > 0)
                Launch(target);
            else
                KnockBack(target);

            StartCoroutine(HitStop());
        }
    }



    private void Launch(Collider target)
    {
        Vector3 start = target.transform.position;
        Vector3 end = start + Vector3.up * currentAttack.upwardLaunchImpulse;

        impacts[target] = new ImpactData
        {
            start = start,
            end = end,
            duration = currentAttack.attackSpeed,
            curve = currentAttack.lungeCurve
        };
    }




    private void KnockBack(Collider target)
    {
        Vector3 start = target.transform.position;
        Vector3 dir = (target.transform.position - transform.position).normalized;

        impacts[target] = new ImpactData
        {
            start = start,
            end = start + dir * currentAttack.knockBackInflicted,
            duration =currentAttack.attackSpeed,
            curve = currentAttack.lungeCurve
        };
    }


    public WindowType DeduceCurrentWindow(int tick, Attack currentAttack)
    { 
        if (currentAttack == null)
            return WindowType.None; 
        for (int i = 0; i < currentAttack.FrameWindows.Length; i++) 
        {
            if (currentAttack.FrameWindows[i].windowType == WindowType.Interrupt && 
                (tick >= currentAttack.FrameWindows[i].startFrame && tick <= currentAttack.FrameWindows[i].endFrame))
                return WindowType.Interrupt; 

            else if (currentAttack.FrameWindows[i].windowType == WindowType.Invulnerability && 
                (tick >= currentAttack.FrameWindows[i].startFrame && tick <= currentAttack.FrameWindows[i].endFrame)) 
                return WindowType.Invulnerability; 

        } 
        return WindowType.None; 
    
    }


    private void UpdateImpacts()
    {
        finishedImpacts.Clear();

        foreach (var pair in impacts)
        {
            pair.Value.timer += Time.deltaTime;

            float t = Mathf.Clamp01(pair.Value.timer / pair.Value.duration );

            float eased = pair.Value.curve.Evaluate(t);


            if (pair.Key)
            {
                pair.Key.transform.position =
                    Vector3.Lerp( pair.Value.start, pair.Value.end, eased);
            }


            if (pair.Value.timer >= pair.Value.duration)
                finishedImpacts.Add(pair.Key);
        }

        foreach (var c in finishedImpacts)
            impacts.Remove(c);
    }



    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(currentAttack.hitStopDuration);
        Time.timeScale = 1f;
    }


    public void End()
    {
        int hash =Animator.StringToHash(currentAttack.clip.name);

        animator.StopAttack(hash);

        weaponHandler.DisableHitbox();
        weaponHandler.ClearTargets();

        hitTargets.Clear();

        currentAttack = null;

        IsExecuting = false;
        IsTransitioning = false;
    }
}