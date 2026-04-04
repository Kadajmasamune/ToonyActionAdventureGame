using System;
using System.Collections.Generic;
using UnityEngine;

public class InputBufferer : MonoBehaviour
{
    private Ticker ticker;

    private AttackInput[] AttackHistory;

    public enum AttackInput { Light, Heavy }
    public Queue<AttackInput> AttackBuffer;


    [Header("Ticker Configuration")]
    [SerializeField] private int TicksToReset;



    private int nextResetTick = 0;

    private void Start()
    {
        AttackBuffer = new Queue<AttackInput>();
        if (ticker == null)
        {
            ticker = FindFirstObjectByType<Ticker>();
        }
        else
            Debug.LogError("Ticker Not Found");

        nextResetTick = TicksToReset + ticker.CurrentTick;

    }

    private void Update()
    {
        BufferInput();
        //HandleBufferedInput(); 
        ClearBuffer();
        //Debug.Log(nextResetTick);
    }

    private void BufferInput()
    {
        if (Input.GetMouseButtonDown((int)AttackInput.Light))
        {
            AttackBuffer.Enqueue(AttackInput.Light);
            nextResetTick = ticker.CurrentTick + TicksToReset; //Update

        }
        if (Input.GetMouseButtonDown((int)AttackInput.Heavy))
        {
            AttackBuffer.Enqueue(AttackInput.Heavy);
            nextResetTick = ticker.CurrentTick + TicksToReset; //Update
        }

    }

    //Finish Handling Buffered Input and Decide the Scope
    private void HandleBufferedInput ()
    {
        if (AttackBuffer.Count == 0)
            return;

        AttackInput input = AttackBuffer.Dequeue();

        
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