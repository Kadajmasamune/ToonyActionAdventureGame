using System;
using System.Collections.Generic;
using UnityEngine;

public class InputBufferer : MonoBehaviour
{


    public enum AttackInput { Light, Heavy }

    public Queue<AttackInput> AttackBuffer;

    public AttackInput attackInput;

    private AttackInput[] AttackHistory;


    [Header("Ticker Configuration")]
    private Ticker ticker;
    [SerializeField] private int TicksToReset;
    private int nextResetTick = 0;


    private void Start()
    {
        AttackBuffer = new Queue<AttackInput>();
        ticker = FindFirstObjectByType<Ticker>();

        if (ticker == null)
            Debug.LogError("Ticker Not Found");
        

        nextResetTick = TicksToReset + ticker.CurrentTick;

    }

    private void Update()
    {
        BufferInput();
        HandleBufferer();
        ClearBuffer();

        //Debug.Log(nextResetTick);
    }

    private void BufferInput()
    {
        if (Input.GetMouseButtonDown((int)AttackInput.Light))
        {
            AttackBuffer.Enqueue(AttackInput.Light);
            nextResetTick = ticker.CurrentTick + TicksToReset; //Update
            return;

        }
        if (Input.GetMouseButtonDown((int)AttackInput.Heavy))
        {
            AttackBuffer.Enqueue(AttackInput.Heavy);
            nextResetTick = ticker.CurrentTick + TicksToReset; //Update
            return;
        }

    }


    private void HandleBufferer()
    {
        if (AttackBuffer.Count == 0)
            return;

        attackInput = AttackBuffer.Peek();
    }


    private void ClearBuffer()
    {
        if (AttackBuffer.Count == 0)
            return;
        if (ticker.CurrentTick >= nextResetTick)
        { 
            Debug.Log($"Buffer Cleared at Tick : {ticker.CurrentTick}");
            AttackBuffer.Clear();
        }
        else
            return;
    }
}