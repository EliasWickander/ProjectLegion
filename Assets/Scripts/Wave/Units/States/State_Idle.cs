using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Idle : State
{
    private UnitBase m_ownerUnit = null;
    public State_Idle(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        Debug.Log("start idle");
    }

    public override void Update()
    {
        if (m_ownerUnit.Detect(out UnitBase detectedUnit))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Chase, detectedUnit);
        }
    }

    public override void OnExit(State nextState)
    {
        
    }
}
