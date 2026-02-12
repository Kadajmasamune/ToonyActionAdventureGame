from enum import Enum
from collections import deque
from abc import ABC, abstractmethod


class State(Enum):
    Idle = 0
    Attacking = 1

class AttackInput(Enum) : 
    Light = 0 
    Heavy = 1
