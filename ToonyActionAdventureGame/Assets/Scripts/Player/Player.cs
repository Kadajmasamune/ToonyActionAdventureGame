using EntityStateMachines;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimatorController))]
public class Player : MonoBehaviour, Iplayer , ICombatHandler 
{


    [Header("Rotation")]
    public float rotationSharpness = 25f;

    [Header("References")]
    public Transform cameraTransform;



    


    public Health Health { get; private set; }
    public AnimatorController Animator { get; set; }
    public EntityStateMachine<Player> entityStateMachine { get; private set; }
    public CombatStateMachine combatStateMachine { get; private set; }

    public CameraControllerCinemachine cinCam;

    void Awake()
    {
        cinCam = FindFirstObjectByType<CameraControllerCinemachine>();
        Animator = GetComponent<AnimatorController>();
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
        playerWeaponControler = GetComponent<WeaponController>();
        combatStateMachine = GetComponent<CombatStateMachine>();
        combatStateMachine.Initialize(this);
    }

    void Start()
    {
        entityStateMachine = new EntityStateMachine<Player>(this);
        entityStateMachine.SwitchStates(new GroundedState());
    }

    void Update()
    { 

        entityStateMachine.currentState.HandleInput(this);
        entityStateMachine.currentState.Update(this);
        //playerStateMachine.currentState.Exit(this);
    }
}
