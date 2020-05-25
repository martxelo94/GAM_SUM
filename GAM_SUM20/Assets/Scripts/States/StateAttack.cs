using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : State
{
    float attackTime = 0.0f;

    public StateAttack(StateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        // stop movement
        stateMachine.rig.velocity = Vector3.zero;
    Debug.Log("Enter " + typeof(StateAttack).ToString());
    }
    public override void Update(float dt)
    {
        if (stateMachine.sensor.target == null)
        {
            stateMachine.SetNextState(new StateAdvance(stateMachine));
            return;
        }
        attackTime += dt;
        // face towards target
        Debug.DrawLine(stateMachine.sensor.target.transform.position, stateMachine.transform.position);
        Vector2 dir = stateMachine.sensor.target.transform.position - stateMachine.transform.position;
        stateMachine.transform.up = dir.normalized;


        // attack cooldown
        if (attackTime > stateMachine.stats.attackTime) {
            if (stateMachine.sensor.target != null) {
                if (stateMachine.stats.attackPrefab != null)
                    SpawnProyectilAttack(stateMachine.transform.position, stateMachine.sensor.target.transform.position);
                else if (stateMachine.sensor.target.ReceiveDamage(stateMachine.stats.damage))
                    stateMachine.sensor.target = null;
            }
            attackTime = 0.0f;
        }
    }
    public override void Exit() { }

    void SpawnProyectilAttack(Vector3 start, Vector3 end)
    {
        ProyectilAttack attack = GameObject.Instantiate(stateMachine.stats.attackPrefab);
        attack.startShoot = start;
        attack.endShoot = end;

    }
}
