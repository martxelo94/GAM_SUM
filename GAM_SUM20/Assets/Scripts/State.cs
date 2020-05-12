using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected GameObject gameObject;

    protected State(GameObject gameObject)
    {
        this.gameObject = gameObject;
    }

    public virtual void Enter() { }
    public virtual void Update(float dt) { }
    public virtual void Exit() { }
}
