using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStateMachines
{

    public class EntityStateMachine
    {
        public State currentState;
        public Type lastState;

        private readonly Dictionary<Type, State> states = new();

        public void Register(State state)
        {
            states[state.GetType()] = state;
        }

        public T GetState<T>() where T : State
        {
            return (T)states[typeof(T)];
        }

        public void SwitchState<T>() where T : State
        {
            SwitchStates(GetState<T>());
        }

        public void SwitchStates(State newState)
        {
            if(currentState != null)
                lastState = currentState.GetType();


            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
    }

    public abstract class State
    {
        [NonSerialized] public GameObject gameObj;
        
        [NonSerialized] public IMovementInput movementInput;
        [NonSerialized] public Camera cam;

        public AnimationClip clip;
        protected EntityStateMachine Emachine;

        [NonSerialized] public CollisionHandlerSystem collisionHandler;

        public struct RotationInfo
        {
            public Vector3 startpos;
            public Vector3 dst;
        }

        public RotationInfo rotationInfo;

        public virtual void Initialize(GameObject obj, EntityStateMachine machine, IMovementInput input, CollisionHandlerSystem collision, Camera cam)
        {
            gameObj = obj;
            Emachine = machine;
            movementInput = input;
            collisionHandler = collision;
            this.cam = cam;
        }


        public abstract void Enter();
        public abstract void HandleInput();
        public abstract void Update();
        public abstract void Exit();
    }


  
}

