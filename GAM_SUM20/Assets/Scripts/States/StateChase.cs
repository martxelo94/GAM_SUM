using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChase : State
{
    public StateChase(StateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        Debug.Log("Enter " + typeof(StateChase).ToString());
    }
    public override void Update(float dt)
    {
        // change state to advance
        if (stateMachine.sensor.target == null) {
            stateMachine.SetNextState(new StateAdvance(stateMachine));
            return;
        }
        // physics move
        // change to attack if in range
        Vector2 dir = stateMachine.sensor.target.transform.position - stateMachine.transform.position;
        float dist = dir.magnitude; // not using squared becouse can be reused later
        if (dist < stateMachine.stats.range)
        {
            stateMachine.SetNextState(new StateAttack(stateMachine));
            return;
        }
        dir = dir / dist;
#if true

        stateMachine.transform.up = dir;
        stateMachine.transform.position += new Vector3(dir.x, dir.y, 0) * stateMachine.stats.maxSpeed * dt;

#else
        float speed2 = stateMachine.rig.velocity.sqrMagnitude;
        if (speed2 < stateMachine.stats.maxSpeed * stateMachine.stats.maxSpeed)
        {
            stateMachine.rig.AddForce(dir * stateMachine.stats.acceleration);
            //Debug.DrawRay(stateMachine.transform.position, dir * stateMachine.stats.maxSpeed, Color.red);
        }
        stateMachine.rig.AddTorque(
    Vector2.Dot(new Vector2(-stateMachine.transform.up.y, stateMachine.transform.up.x), dir)
    * stateMachine.stats.acceleration);

#endif

    }
    public override void Exit() { }

}
