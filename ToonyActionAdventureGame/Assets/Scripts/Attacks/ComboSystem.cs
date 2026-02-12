using System.Collections.Generic;
using UnityEngine;

namespace ComboSystem
{
    public enum AttackInput
    {
        Light,
        Heavy
    }

    public class AttackNode
    {
        public AttackInput Input;
        public int StartUpFrames;
        public int ActiveFrames;
        public int RecoveryFrames;

        public int CancelStart;
        public int CancelEnd;

        public List<AttackInput> Transition;

        public AttackNode(
            AttackInput input,
            int startUpFrames,
            int activeFrames,
            int recoveryFrames,
            int cancelStart,
            int cancelEnd,
            List<AttackInput> transition)
        {
            Input = input;
            StartUpFrames = startUpFrames;
            ActiveFrames = activeFrames;
            RecoveryFrames = recoveryFrames;
            CancelStart = cancelStart;
            CancelEnd = cancelEnd;
            Transition = transition ?? new List<AttackInput>();
        }

        public bool CanCancel()
        {
            return false;
        }

        public void AddCombo(AttackInput nextCombo)
        {
            Transition.Add(nextCombo);
        }
    }

    public class ComboTree
    {
        public AttackNode BaseCombo;

        public void SetRootCombo(AttackNode combo)
        {
            BaseCombo = combo;
        }
    }

    public class ComboTreeProcessor
    {
        private ComboTree tree;
        private AttackNode[] attackHistory; // Make this into a circular Buffer

        public ComboTreeProcessor()
        {
            tree = new ComboTree();
        }

        public void ResetCombo()
        {
        }

        public bool IsComboPartiallyComplete()
        {
            return false;
        }

        public void ConnectNodes()
        {
        }

        public void ExecuteMove(AttackNode attack)
        {
        }
    }

    public class InputBufferer
    {
        public Queue<AttackInput> Buffer = new Queue<AttackInput>();
        private AttackInput PreviousInput;
        private AttackInput CurrentInput;

        public void BufferInput(AttackInput input)
        {
            Buffer.Enqueue(input);
        }

        public void ClearBuffer()
        {
            Buffer.Clear();
        }

        public AttackInput GetCurrentInput()
        {
            return Buffer.Dequeue();
        }

        public AttackInput GetPreviousInput()
        {
            return Buffer.Dequeue();
        }
    }
}
