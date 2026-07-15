using UnityEngine;
using EntityStateMachines;
public class Fall : State
{
    //Handle fast fall
    // Handle normal Gravitational movement 

    [SerializeField] private FallSettings data;

    public Grounded groundState;
    public Fall(FallSettings @data)
    {
        this.data = @data;

    }


    public override void Enter() { }
    public override void HandleInput() { }

    public override void Update()
    {
        if (stopApplyingGravity())
            Emachine.SwitchStates(groundState);


        ApplyGravity();
    }

    private void ApplyGravity()
    {
        gameObj.transform.position += new Vector3(0, -data.Gravity * Time.deltaTime, 0);
    }

    private bool stopApplyingGravity()
    {
        if (Physics.Raycast(gameObj.transform.position, Vector3.down, data.rayDistanceCheck, data.GroundLayer))
            return true;
        return false;
    }
    public override void Exit() { }
}

