using System;
using System.Collections.Generic;
using UnityEngine; 

public class InputBufferer : MonoBehaviour
{
    public enum AttackInput { Light, Heavy }

    private AttackInput[] AttackHistory;

    public Queue<AttackInput> Buffer;



    private void Start()
    {
        Buffer = new Queue<AttackInput>();
    }


    public void ClearBuffer()
    {

    }
}