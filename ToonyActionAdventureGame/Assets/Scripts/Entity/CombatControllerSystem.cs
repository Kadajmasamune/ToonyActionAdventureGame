using UnityEngine;
using EntityStateMachines;
public class CombatControllerSystem : MonoBehaviour ,  IEntitySystem
{

    private EntityStateMachine combatControllerFSM; 

    public void Init ()
    {
        combatControllerFSM = new EntityStateMachine(this);
    }

    public void Update()
    {
        combatControllerFSM.currentState.HandleInput();
        combatControllerFSM.currentState.Update();
    }


    private class GroundAttackState : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }

    }

    private class AirAttackState : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }

    private class LockOnState : State
    {
        public override void Enter() { }
        public override void HandleInput() { }
        public override void Update() { }
        public override void Exit() { }
    }
}

