using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Attack : State
{
    private UnitBase m_ownerUnit = null;
    private UnitBase m_targetUnit = null;
    public State_Attack(UnitBase owner) : base(owner.gameObject)
    {
        m_ownerUnit = owner;
    }
    
    // Update is called once per frame
    public override void OnEnter(State prevState, params object[] param)
    {
        m_targetUnit = (UnitBase) param[0];
    }

    public override void Update()
    {
        RotateToTargetUnit();
        Debug.Log("attacking");
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
