
using EntityStateMachines;
using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CollisionHandlerSystem))]
[RequireComponent(typeof(RotationComposerSystem))]
public class MovementSystem : MonoBehaviour, IEntitySystem
{

    public EntityStateMachine movementFSM { get; private set; }


    [Header("States")]
    [SerializeField] private States _states;

    public void Init()
    {
        movementFSM = new EntityStateMachine();

        foreach (State state in _states.All)
        {
            state.Initialize(this.gameObject, movementFSM , GetComponent<IMovementInput>() , GetComponent<CollisionHandlerSystem>() , 
                FindFirstObjectByType<Camera>());

            movementFSM.Register(state);
        }

        movementFSM.SwitchStates(_states.groundedState);
    }


    public void Tick()
    {
        movementFSM.currentState.HandleInput();
        movementFSM.currentState.Update();
    }
             
}


[System.Serializable]
public class States
{
    public Grounded groundedState;
    public Jump jumpState;
    public Fall fallState;
    public Dash dashState;
    public Quick180 quick180State;

    public IEnumerable<State> All
    {
        get
        {
            yield return groundedState;
            yield return jumpState;
            yield return fallState;
            yield return dashState;
            yield return quick180State;
        }
    }
}