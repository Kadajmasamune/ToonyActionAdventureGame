from common import *
from Combos import * 
import pygame

#We Now Want to make some sort of Tree that Defines all the possible paths a combo can take and be merged into
#and Add a Finite State Machine to enforce legal combo moves 

#Buffer Inputs 
#Check for partial Completion
#Back Tracking on Inputs 

#Execute moves and combos 

#Cancellations of Animations as well as blending 


class Player :
    def __init__(self , startingState : State):
        self.currentState : State = State()

        if self.currentState == None : 
            self.currentState = State.Idle
        self.currentState = startingState



class InputBufferer: 
    def __init__(self):
        self.Buffer : deque = deque()

        self.previousInput : AttackInput = None
        self.currentInput : AttackInput = None



    def bufferInput (self, AttackInput : AttackInput) -> None  :
        return self.Buffer.append(AttackInput)
    

    def flushBuffer(self) -> None :
        return self.Buffer.pop if (len(self.Buffer) > 0) else None 


    def GetCurrentInput(self) :
        """
            The Element that is to be Popped out of the Queue is the current Input.
        """ 
        # return self.Buffer[-1] if (len(self.Buffer) > 0) else None
        return self.Buffer[0] if (len(self.Buffer) > 0) else None 


    def GetPreviousInput(self):
        """
            The Element that was previously popped will be deduced as the previous input.
        """
        # return self.Buffer[-2] if (len(self.Buffer) >= 1) else None
        return self.Buffer[1] if (len(self.Buffer) >= 1) else None 


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
        self.deltaTime  = 0 
        self.currentTime = 0 

        self.attackTime = ATTACK_TIME
        self.comboTime = COMBO_TIME

        #Attack System :
        self.inputBufferer = InputBufferer()
        
        #Root Combo
        self.light1 = AttackNode(AttackInput.Light)



        self.light2 = AttackNode(AttackInput.Light)
        self.heavy1 = AttackNode(AttackInput.Heavy)
        self.heavy2 = AttackNode(AttackInput.Heavy)

        self.light1.addCombo(self.light2)
        self.light1.addCombo(self.heavy1)
        self.heavy1.addCombo(self.heavy2)


    def initAttackSystem(self):
        return
    

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
            self.deltaTime = self.clock.tick(60) / 1000
            self.currentTime = pygame.time.get_ticks()

            attack = self.GetAttackInput()

            if attack == "QUIT":
                break
            
            if attack:
                # print(attack)
                
                self.inputBufferer.bufferInput(attack)
                
                
                # print(self.inputBufferer.Buffer)
                # self.inputBufferer.flushBuffer()



            self.screen.fill((30, 30, 30))  
            pygame.display.flip()
            self.clock.tick(60)  




def main() -> None : 
    window = Pygame()
    window.Update()

if __name__ == "__main__" :
    main()