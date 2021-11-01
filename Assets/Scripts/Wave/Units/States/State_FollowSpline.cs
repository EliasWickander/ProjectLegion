using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_FollowSpline : State
{
    private UnitBase m_ownerUnit = null;
    
    private Queue<Vector3> m_pathPoints = new Queue<Vector3>();
    private Vector3 m_currentPathPoint = Vector3.zero;
    
    public State_FollowSpline(UnitBase mOwner) : base(mOwner.gameObject)
    {
        m_ownerUnit = mOwner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        //Initialize path points, but only if the queue is empty (to allow for temporary exit of this state)
        if (m_pathPoints.Count <= 0)
        {
            Transform[] pathPoints = (Transform[])param[0];

            foreach (Transform pathPoint in pathPoints)
            {
                m_pathPoints.Enqueue(pathPoint.position);
            }

            m_currentPathPoint = m_pathPoints.Peek();
        }
    }

    public override void Update()
    {
        if (m_ownerUnit.Detect(out UnitBase detectedUnit))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Chase, detectedUnit);
        }
        
        if (m_pathPoints.Count > 0)
        {
            HandlePathFollow();
            RotateToCurrentPoint();
        }
        else
        {
            //We finished walking along the path
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Idle);
        }
    }

    public override void OnExit(State nextState)
    {
        m_currentPathPoint = Vector3.zero;
    }

    private void HandlePathFollow()
    {
        //Get direction to current path point (without taking y value into account)
        Vector3 dirToCurrentPoint = m_currentPathPoint - m_ownerUnit.transform.position;
        dirToCurrentPoint.y = 0;
        
        //If point is still far away, walk towards it. If not, set next point
        if (dirToCurrentPoint.magnitude > 0.05)
        {
            dirToCurrentPoint.Normalize();

            m_ownerUnit.transform.position += dirToCurrentPoint * m_ownerUnit.m_patrolSpeed * Time.deltaTime;
        }
        else
        {
            //Remove reference to path point we just finished walking to
            m_pathPoints.Dequeue();
                
            //Set next path point if there is any
            if(m_pathPoints.Count > 0)
                m_currentPathPoint = m_pathPoints.Peek();
        }
    }

    private void RotateToCurrentPoint()
    {
        //Get direction to current path point (without taking y value into account)
        Vector3 dirToCurrentPoint = m_currentPathPoint - m_ownerUnit.transform.position;
        dirToCurrentPoint.y = 0;

        Quaternion lookRot = Quaternion.LookRotation(dirToCurrentPoint, Vector3.up);
        m_ownerUnit.transform.rotation = lookRot;
    }
}