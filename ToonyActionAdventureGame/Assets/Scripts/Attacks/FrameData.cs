using UnityEngine;

public abstract class FrameData : ScriptableObject
{
    [Header("Frame Data")]
    public int StartUpFrames;
    public int ActiveFrames;
    public int RecoveryFrames;

    [System.Serializable]
    public struct CancelWindow
    {
        public int startFrame;
        public int endFrame;
    }
}