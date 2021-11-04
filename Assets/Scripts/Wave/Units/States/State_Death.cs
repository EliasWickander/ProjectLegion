using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Death : State
{
    private UnitBase m_ownerUnit = null;
    public State_Death(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        m_ownerUnit.m_collider.enabled = false;
    }

    public override void Update()
    {

    }

    public override void OnExit(State nextState)
    {
        
    }
}
