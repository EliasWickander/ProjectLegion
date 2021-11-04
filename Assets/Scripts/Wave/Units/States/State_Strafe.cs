using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Strafe : State
{
    private enum StrafeState
    {
        StrafeRight,
        StrafeLeft
    }
    private UnitBase m_ownerUnit = null;
    private UnitBase m_targetUnit = null;

    private Vector3 m_moveDir = Vector3.zero;

    private StrafeState m_state;

    public State_Strafe(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        m_targetUnit = (UnitBase) param[0];
        m_state = GetStrafeState();
    }

    public override void Update()
    {
        //If target unit is dead, go back to following spline
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

        //If path to target unit is not obstructed anymore, go back to chasing it
        if (!m_ownerUnit.IsPathToTargetObstructed(m_targetUnit.transform.position))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Chase, m_targetUnit);
            return;
        }

        //Strafe right or left depending on strafe state
        m_moveDir = m_state == StrafeState.StrafeRight ? m_ownerUnit.transform.right : -m_ownerUnit.transform.right;

        m_ownerUnit.transform.position += m_moveDir * m_ownerUnit.m_chaseSpeed * Time.deltaTime;
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
    
    private StrafeState GetStrafeState()
    {
        StrafeState finalState;
        
        //Calculate how many attackers of the target unit are to the left vs to the right of this unit
        int amountAttackersLeft = 0;
        int amountAttackersRight = 0;

        foreach (UnitBase attacker in m_targetUnit.m_attackers)
        {
            Vector3 dirToAttacker = attacker.transform.position - m_ownerUnit.transform.position;
            dirToAttacker.y = 0;
            dirToAttacker.Normalize();
            
            if (Vector3.Dot(dirToAttacker, m_ownerUnit.transform.right) > 0)
            {
                amountAttackersRight++;
            }
            else
            {
                amountAttackersLeft++;
            }
        }

        //Move in the direction with the least attackers
        if (amountAttackersRight > amountAttackersLeft)
        {
            finalState = StrafeState.StrafeLeft;
        }
        else
        {
            finalState = StrafeState.StrafeRight;
        }

        return finalState;
    }
}
