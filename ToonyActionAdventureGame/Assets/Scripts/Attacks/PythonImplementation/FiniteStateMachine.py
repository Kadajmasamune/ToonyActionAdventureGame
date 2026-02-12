from enum import Enum 
from abc import ABC , abstractmethod


class State(ABC) :
    def __init__(self , machine ):
        self.machine = machine 

    @abstractmethod
    def Enter():
        pass

    @abstractmethod
    def Update():
        pass    

    @abstractmethod
    def Exit():
        pass


class StateMachine() : 
    def __init__(self , startingState : State):
        self.currentState = startingState
        self.currentState.Enter()

    def ChangeState(self , newState):
        if self.currentState == newState :
            return 
        
        self.currentState = newState

    def Update(self):   
        self.currentState.Update()


class IdleState(State):
    def __init__(self, machine ):
        super().__init__(machine)

    def Update(self):
        key = input("Enter a Key") 
        if key == "w":
            self.machine.Update()