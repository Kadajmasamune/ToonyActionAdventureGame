using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStateMachines
{

    public class EntityStateMachine
    {
        public State currentState;
        public object context;

        public EntityStateMachine(object context)
        {
            this.context = context;
        }

        public void SwitchStates(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void HandleInput()
        {
            currentState?.HandleInput();
        }

        public void Update()
        {
            currentState?.Update();
        }
    }

    
    public abstract class State
    {
        public GameObject gameObj;
        public Collider Collider;
        public EntityStateMachine Emachine;

        public IMovementInput movementInput;
        public Camera cam;

        public abstract void Enter();
        public abstract void HandleInput();
        public abstract void Update();
        public abstract void Exit();
    }


  
}

