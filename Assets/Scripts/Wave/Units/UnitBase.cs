using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ETeam
{
    Red,
    Blue,
}

public enum EUnitState
{
    State_Idle,
    State_FollowSpline,
    State_Chase,
    State_Attack
}

public class UnitBase : MonoBehaviour
{
    public int ID { get; set; } = 0;
    
    public ETeam m_team;
    public float m_patrolSpeed = 5;
    public float m_chaseSpeed = 5;
    public float m_detectionRadius = 5;
    public float m_attackRange = 2;

    public StateMachine m_stateMachine;

    protected void Awake()
    {
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {EUnitState.State_Idle, new State_Idle(this)},
            {EUnitState.State_FollowSpline, new State_FollowSpline(this)},
            {EUnitState.State_Chase, new State_Chase(this)},
            {EUnitState.State_Attack, new State_Attack(this)}
        };
        
        m_stateMachine = new StateMachine(states);
    }

    private void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = GetTeamLayer(m_team);   
        }
    }

    protected void Update()
    {
        m_stateMachine.Update();
    }

    public bool Detect(out UnitBase detectedUnit)
    {
        detectedUnit = null;

        LayerMask opposingUnitMask = 1 << GetTeamLayer(GetOpposingTeam());

        //Get all opposing units in range
        Collider[] foundUnits = Physics.OverlapSphere(transform.position, m_detectionRadius, opposingUnitMask);

        if (foundUnits.Length > 0)
        {
            //Find closest unit
            float closestDist = Mathf.Infinity;
            Collider closestUnit = foundUnits[0];

            foreach (Collider unit in foundUnits)
            {
                Vector3 dirToUnit = unit.transform.position - transform.position;
                dirToUnit.y = 0;

                if (dirToUnit.magnitude < closestDist)
                    closestUnit = unit;
            }
            
            //Return closest detected unit
            detectedUnit = closestUnit.GetComponentInParent<UnitBase>();
            return true;
        }

        return false;
    }

    private LayerMask GetTeamLayer(ETeam team)
    {
        return team == ETeam.Red ? LayerMask.NameToLayer("RedTeam") : LayerMask.NameToLayer("BlueTeam");
    }
    
    private ETeam GetOpposingTeam()
    {
        return m_team == ETeam.Red ? ETeam.Blue : ETeam.Red;
    }
}
