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
        if (ai.unit.target == null) {
            ai.SetNextState(new StateAdvance(ai));
            return;
        }
        // physics move
        // change to attack if in range
        Vector2 dir = ai.unit.target.transform.position - ai.transform.position;
        float dist2 = dir.sqrMagnitude; // not using squared becouse can be reused later
        float range2 = ai.unit.common.attackRange; range2 *= range2;
        if (dist2 < range2)
        {
            ai.SetNextState(new StateAttack(ai));
            return;
        }
        // normalize
        dir = dir / Mathf.Sqrt(dist2);
#if true

        ai.transform.up = dir;
        //ai.transform.position += new Vector3(dir.x, dir.y, 0) * ai.unit.common.maxSpeed * dt;
        ai.rig.velocity = new Vector3(dir.x, dir.y, 0) * ai.unit.common.maxSpeed;
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
