using UnityEngine;

public abstract class FrameData : ScriptableObject
{
    [Header("Frame Data")]
    public int StartUpFrames;
    public int ActiveFrames;
    public int RecoveryFrames;

    public enum WindowType
    {
        Interrupt, 
        Invulnerability,
        None
    }

  
    [System.Serializable]
    public struct FrameWindows
    {
        public int startFrame;
        public int endFrame;
        public WindowType windowType;
    }
}