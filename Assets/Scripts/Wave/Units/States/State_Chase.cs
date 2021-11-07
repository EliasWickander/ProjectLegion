using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //If target is dead, go back to following spline
        if (m_targetUnit.m_isPendingDeath)
        {
            if (m_ownerUnit.m_unitType == UnitType.Attacker)
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

        //If path to target unit is obstructed, strafe around it
        if (m_ownerUnit.IsPathObstructed(m_ownerUnit.transform.forward, m_ownerUnit.m_obstructionCheckRange))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Strafe_Chase, m_targetUnit);
            return;
        }

        //Move towards target unit until in attack range
        Vector3 dirToTarget = m_targetUnit.transform.position - m_ownerUnit.transform.position;
        dirToTarget.y = 0;
        float targetBoundsExtents = m_targetUnit.m_collider.bounds.extents.x;
        
        if (dirToTarget.magnitude > m_ownerUnit.m_attackRange + targetBoundsExtents)
        {
            dirToTarget.Normalize();
            m_ownerUnit.transform.position += dirToTarget * m_ownerUnit.m_chaseSpeed * Time.deltaTime;
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
        //Get direction to current path point
        Vector3 dirToCurrentPoint = m_targetUnit.transform.position - m_ownerUnit.transform.position;
        dirToCurrentPoint.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToCurrentPoint, Vector3.up);
        m_ownerUnit.transform.rotation = lookRot;
    }
}
