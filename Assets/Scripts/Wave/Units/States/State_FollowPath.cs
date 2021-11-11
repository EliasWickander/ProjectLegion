using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_FollowPath : State
{

    private UnitBase m_ownerUnit = null;

    private Transform[] m_pathPoints;
    
    private Queue<Vector3> m_pathPointsQueue = new Queue<Vector3>();
    private Vector3 m_currentPathPoint = Vector3.zero;
    

    public State_FollowPath(UnitBase mOwner) : base(mOwner.gameObject)
    {
        m_ownerUnit = mOwner;
    }

    public override void OnEnter(State prevState, params object[] param)
    {
        //Initialize path points, but only if the queue is empty (to allow for temporary exit of this state)
        if (m_pathPointsQueue.Count <= 0)
        {
            m_pathPoints = (Transform[])param[0];

            SetUpPathPointsQueue();
            m_currentPathPoint = m_pathPointsQueue.Peek();
            Debug.DrawLine(m_ownerUnit.transform.position, m_currentPathPoint, Color.green, 5);
        }
    }

    public override void Update()
    {
        //If an enemy unit is detected, start chasing that unit
        if (m_ownerUnit.Detect(out UnitBase detectedUnit))
        {
            m_ownerUnit.m_stateMachine.SetState(EUnitState.State_Chase, detectedUnit);
            return;
        }


        if (m_ownerUnit.IsPathObstructed(m_ownerUnit.transform.forward, m_ownerUnit.m_obstructionCheckRange))
        {

        }

        if (m_pathPointsQueue.Count > 0)
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
            m_pathPointsQueue.Dequeue();
                
            //Set next path point if there is any
            if (m_pathPointsQueue.Count > 0)
            {
                m_currentPathPoint = m_pathPointsQueue.Peek();
                Debug.DrawLine(m_ownerUnit.transform.position, m_currentPathPoint, Color.green, 5);
            }
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
    
        //Sets up queue of path points with offset
    private void SetUpPathPointsQueue()
    {
        bool isMirrored = IsMirrored();
        
        for (int i = 0; i < m_pathPoints.Length; i++)
        {
            Transform currentPathPoint = m_pathPoints[i];
                
            //If at the first path point, let the spawner spawn point count as the previous one
            Transform prevPathPoint = i == 0 ? m_ownerUnit.m_spawner.m_spawnPoint : m_pathPoints[i - 1];

            Vector3 dirToCurrentPathPoint = currentPathPoint.position - prevPathPoint.position;
            dirToCurrentPathPoint.y = 0;
            dirToCurrentPathPoint.Normalize();

            Vector3 unitSpawnOffset = isMirrored ? -m_ownerUnit.m_startOffset : m_ownerUnit.m_startOffset;

            //Get right offset by rotating spawn offset in the direction of the current path point
            Quaternion rotToCurrentPathPoint = Quaternion.LookRotation(dirToCurrentPathPoint);
            Vector3 rightOffset = rotToCurrentPathPoint * unitSpawnOffset;
                
            Vector3 frontOffset = Vector3.zero;
                
            //Get front offset by rotating spawn offset in the direction of the next path point
            if (i < m_pathPoints.Length - 1)
            {
                Transform nextPathPoint = m_pathPoints[i + 1];
                    
                Vector3 dirToNextPathPoint = nextPathPoint.position - currentPathPoint.position;
                dirToNextPathPoint.y = 0;
                dirToNextPathPoint.Normalize();
                
                Quaternion rotToNextPathPoint = Quaternion.LookRotation(dirToNextPathPoint);
                
                frontOffset = rotToNextPathPoint * unitSpawnOffset;
                
            }

            Vector3 offset = rightOffset + frontOffset;

            //Add path point with offset to queue
            m_pathPointsQueue.Enqueue(currentPathPoint.position + offset);
        }
    }
    
    //Check if this unit's path is mirrored
    private bool IsMirrored()
    {
        if (m_pathPoints.Length > 0)
        {
            Transform firstPathPoint = m_pathPoints[0];

            Vector3 dirToFirstPathPoint = firstPathPoint.position - m_ownerUnit.m_spawner.m_spawnPoint.position;
            dirToFirstPathPoint.y = 0;
            dirToFirstPathPoint.Normalize();

            return Vector3.Dot(dirToFirstPathPoint, Vector3.forward) < 0;
        }

        return false;
    }
}
