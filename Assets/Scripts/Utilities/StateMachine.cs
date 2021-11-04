using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine
{
    public Dictionary<Enum, State> m_states;

    private State m_currentState;
    public Enum m_currentStateType;

    public StateMachine(Dictionary<Enum, State> states)
    {
        this.m_states = states;
        
        SetState(states.Keys.First());
    }

    public void Update()
    {
        if (m_currentState != null)
        {
            m_currentState.Update();
        }
    }

    public void SetState(Enum state, params object[] param)
    {
        if (!m_states.ContainsKey(state))
            throw new Exception("Tried to set to an invalid state");

        State oldState = m_currentState;
        State newState = m_states[state];
        
        if(oldState != null)
            oldState.OnExit(newState);
        
        newState.OnEnter(oldState, param);

        m_currentStateType = state;
        m_currentState = newState;
    }
}
