using EntityStateMachines;
using UnityEngine; 

public interface IEntity<T>
{
    public Health Health {get; }     
    public Vector3 Velocity { get; }
    public Vector2 Input { get;  }

    public AnimatorController Animator { get; }
    public EntityStateMachine<T> entityStateMachine { get; }
    public CombatStateMachine combatStateMachine { get; }
}