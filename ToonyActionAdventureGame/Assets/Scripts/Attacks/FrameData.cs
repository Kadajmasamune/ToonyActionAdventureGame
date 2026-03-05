using UnityEngine;

public abstract class FrameData: ScriptableObject
{
    [Header("Frame Data")]
    public int StartUpFrames;
    public int ActiveFrames;
    public int RecoveryFrames;

    public int CancelWindowStartFrame;
    public int CancelWindowEndFrame;

}
