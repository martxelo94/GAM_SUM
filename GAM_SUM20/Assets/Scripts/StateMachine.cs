using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StateMachine : MonoBehaviour
{
    public Dictionary<string, State> states;
    public State current_state;
    public State next_state;
    // Start is called before the first frame update
    void Start()
    {
        current_state = next_state = new StateAdvance(gameObject);
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

    void SetNextState(State state)
    {
        next_state = state;
    }
}
