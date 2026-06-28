using UnityEngine;
using EntityStateMachines;

public class MovementSystem : MonoBehaviour , IEntitySystem 
{
    private EntityStateMachine movementFSM;

    public void Init() 
    {
        movementFSM = new EntityStateMachine(this);
    }


    public void Update() 
    {
        movementFSM.currentState.HandleInput();
        movementFSM.currentState.Update();
    }


    private class Idle : State 
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Walk : State 
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Sprint : State 
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Jump : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Fall : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }
}