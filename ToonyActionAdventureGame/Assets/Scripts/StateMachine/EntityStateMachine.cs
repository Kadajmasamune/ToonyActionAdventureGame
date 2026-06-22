


namespace EntityStateMachines
{

    public class EntityStateMachine<T>
    {
        public State<T> currentState;
        public T context;

        public EntityStateMachine(T context)
        {
            this.context = context;
        }

        public void SwitchStates(State<T> newState)
        {
            currentState?.Exit(context);
            currentState = newState;
            currentState.Enter(context);
        }

        public void HandleInput()
        {
            currentState?.HandleInput(context);
        }

        public void Update()
        {
            currentState?.Update(context);
        }
    }

    
    public abstract class State<T>
    {
        public abstract void Enter(T context);
        public abstract void HandleInput(T context);
        public abstract void Update(T context);
        public abstract void Exit(T context);
    }


  
}

