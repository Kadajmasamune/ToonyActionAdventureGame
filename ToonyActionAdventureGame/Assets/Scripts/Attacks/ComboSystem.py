from enum import Enum
from collections import deque
import pygame

#We Now Want to make some sort of Tree that Defines all the possible paths a combo can take and be merged into
#and Add a Finite State Machine to enforce legal combo moves 

#Buffer Inputs 
#Check for partial Completion
#Back Tracking on Inputs 

#Execute moves and combos 

#Cancellations of Animations as well as blending 

class State(Enum):
    Idle = 0
    Attacking = 1

class Player :
    def __init__(self , startingState : State):
        self.currentState : State = State()

        if self.currentState == None : 
            self.currentState = State.Idle
        self.currentState = startingState

class AttackInput(Enum) : 
    Light = 0 
    Heavy = 1

class ComboSequence: 
    #Each Node will simply be a sequence of Inputs 
    #And then the next possible chains it can connect with 
    def __init__(self , baseCombo):
        self.baseCombo: list[AttackInput] = baseCombo  
        self.nextCombo : list[ComboSequence] = []
        self.frames = 0 

        
    def addCombo(self , nextCombo) -> None:
        return self.nextCombo.append(nextCombo) 

class InputBufferer: 
    def __init__(self):
        self.Buffer : deque = deque()

        self.previousInput : AttackInput = None
        self.currentInput : AttackInput = None

    def bufferInput (self, AttackInput : AttackInput) -> None  :
        return self.Buffer.append(AttackInput)
    
    def flushBuffer(self) -> None :
        if len(self.Buffer) > 0 :
            self.Buffer.pop() 

    def GetCurrentInput(self) :
        """
            The Element that is to be Popped out of the Queue is the current Input.
        """ 
        return
    def GetPreviousInput(self):
        """
            The Element that was previously popped will be deduced as the previous input.
        """

        return    

class ComboTree :
    """
        Essentially, Each Node will simply be a sequence of Attack States , Light or Heavy Attacks.
        This will be a Decision Tree, which will mark all the legal moves a user can make per input
    """    
    def __init__(self , baseCombo):
        self.baseCombo : ComboSequence = baseCombo

    def setRootCombo(self , combo) : 
        self.baseCombo = combo


class ComboTreeProcessor : 
    def __init__(self):
        self.tree : ComboTree = ComboTree()
        #self.connectNodes() # Bake Once.

    def TraverseTree() : 
        return

    def isComboPartiallyComplete () -> bool :
        return False  
    
    def connectNodes() -> None  : 
        return 
    
    def ExecuteMove(combo) -> None: 
        return 

class Pygame:
    """
    Debug the Combo System via UI and actual Mouse Input
    """
    def __init__(self):
        self.width = 800
        self.height = 600
        self.title = "Combo System Test"
        pygame.init()
        self.screen = pygame.display.set_mode((self.width, self.height))
        pygame.display.set_caption(self.title)
        self.clock = pygame.time.Clock()

        self.inputBufferer = InputBufferer()

    def GetAttackInput(self):

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                return "QUIT"

            if event.type == pygame.MOUSEBUTTONDOWN:
                if event.button == 1:  # left click
                    return AttackInput.Light
                elif event.button == 3:  # right click
                    return AttackInput.Heavy
        return None

    def Update(self):
        running = True
        while running:
            attack = self.GetAttackInput()
            if attack == "QUIT":
                break
            if attack:
                # print(attack)
                self.inputBufferer.bufferInput(attack)
                print(self.inputBufferer.Buffer)
                # self.inputBufferer.flushBuffer()

            self.screen.fill((30, 30, 30))  
            pygame.display.flip()
            self.clock.tick(60)  


def main() -> None : 
    window = Pygame()
    window.Update()

if __name__ == "__main__" :
    main()