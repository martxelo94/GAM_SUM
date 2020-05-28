using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : State
{
    public StateAttack(StateMachine stateMachine) : base(stateMachine) { }
    public override void Enter()
    {
        // stop movement
        ai.rig.velocity = Vector3.zero;
    Debug.Log("Enter " + typeof(StateAttack).ToString());
    }
    public override void Update(float dt)
    {
        if (ai.unit.target == null)
        {
            ai.SetNextState(new StateAdvance(ai));
            return;
        }
        ai.unit.currentAttackTime += dt;
        // face towards target
        Debug.DrawLine(ai.unit.target.transform.position, ai.transform.position);
        Vector2 dir = ai.unit.target.transform.position - ai.transform.position;
        ai.transform.up = dir.normalized;


        // attack cooldown
        if (ai.unit.currentAttackTime > ai.unit.common.attackTime) {
            if (ai.unit.target != null) {
                if (ai.unit.common.attackPrefab != null)
                    SpawnProyectilAttack(ai.transform.position, ai.unit.target.transform.position);
                else if (ai.unit.target.ReceiveDamage(ai.unit.common.damage))
                    ai.unit.target = null;
                ai.unit.currentAttackTime = 0.0f;
            }
        }
    }
    public override void Exit() { }

    void SpawnProyectilAttack(Vector3 start, Vector3 end)
    {
        ProyectilAttack attack = GameObject.Instantiate(ai.unit.common.attackPrefab);
        attack.startShoot = start;
        attack.endShoot = end;
        attack.shootDamage = ai.unit.common.damage;
    }
}
