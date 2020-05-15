using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAdvance : State
{
    bool troop_forward;

    public StateAdvance(StateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        troop_forward = stateMachine.stats.team == TeamType.Player ? true : false;
        //if (!troop_forward)
        //    stateMachine.transform.Rotate(new Vector3(0, 0, 180));
        Vector3 dir = troop_forward ? Vector2.up : -Vector2.up;
        //stateMachine.transform.LookAt(stateMachine.transform.position + dir, Vector3.back);
        //stateMachine.transform.forward = troop_forward ? Vector3.forward : Vector3.back;
        stateMachine.transform.up = troop_forward ? Vector2.up : -Vector2.up;
        if (!troop_forward)
            stateMachine.transform.Rotate(new Vector3(0, 180, 0));
        Debug.Log("Enter " + typeof(StateAdvance).ToString());
    }
    public override void Update(float dt)
    {
        // physics move
        Vector2 dir = troop_forward ? Vector2.up : -Vector2.up;
#if true
        
        stateMachine.transform.position += new Vector3(dir.x, dir.y, 0) * stateMachine.stats.maxSpeed * dt;
#else
        float speed2 = stateMachine.rig.velocity.sqrMagnitude;
        if (speed2 < stateMachine.stats.maxSpeed * stateMachine.stats.maxSpeed)
        {
            stateMachine.rig.AddForce(new Vector2(0, dir.y * stateMachine.stats.acceleration));
        }
        stateMachine.rig.AddTorque(
    Vector2.Dot(new Vector2(-stateMachine.transform.up.y, stateMachine.transform.up.x), dir)
    * stateMachine.stats.acceleration);

#endif

        // change state
        if (stateMachine.sensor.target != null)
            stateMachine.SetNextState(new StateChase(stateMachine));
    }
    public override void Exit() { }
}
