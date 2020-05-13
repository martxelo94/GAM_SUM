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

        Debug.Log("Enter " + typeof(StateAdvance).ToString());
    }
    public override void Update(float dt)
    {
        // physics move
        float speed2 = stateMachine.rig.velocity.sqrMagnitude;
        Vector2 dir = troop_forward ? Vector2.up : -Vector2.up;
        if (speed2 < stateMachine.stats.maxSpeed* stateMachine.stats.maxSpeed)
        {
            stateMachine.rig.AddForce(new Vector2(0, dir.y * stateMachine.stats.acceleration));
        }
        stateMachine.rig.AddTorque(
            Vector2.Dot(new Vector2(-stateMachine.transform.up.y, stateMachine.transform.up.x), dir)
            * stateMachine.stats.acceleration);

        // change state
        if (stateMachine.sensor.target != null)
            stateMachine.SetNextState(new StateChase(stateMachine));
    }
    public override void Exit() { }
}
