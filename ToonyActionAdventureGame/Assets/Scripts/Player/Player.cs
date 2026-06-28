//using EntityStateMachines;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(AnimatorController))]
//public class Player : MonoBehaviour , ICombatHandler 
//{


//    [Header("Rotation")]
//    public float rotationSharpness = 25f;

//    [Header("References")]
//    public Transform cameraTransform;



//    public Health Health { get; private set; }
//    public AnimatorController Animator { get; set; }
//    public EntityStateMachine entityStateMachine { get; private set; }
//    public CombatStateMachine combatStateMachine { get; private set; }

//    public CameraControllerCinemachine cinCam;


//    void Awake()
//    {
//        //playerWeaponControler = GetComponent<WeaponController>();
//        cinCam = FindFirstObjectByType<CameraControllerCinemachine>();
//        Animator = GetComponent<AnimatorController>();
//        if (!cameraTransform)
//            cameraTransform = Camera.main.transform;
//        combatStateMachine = GetComponent<CombatStateMachine>();
//        combatStateMachine.Initialize(this);
//    }

//    void Start()
//    {
//        entityStateMachine = new EntityStateMachine(this);
//        entityStateMachine.SwitchStates(new GroundedState());
//    }

//    private void OnEnable()
//    {
//        Ticker.OnTick += Tick;
//    }


//    private void OnDisable()
//    {
//        Ticker.OnTick -= Tick;
//    }



//    private void Tick()
//    {
//        entityStateMachine.currentState.HandleInput(this);
//        entityStateMachine.currentState.Update(this);
//        //playerStateMachine.currentState.Exit(this);
//    }
//}