using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;

public class CameraControllerCinemachine : MonoBehaviour
{
    [Header("Cinemachine Camera components")]
    private CinemachineCamera _cam;
    private CinemachineOrbitalFollow _cam_OrbitalFollow;
    private CinemachineRotationComposer _cam_RotComposer;
    private CinemachineGroupFraming _cam_GroupFraming;
    [SerializeField] private CinemachineTargetGroup _targetGroup;


    [Header("Target Tracking Settings")]
    public bool LockedOn;
    public CinemachineTargetGroup.Target Enemy; 
    private GameObject player;
    private bool hasCameraReset = false;
    [SerializeField] private float LockOnRadius;
    [SerializeField] private LayerMask enemyLayer;

    private List<Collider> availableEnemies;

    void Start()
    {

        _cam = GetComponent<CinemachineCamera>();
        _cam_OrbitalFollow = GetComponent <CinemachineOrbitalFollow>();
        _cam_RotComposer = GetComponent <CinemachineRotationComposer>();
        _cam_GroupFraming = GetComponent<CinemachineGroupFraming>();

        player = FindFirstObjectByType<GameObject>();
        if (_cam == null)
            Debug.LogError("Cinemachine Camera Not Found...");
        
    }

    
    void Update()
    {
        HandleInput();

        if (LockedOn)
            LockOn();

        else if(!LockedOn && !hasCameraReset)
            ResetCamera(Enemy);
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(2))
        {
            Recenter();
        }

        if(Input.GetKeyDown(KeyCode.Q) && canLockOn())
        {
            LockedOn = !LockedOn;
        }
    }

    private void Recenter()
    {

    }
    
    private bool canLockOn()
    {
        availableEnemies = new(Physics.OverlapSphere(player.transform.position, LockOnRadius, enemyLayer));
        return availableEnemies.Count > 0;

    }
    private void LockOn(float targetWeight = 1 , float targetRadius = 1 ) 
    {
        // How do we find who to look at ? 

        // --> Use Camera's Position Vectors to deduce lock on enemy intention 
        // --> Check if lock on intent has box collider / some identifier that shows target for locking on is an enemy
        // --> Check if it's in range 
        // = Find best Candidate



        int maxTargetCap = 2;
        int enemyIndex = 1;


        hasCameraReset = false; 

        Transform lockOnCandidate = BestCandidateForLockOn();

        if (lockOnCandidate == null)
            return; // -- > No one to lock on to 


        CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();


        target.Object = lockOnCandidate;
        target.Weight = targetWeight;
        target.Radius = targetRadius;


        Enemy = target;

        //Debug.Log($"Best Lock On Target : -- > {target.gameObject.name}");
        // --> Cinemachine Camera Motion Update
        // --> Player movement Update ------------> Feed Request into Statemachine to introduce strafin 

        if(!_targetGroup.Targets.Contains(Enemy) && _targetGroup.Targets.Count < maxTargetCap)
        {
            _targetGroup.Targets.Add(Enemy);
        }

        // --> Free index is available 
        _targetGroup.Targets[enemyIndex] = target; 

        if (!_cam_GroupFraming.enabled)
        {
            _cam_GroupFraming.enabled = true;
        }
    }

    private void ResetCamera(CinemachineTargetGroup.Target target)
    {
        int enemyIndex = 1;

        hasCameraReset = true;
        _cam_GroupFraming.enabled = false;

        _targetGroup.Targets[enemyIndex].Object = null;

    }

    private Transform BestCandidateForLockOn()
    {
        Vector3 forward = _cam.transform.forward;

        Transform bestTarget = null;
        float bestDot = 0;


        for (int i = 0; i < availableEnemies.Count; i++)
        {
            {
                Vector3 CamToEnemyDir = (availableEnemies[i].transform.position - _cam.transform.position).normalized;

                float dot = Vector3.Dot(forward.normalized, CamToEnemyDir);

                if (dot < 0)
                {
                    availableEnemies.Remove(availableEnemies[i]);
                    continue;
                }
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestTarget = availableEnemies[i].transform;
                }
            }
        }

        return bestTarget;
    }
}

