using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    
}


public class StateMachine
{
    private Dictionary<string, State> states;
    private State currentState;
    
    /**
     *  Create state machine
     *
     *  @return new state machine
     */
    public static StateMachine Create()
    {
        var s = new StateMachine();
        return s;
    }

    /**
     *  Find state by id and return casted state
     *
     *  @return casted state
     */
    //template<class T>
    public T FindState<T>() where T : State
    {
        string name = typeof(T).Name;
        if (states.ContainsKey(name))
        {
            return states[name] as T;
        }
        return default(T);
    }

    /**
     *  Add new state to state machine
     *
     *  @param args arguments to pass to constructor of state
     */
    //template<typename T, class... Args>
    public void AddState<T>() where T : State, new()
    {
        var typeId = typeof(T).Name;

        var state = new T();
        state.SetStateMachine(this);

        if (states == null)
            states = new Dictionary<string, State>();

        states.Add(typeId, state);
    }

    /**
     *  Check if we can enter state
     *
     *  @return true if this state is valid, false otherwise
     */
    //template<typename T>
    bool CanEnterState<T>() where T : State
    {
        if (currentState == null)
        {
            return true;
        }
        else
        {
            var state = FindState<T>() as State;
            if (state != null)
            {
                return currentState.IsValidNextState(state);
            }
        }
        return false;
    }

    /**
     *  Enters new state
     *
     *  Before entering new state old state will check if it is a valid state to execute
     *  transaction
     *
     *  Order of execution:
     *
     *  willExitWithNextState will be called on current state
     *  didEnterWithPreviousState will be called on new state
     *
     *  @return true if entered, false otherwise
     */
    //template<typename T>
    public bool EnterState<T>() where T : State
    {
        var state = FindState<T>() as State;
        if (state != null)
        {
            if (currentState == null)
            {
                currentState = state;
                currentState.DidEnterWithPreviousState(null);
                return true;
            }
            else
            {
                if (currentState.IsValidNextState(state))
                {
                    currentState.WillExitWithNextState(state);
                    state.DidEnterWithPreviousState(currentState);
                    currentState = state;
                    return true;
                }
            }
        }
        return false;
    }

    /**
     *  Enters new state without any check if next state is valid
     *
     *
     *  Order of execution:
     *
     *  willExitWithNextState will be called on current state
     *  didEnterWithPreviousState will be called on new state
     *
     *  @return true if entered, false otherwise
     */
    //template<typename T>
    public bool SetState<T>() where T : State
    {
        var state = FindState<T>() as State;
        var previousState = currentState == null ? "NULL" : currentState.GetStateType();

        if (previousState == state.GetStateType())
            return false;

        if (state != null)
        {
            if (currentState == null)
            {
                currentState = state;
                currentState.DidEnterWithPreviousState(null);
                return true;
            }
            else
            {
                currentState.WillExitWithNextState(state);
                state.DidEnterWithPreviousState(currentState);
                currentState = state;
                return true;
            }
        }
        return false;
    }

    /**
     *  Update state machine delta time, this will call updateWithDeltaTime on current state
     *
     *  @param delta delta time
     */
    public void UpdateWithDeltaTime()
    {
        if (currentState != null)
        {
            currentState.UpdateState();
        }
    }

    /**
     *  Get current state
     *
     *  @return current state
     */
    public State GetState()
    {
        return currentState;
    }

    ~StateMachine()
    {
        currentState = null;
    }

}

public abstract class State
{
    /**
     *  Called when entering state
     *
     *  @param previousState previous state or null if this is the first state
     */
    public virtual void DidEnterWithPreviousState(State previousState) { }
    /**
     *  Called every frame by state machine
     *
     *  @param delta time
     */
    public virtual void UpdateState() { }
    /**
     *  Checks if next state is valid for transition
     *
     *  @param state next state
     *
     *  @return true if valid, false otherwise
     */
    public virtual bool IsValidNextState(State state) { return false; }
    /**
     *  Called when exiting current state
     *
     *  @param nextState next state
     */
    public virtual void WillExitWithNextState(State nextState) { }

    public virtual string GetStateType() { return ""; }

    /**
     *  Get state machine
     *
     *  @return state machine
     */
    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    /**
     *  Set State machine, this will be set when state has been added to state machine
     *
     *  @param stateMachine parent state machine
     */
    public void SetStateMachine(StateMachine sm)
    {
        stateMachine = sm;
    }

    protected StateMachine stateMachine;
}


public class PlayerIdle : State
{
    PlayerStateMachine playerStateMachine;
    PlayerController player;

    void Init()
    {
        playerStateMachine = stateMachine as PlayerStateMachine;
        player = playerStateMachine.player;
    }

    public override void DidEnterWithPreviousState(State previousState)
    {

        if (playerStateMachine == null)
            Init();

        if (StageManager.IsStageFailed)
            return;

        if (StageManager.IsStageComplete)
            return;

        if (player.playerMovement.GotMovementInput)
            playerStateMachine.SetState<PlayerFlying>();
        else
            player.SetPlayerAnimation(PlayerController.PlayerAnimation.Idle);


    }

    public override void UpdateState()
    {

        if (player.playerMovement.GotMovementInput)
            playerStateMachine.SetState<PlayerFlying>();
    }

    public override string GetStateType()
    {
        return "Idle";
    }
}

public class PlayerStateMachine : StateMachine
{
    public PlayerController player;

    PlayerStateMachine(PlayerController p)
    {
        player = p;
    }

    public static PlayerStateMachine Create(PlayerController p)
    {
        PlayerStateMachine psm = new PlayerStateMachine(p);

        return psm;
    }
}