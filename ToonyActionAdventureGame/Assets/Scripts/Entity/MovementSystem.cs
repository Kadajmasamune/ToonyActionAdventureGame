using UnityEngine;
using EntityStateMachines;
using UnityEngine.InputSystem;
using System;
using NUnit.Framework;
using System.Collections.Generic;

public class MovementSystem : MonoBehaviour, IEntitySystem
{

    private EntityStateMachine movementFSM;

    

    [Header("States")]
    [SerializeField] private Grounded groundedState;
    [SerializeField] private Jump jumpState;
    [SerializeField] private Fall fallState;

    private List<State> states;

    public void Init()
    {
        movementFSM = new EntityStateMachine(this);
        states = new List<State>();

        groundedState.jumpState = jumpState;
        jumpState.fallState = fallState;
        fallState.groundState = groundedState;

        states.Add(groundedState);
        states.Add(jumpState);
        states.Add(fallState);


        foreach (State state in states)
        {
            state.gameObj = this.gameObject;
            state.Collider = this.gameObject.GetComponent<Collider>();
            state.Emachine = this.movementFSM;
            state.movementInput = this.gameObject.GetComponent<IMovementInput>();
            state.cam = FindFirstObjectByType<Camera>();
        }

        movementFSM.SwitchStates(groundedState);
    }


    public void Update()
    {
        movementFSM.currentState.HandleInput();
        movementFSM.currentState.Update();
    }
             
}
