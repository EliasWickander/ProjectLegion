using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : State
{
    private UnitBase m_ownerUnit = null;
    private UnitBase m_targetUnit = null;

    private float m_attackTimer = 0;
    
    public State_Attack(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }
    
    // Update is called once per frame
    public override void OnEnter(State prevState, params object[] param)
    {
        m_targetUnit = (UnitBase) param[0];
        
        m_targetUnit.m_attackers.Add(m_ownerUnit);
        
        Debug.Log(m_targetUnit);
    }

    public override void Update()
    {
        if (m_targetUnit.m_isPendingDeath)
        {
            if (m_ownerUnit.m_unitType == UnitType.Attacker && !(m_targetUnit is King))
            {
                m_ownerUnit.m_stateMachine.SetState(EUnitState.State_FollowSpline);   
            }
            else
            {
                m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Idle);
            }
            return;
        }
        
        RotateToTargetUnit();

        if (m_attackTimer < m_ownerUnit.m_attackRate)
        {
            m_attackTimer += Time.deltaTime;
        }
        else
        {
            m_targetUnit.TakeDamage(m_ownerUnit.m_damage);
            m_attackTimer = 0;
        }
    }

    public override void OnExit(State nextState)
    {
        m_targetUnit.m_attackers.Remove(m_ownerUnit);
        m_targetUnit = null;
    }

    private void RotateToTargetUnit()
    {
        //Get direction to current path point
        Vector3 dirToCurrentPoint = m_targetUnit.transform.position - m_ownerUnit.transform.position;
        dirToCurrentPoint.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToCurrentPoint, Vector3.up);
        m_ownerUnit.transform.rotation = lookRot;
    }
}
