using UnityEngine;
using System.Collections.Generic;

public class Ticker : MonoBehaviour
{
    private const float tickRate = 1f / 60f;
    private float tickTimer;

    public Attack[] Attacks;

    private Dictionary<InputBufferer.AttackInput, Attack> attackData;

    private InputBufferer inputBufferer;
    private Player player;

    private Attack currentAttack;

    private int attackFrame;
    private int comboIndex;

    private int comboResetTimer;
    private const int comboResetTicks = 90; // 1.5 seconds at 60fps

    private void Start()
    {
        inputBufferer = GetComponent<InputBufferer>();
        player = FindFirstObjectByType<Player>();

        attackData = new Dictionary<InputBufferer.AttackInput, Attack>();

        foreach (var atk in Attacks)
        {
            attackData[atk.RequiredInput] = atk;
        }
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;

        while (tickTimer >= tickRate)
        {
            tickTimer -= tickRate;
            CombatTick();
        }
    }

    private void CombatTick()
    {
        comboResetTimer++;

        if (comboResetTimer >= comboResetTicks)
        {
            comboIndex = 0;
        }

        HandleBufferedInput();

        if (currentAttack != null)
        {
            attackFrame++;

            int totalFrames =
                currentAttack.StartUpFrames +
                currentAttack.ActiveFrames +
                currentAttack.RecoveryFrames;

            if (attackFrame >= totalFrames)
            {
                currentAttack = null;
                attackFrame = 0;
            }
        }
    }

    private void HandleBufferedInput()
    {
        if (inputBufferer.Buffer.Count == 0)
            return;

        var input = inputBufferer.Buffer.Peek();

        if (!attackData.TryGetValue(input, out Attack attack))
        {
            inputBufferer.Buffer.Dequeue();
            return;
        }

        if (currentAttack == null)
        {
            StartAttack(attack);
            inputBufferer.Buffer.Dequeue();
            return;
        }

        if (IsInCancelWindow(currentAttack))
        {
            StartAttack(attack);
            inputBufferer.Buffer.Dequeue();
        }
    }

    private bool IsInCancelWindow(Attack attack)
    {
        return attackFrame >= attack.CancelWindowStartFrame &&
               attackFrame <= attack.CancelWindowEndFrame;
    }

    private void StartAttack(Attack attack)
    {
        currentAttack = attack;
        attackFrame = 0;

        comboResetTimer = 0;

        player.Animator.animator.SetInteger("AttackIndex", comboIndex);
        player.Animator.animator.SetTrigger("AttackTrigger");

        comboIndex++;
    }
}