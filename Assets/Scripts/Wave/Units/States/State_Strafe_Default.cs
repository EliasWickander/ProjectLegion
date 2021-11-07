using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Strafe_Default : State
{
    private enum StrafeState
    {
        StrafeRight,
        StrafeLeft
    }
    private UnitBase m_ownerUnit = null;

    private Vector3 m_moveDir = Vector3.zero;

    private StrafeState m_state;

    public State_Strafe_Default(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        m_state = GetStrafeState();
    }

    public override void Update()
    {
        if (!m_ownerUnit.IsPathObstructed(m_ownerUnit.transform.forward, m_ownerUnit.m_obstructionCheckRange))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_FollowSpline);
            return;
        }
        m_moveDir = m_state == StrafeState.StrafeRight ? m_ownerUnit.transform.right : -m_ownerUnit.transform.right;

        m_ownerUnit.transform.position += m_moveDir * m_ownerUnit.m_chaseSpeed * Time.deltaTime;
    }

    public override void OnExit(State nextState)
    {

    }

    private StrafeState GetStrafeState()
    {
        StrafeState finalState;
        
        LayerMask allyMask = 1 << m_ownerUnit.GetTeamLayer(m_ownerUnit.m_team);
        
        //Calculate how many units are to the left vs to the right of this unit
        int amountUnitsLeft = 0;
        int amountUnitsRight = 0;

        Collider[] unitsInArea = Physics.OverlapSphere(m_ownerUnit.m_collider.bounds.center, 5, allyMask);

        foreach (Collider unit in unitsInArea)
        {
            Vector3 dirToUnit = unit.transform.position - m_ownerUnit.transform.position;
            dirToUnit.y = 0;
            dirToUnit.Normalize();
            
            //We only care about units that are in front of this unit
            if (Vector3.Dot(dirToUnit, m_ownerUnit.transform.forward) > 0)
            {
                if (Vector3.Dot(dirToUnit, m_ownerUnit.transform.right) > 0)
                {
                    amountUnitsRight++;
                }
                else
                {
                    amountUnitsLeft++;
                }
            }
        }

        //Move in the direction with the least units
        if (amountUnitsRight > amountUnitsLeft)
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
