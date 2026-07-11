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

    
    private class Grounded : State
    {
        //idle , walk , sprint 
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }  
    }

    private class Jump : State
    {
        //Handle Double Jumps 

        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Fall : State
    {
        //Handle fast fall 
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }
    private class SideStep : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
 
    }


    private class Dash : State
    {
        // Forward , backward , and diagonal dashes 
        // Jump Dashes too
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class Quick180 : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }    

    private class BackFlip : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }
    
}