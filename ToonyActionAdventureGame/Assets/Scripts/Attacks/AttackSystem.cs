using UnityEngine;
using System.Collections.Generic;

//Decide the Scope for this class
public class AttackSystem : MonoBehaviour
{

    private Ticker ticker;
    private InputBufferer inputBufferer;

    private Attack currentAttack;
    private int currentAttackTick;

    public enum FrameDataTypes
    {
        StartUpFrames,
        ActiveFrames,
        RecoveryFrames,

        CancelWindowStartFramesEnter,
        CancelWindowStartFramesExit,

        CancelWindowEndFramesEnter,
        CancelWindowEndFramesExit
    }

    private void Start()
    {
        ticker = FindFirstObjectByType<Ticker>();
        inputBufferer = GetComponent<InputBufferer>();
    }
    private void Update()
    {

    }

    private bool isAttackInCancelWindow(Attack attack, int currentAttackTick)
    {
        if (attack == null)
            Debug.LogError("Failed to Check for Cancel Window");

        int[] ScaledFrameData = scaleAttackFrameData(attack);

        bool inFirstWindow = (currentAttackTick >= ScaledFrameData[(int)FrameDataTypes.CancelWindowStartFramesEnter] && currentAttackTick <= ScaledFrameData[(int)FrameDataTypes.CancelWindowStartFramesExit]);
        bool inSecondWindow = (currentAttackTick >= ScaledFrameData[(int)FrameDataTypes.CancelWindowEndFramesEnter] && currentAttackTick <= ScaledFrameData[(int)FrameDataTypes.CancelWindowEndFramesExit]);


        return inFirstWindow || inSecondWindow;
    }
    
    //Improve Accuracy 
    //(int) Explicit conversion can reduce precision and cause Attacks to end 1 animation late/early
    private int scaleFramesToTickRate(AnimationClip clip, int frames , int TargetFrameRate = 60)
    {
        float ScaleFactor = TargetFrameRate / clip.frameRate; //Scale Factor
        int ScaledFrames = 0;
        
        ScaledFrames = (int)(frames * ScaleFactor);
        return ScaledFrames; //Ensures 1 Tick = 1 Frame for all animations.
    }
   
    
    private int[] scaleAttackFrameData (Attack attack)
    {
        int[] FrameData = {attack.StartUpFrames , attack.ActiveFrames , attack.RecoveryFrames ,
                attack.CancelWindowStartFrames[(int)FrameDataTypes.CancelWindowStartFramesEnter],
                attack.CancelWindowStartFrames[(int)FrameDataTypes.CancelWindowStartFramesExit],

                attack.CancelWindowEndFrames[(int)FrameDataTypes.CancelWindowEndFramesEnter],
                attack.CancelWindowEndFrames[(int)FrameDataTypes.CancelWindowEndFramesExit]
        }; 

        int[] ScaledFrameData = new int[FrameData.Length];
        for (int i = 0; i < FrameData.Length; i++)
        {
            int Frame = scaleFramesToTickRate(attack.clip, FrameData[i]);
            ScaledFrameData[i] = Frame;
        }

        Debug.Log($"Scaled Frames : {FrameData} to ----> {ScaledFrameData}");

        return ScaledFrameData;
    }
}
