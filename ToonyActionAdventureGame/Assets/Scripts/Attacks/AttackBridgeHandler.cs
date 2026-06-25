using Unity.Properties;
using UnityEngine;

//[RequireComponent(typeof(CombatStateMachine))]
public class AttackBridgeHandler<T> : MonoBehaviour
{
    // Bridge Combat state machine with the handler (player / AI) 

    //private IHasEntityProperties<T> HANDLE;

    private void Start()
    {
        //HANDLE = GetComponent<IHasEntityProperties<T>>(); 
    }

    private void RequestHandlerData()
    {

    }
}