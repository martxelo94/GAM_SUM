using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        if (ai.unit.IsTargetAlive() == false)
        {
            ai.SetNextState(new StateAdvance(ai));
            return;
        }
        ai.unit.currentAttackTime += dt;
        // face towards target
        Vector2 dir = ai.unit.DifToTarget();
        ai.transform.up = dir.normalized;

        // stop unit
        ai.rig.velocity = Vector3.zero;

        // attack cooldown
        if (ai.unit.currentAttackTime > ai.unit.common.attackTime) {
            Assert.IsTrue(ai.unit.IsTargetAlive() == true);
            //if (ai.unit.target != null)
            {
                Assert.IsTrue(ai.unit.common.attackPrefabs[0] != null);
                //if (ai.unit.common.attackPrefabs[0] != null)
                ai.unit.SpawnAttack();
                //else Debug.LogError("Unit " + ai.name + " lacks attack prefab!");
                ai.unit.currentAttackTime = 0.0f;
            }
        }
    }
    public override void Exit() { }

}
