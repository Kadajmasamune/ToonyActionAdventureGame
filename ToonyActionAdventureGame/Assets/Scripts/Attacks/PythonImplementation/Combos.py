from common import AttackInput

import sys
sys.stdout.reconfigure(encoding='utf-8')


ATTACK_TIME = 0.25
COMBO_TIME = 0.32


class ComboTree :
    """
        Essentially, Each Node will simply be a sequence of Attack States , Light or Heavy Attacks.
        This will be a Decision Tree, which will mark all the legal moves a user can make per input
    """    
    def __init__(self , baseCombo=None):
        self.baseCombo : AttackNode = baseCombo

    def setRootCombo(self , combo) : 
        self.baseCombo = combo


class ComboTreeProcessor : 

    """
        State Machine for the combos 
    """
    def __init__(self ):
        self.tree : ComboTree = ComboTree()
        self.connectNodes() # Bake Once.

    def resetCombo(self) -> None : 

        return 

    def isComboPartiallyComplete (self) -> bool :
        """
            A combo is only partially complete if at least one or more inputs of the combo sequence have been inputted by the user
            in a given combo time or a set interval of frames 

            If Combo is partially complete : we return to the default state , which is idle.
        """

        return False  
    
    def connectNodes(self) -> None  : 
        return 
    
    def ExecuteMove(combo) -> None: 
        return 


class AttackNode: 
    #Each Node will simply be a sequence of Inputs 
    #And then the next possible chains it can connect with 
    def __init__(self , combo ):
        self.input : AttackInput = combo 


        self.startup_frames = 0
        self.active_frames = 0
        self.recovery_frames = 0

        # Cancel Window
        self.cancel_start = 0
        self.cancel_end = 0

        self.transitions : list[AttackNode] = []

        
    def addCombo(self , nextCombo) -> None:
        """
            The next combo that The current combo can transition into will be added from here  : 
        """
        return self.transitions.append(nextCombo) 


#Example : 
# light1 = AttackNode(AttackInput.Light)
# Heavy1 = AttackNode(AttackInput.Heavy)

# light1.addCombo(Heavy1)

def print_tree(root):
    visited = set()

    def recurse(node, prefix="", is_last=True):
        if id(node) in visited:
            print(prefix + ("└── " if is_last else "├── ") + f"{node.input.name} (loop)")
            return

        visited.add(id(node))

        connector = "└── " if is_last else "├── "
        print(prefix + connector + node.input.name)

        children = node.transitions
        new_prefix = prefix + ("    " if is_last else "│   ")

        for i, child in enumerate(children):
            is_child_last = i == len(children) - 1
            recurse(child, new_prefix, is_child_last)

    recurse(root)



# print_tree(light1)