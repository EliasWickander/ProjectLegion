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
    State_Strafe,
    State_Attack,
    State_Death
}

public enum UnitType
{
    Attacker,
    Defender,
}
public class UnitBase : MonoBehaviour
{
    public ETeam m_team;
    public float m_maxHealth = 100;
    public float m_patrolSpeed = 5;
    public float m_chaseSpeed = 5;
    public float m_detectionRadius = 5;
    public float m_attackRange = 2;
    public float m_attackRate = 1;
    public float m_damage = 10;

    private float m_currentHealth;

    public StateMachine m_stateMachine;

    public Collider m_collider;
    
    public List<UnitBase> m_attackers = new List<UnitBase>();

    public float m_timeToDie = 1;
    public bool m_isPendingDeath = false;
    private float m_deathTimer = 0;

    //Find better solution for this. Separate by class?
    public UnitType m_unitType;
    public EUnitState m_currentState;

    protected virtual void Awake()
    {
        m_collider = GetComponentInChildren<Collider>();
        
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {EUnitState.State_Idle, new State_Idle(this)},
            {EUnitState.State_FollowSpline, new State_FollowSpline(this)},
            {EUnitState.State_Chase, new State_Chase(this)},
            {EUnitState.State_Strafe, new State_Strafe(this)},
            {EUnitState.State_Attack, new State_Attack(this)},
            {EUnitState.State_Death, new State_Death(this)}
        };
        
        m_stateMachine = new StateMachine(states);
    }

    protected virtual void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = GetTeamLayer(m_team);   
        }

        m_currentHealth = m_maxHealth;
    }

    protected virtual void Update()
    {
        m_stateMachine.Update();

        m_currentState = (EUnitState)m_stateMachine.m_currentStateType;
        if (m_isPendingDeath)
        {
            if (m_deathTimer < m_timeToDie)
            {
                m_deathTimer += Time.deltaTime;   
            }
            else
            {
                Destroy(gameObject);
            }
        }
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

    public LayerMask GetTeamLayer(ETeam team)
    {
        return team == ETeam.Red ? LayerMask.NameToLayer("RedTeam") : LayerMask.NameToLayer("BlueTeam");
    }
    
    public ETeam GetOpposingTeam()
    {
        return m_team == ETeam.Red ? ETeam.Blue : ETeam.Red;
    }
    
    public bool IsPathToTargetObstructed(Vector3 targetPos)
    {
        //TODO: Take into account ally units that are overlapping with this unit on start of raycast
        LayerMask allyMask = 1 << GetTeamLayer(m_team);

        Bounds ownerColBounds = m_collider.bounds;

        Vector3 dirToTarget = targetPos - transform.position;
        dirToTarget.y = 0;
        
        Vector3 offset = transform.TransformDirection(new Vector3(0, 0, ownerColBounds.extents.z));
        
        Debug.DrawRay(ownerColBounds.center + offset, dirToTarget, Color.blue);

        if (Physics.Raycast(ownerColBounds.center + offset, dirToTarget.normalized, out RaycastHit hit, dirToTarget.magnitude, allyMask))
        {
            return true;
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth - damage, 0, m_maxHealth);

        if (m_currentHealth <= 0)
        {
            m_isPendingDeath = true;

            m_stateMachine.SetState(EUnitState.State_Death);
        }
        else
        {
            Debug.Log(name + " took " + damage + " damage");
        }
    }
}
