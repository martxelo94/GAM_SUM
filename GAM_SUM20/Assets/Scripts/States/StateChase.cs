using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChase : State
{
    Transform target;

    public StateChase(GameObject gameObject) : base(gameObject) { }
    public override void Enter() { }
    public override void Update(float dt) { }
    public override void Exit() { }

}
