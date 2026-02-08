from enum import Enum

class AttackInput(Enum) : 
    Light = 0 
    Heavy = 1

class ComboSequence:
    def __init__(self , value : list[AttackInput]):
        self.combos = value
        self.nextCombos : list[ComboSequence] = []

    def addCombo(self , combo) -> None : 
        return self.nextCombos.append(combo)

class ComboTree:
    """
        In Memory, this is simply a list of ComboSequences Pointing to different Values
    """ 
    def __init__(self , baseCombo=None):
        self.baseCombo : ComboSequence = baseCombo

    def setRoot(self , combo): 
        self.baseCombo = combo

BaseCombo = ComboSequence([AttackInput.Light , AttackInput.Light, AttackInput.Heavy])
Combosys = ComboTree(BaseCombo)