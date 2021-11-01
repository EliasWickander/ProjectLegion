using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{

    public State(GameObject owner)
    {
        this.m_owner = owner;
        this.m_transform = owner.transform;
    }
    
    public abstract void OnEnter(State prevState, params object[] param);
    public abstract void Update();
    public abstract void OnExit(State nextState);

    protected GameObject m_owner;
    protected Transform m_transform;
}
