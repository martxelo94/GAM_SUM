using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAdvance : State
{
    public StateAdvance(StateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        bool troop_forward = ai.unit.team == TeamType.Player ? true : false;
        ai.transform.eulerAngles = troop_forward ? Vector3.zero : new Vector3(0, 0, 180);
        //Debug.Log("Enter " + typeof(StateAdvance).ToString());
    }
    public override void Update(float dt)
    {
        // change state
        if (ai.unit.IsTargetAlive() == true) {
            ai.SetNextState(new StateChase(ai));
            return;
        }

        bool troop_forward = ai.unit.team == TeamType.Player ? true : false;
        // physics move
        Vector3 dir = troop_forward ? Vector3.up : Vector3.down;
#if true
        ai.rig.velocity = dir * ai.unit.common.maxSpeed;
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

    }
    public override void Exit() { }
}
