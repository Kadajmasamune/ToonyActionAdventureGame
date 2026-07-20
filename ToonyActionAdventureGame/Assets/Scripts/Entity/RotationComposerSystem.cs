using EntityStateMachines;
using System;
using UnityEngine;

public class RotationComposerSystem : MonoBehaviour, IEntitySystem
{

    private MovementSystem movSys;

    private State currentState;

    [SerializeField] private float rotationSharpness = 600;


    private void Awake()
    {
        movSys = GetComponent<MovementSystem>();
    }

    public void Init()
    {
        currentState = movSys.movementFSM.currentState;
    }

    public void Tick()
    {
        updateRotation(currentState.rotationInfo.startpos , currentState.rotationInfo.dst, rotationSharpness);
    }


    public void updateRotation(Vector3 startPos, Vector3 dst , float rotationSharpness)
    {
        if (startPos == null || dst == null || rotationSharpness == 0)
            return;

        Vector3 movementDirection = (dst - startPos).normalized;

        if (movementDirection.sqrMagnitude < 0.0001f)
            return;


        float angle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle * rotationSharpness * Ticker.deltaTick, 0);

        //Debug.Log($"Movement Vector : {movementDirection}");
        //Debug.Log($"Target rot : {targetRotation.eulerAngles}");
    }
}
