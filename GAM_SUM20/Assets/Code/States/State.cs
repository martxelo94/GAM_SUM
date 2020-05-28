using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected StateMachine ai;

    protected State(StateMachine stateMachine)
    {
        ai = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update(float dt) { }
    public virtual void Exit() { }
}
