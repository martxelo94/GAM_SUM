﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class StateMachine : MonoBehaviour
{
    [HideInInspector]
    public Sensor sensor;
    [HideInInspector]
    public UnitStats stats;
    [HideInInspector]
    public Rigidbody2D rig;

    public State current_state;
    public State next_state;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        stats = GetComponent<UnitStats>();
        sensor = GetComponent<Sensor>();

        current_state = next_state = new StateAdvance(this);
        current_state.Enter();
    }

    // Update is called once per frame
    void Update()
    {
        if (current_state != next_state) {
            current_state.Exit();
            next_state.Enter();
            current_state = null;   //delete?
            current_state = next_state;
        }
        current_state.Update(Time.deltaTime * Time.timeScale);
    }

    public void SetNextState(State state)
    {
        next_state = null;  // delete?
        next_state = state;
    }
}