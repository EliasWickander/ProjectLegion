using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Check attackers of target unit by doing overlap sphere with attack range radius, and strafe in the direction where there are least attackers
public class State_Chase : State
{
    private UnitBase m_ownerUnit = null;
    private UnitBase m_targetUnit = null;
    
    public State_Chase(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        m_targetUnit = (UnitBase) param[0];
    }

    public override void Update()
    {
        RotateToTargetUnit();
        
        //Walk towards target unit until within range. Then transition to attack state
        Vector3 dirToUnit = m_targetUnit.transform.position - m_ownerUnit.transform.position;
        dirToUnit.y = 0;

        if (dirToUnit.magnitude > m_ownerUnit.m_attackRange)
        {
            dirToUnit.Normalize();
            
            m_ownerUnit.transform.position += dirToUnit * m_ownerUnit.m_chaseSpeed * Time.deltaTime;
        }
        else
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Attack, m_targetUnit);
        }
    }

    public override void OnExit(State nextState)
    {
        m_targetUnit = null;
    }
    
    private void RotateToTargetUnit()
    {
        //Get direction to current path point (without taking y value into account)
        Vector3 dirToCurrentPoint = m_targetUnit.transform.position - m_ownerUnit.transform.position;
        dirToCurrentPoint.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToCurrentPoint, Vector3.up);
        m_ownerUnit.transform.rotation = lookRot;
    }
}
