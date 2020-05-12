using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAdvance : State
{
    Rigidbody2D rig;
    UnitStats stats;
    bool troop_forward;

    public StateAdvance(GameObject gameObject) : base(gameObject) { }
    public override void Enter()
    {
        rig = gameObject.GetComponent<Rigidbody2D>();
        stats = gameObject.GetComponent<UnitStats>();
        troop_forward = stats.team == TeamType.Player ? true : false;
    }
    public override void Update(float dt)
    {
        // physics move
        float speed2 = rig.velocity.sqrMagnitude;
        Vector2 dir = troop_forward ? Vector2.up : -Vector2.up;
        if (speed2 < stats.maxSpeed* stats.maxSpeed)
        {
            rig.AddForce(new Vector2(0, dir.y * stats.acceleration));
        }
        rig.AddTorque(Vector2.Dot(new Vector2(-gameObject.transform.up.y, gameObject.transform.up.x), dir) * stats.maxSpeed* stats.acceleration);
    }
    public override void Exit() { }
}
