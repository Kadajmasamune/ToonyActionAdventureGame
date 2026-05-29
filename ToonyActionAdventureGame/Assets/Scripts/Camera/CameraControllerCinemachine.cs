using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
public class CameraControllerCinemachine : MonoBehaviour
{
    private CinemachineCamera _cam;

    [SerializeField] private float LockOnRadius;

    private Transform LockOnTarget;
    private Player player;

    private bool LockedOn; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_cam == null)
            Debug.LogError("Cinemachine Camera Not Found...");

        _cam = GetComponentInChildren<CinemachineCamera>(); 
    }

    
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(2))
        {
            Recenter();
        }

        if(Input.GetKeyDown(KeyCode.Q) && !LockedOn)
        {
            LockOn(); 
        }
    }

    private void Recenter()
    {
        
    }

    private void LockOn()
    {
        // How do we find who to look at ? 

        // --> Use Camera's Position Vectors to deduce lock on enemy intention 
        // --> Check if lock on intent has box collider / some identifier that shows target for locking on is an enemy
        // --> Check if it's in range 
        // = Find best Candidate

        LockedOn = true;
        Transform LockOntarget = BestCandidateForLockOn();

        // --> Cinemachine Camera Motion Update
        // --> Player movement Update ------------> Feed Request into Statemachine to introduce strafing 
    }


    private Transform BestCandidateForLockOn()
    {
        List<Collider> availableEnemies = new List<Collider>(Physics.OverlapSphere(player.transform.position, LockOnRadius));

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

