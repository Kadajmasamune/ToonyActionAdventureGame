using UnityEngine;
using static FrameData;

public class CombatStateMachine : MonoBehaviour // ----> “Given state + input + context → what happens next?”
                                                // ----> This class Decides Transition logic, not Attacks Themselves, they simply consume the signals given by the Machine and perform the exact Transition. 
{
    public Attack[] PossibleAttacks;

    public Attack CurrentAttack;

    public struct AttackRuntimeData
    {
        public int startUp;
        public int active;
        public int recovery;
    }

    public struct CancelWindowRuntime
    {
        public int start;
        public int end;
    }

    public AttackRuntimeData currentAttackData;
    public CancelWindowRuntime[] cancelWindows;

    public void ExecuteAttack()
    {

    }

    public void CancelAttack()
    {

    }


    public bool CanChain()
    {
        return false;
    }

    public bool IsInCancelWindow(int tick)
    {
        for (int i = 0; i < cancelWindows.Length; i++)
        {
            if (tick >= cancelWindows[i].start && tick <= cancelWindows[i].end)
                return true;
        }
        return false;

    }

}


