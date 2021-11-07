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
    State_Strafe_Chase,
    State_Strafe_Default,
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
    [HideInInspector] public UnitSpawner m_spawner;
    
    public ETeam m_team;
    public float m_maxHealth = 100;
    public float m_patrolSpeed = 5;
    public float m_chaseSpeed = 5;
    public float m_detectionRadius = 5;
    public float m_attackRange = 2;
    public float m_attackRate = 1;
    public float m_damage = 10;
    public float m_obstructionCheckRange = 2;

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

    public Vector3 m_startOffset;
    

    protected virtual void Awake()
    {
        m_collider = GetComponentInChildren<Collider>();
        
        Dictionary<Enum, State> states = new Dictionary<Enum, State>()
        {
            {EUnitState.State_Idle, new State_Idle(this)},
            {EUnitState.State_FollowSpline, new State_FollowSpline(this)},
            {EUnitState.State_Chase, new State_Chase(this)},
            {EUnitState.State_Strafe_Chase, new State_Strafe_Chase(this)},
            {EUnitState.State_Strafe_Default, new State_Strafe_Default(this)},
            {EUnitState.State_Attack, new State_Attack(this)},
            {EUnitState.State_Death, new State_Death(this)}
        };
        
        m_stateMachine = new StateMachine(states);
    }

    public void Init()
    {
        m_startOffset = transform.position - m_spawner.m_spawnPoint.position;

        m_team = m_spawner.m_team;
        m_stateMachine.SetState(EUnitState.State_FollowSpline, m_spawner.m_pathPoints, null);   
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
    
    public bool IsPathObstructed(Vector3 direction, float obstructionCheckRange)
    {
        //TODO: Take into account ally units that are overlapping with this unit on start of raycast
        LayerMask allyMask = 1 << GetTeamLayer(m_team);

        Bounds ownerColBounds = m_collider.bounds;
        
        
        Vector3 offset = transform.TransformDirection(new Vector3(0, 0, ownerColBounds.extents.z));

        //Check if a unit is in front of owner unit but not overlapping
        if (Physics.Raycast(ownerColBounds.center + offset, direction, obstructionCheckRange, allyMask))
        {
            return true;
        }
        
        // Check if a unit is in front of owner unit and overlapping
        // Vector3 topPos = ownerColBounds.center + new Vector3(0, ownerColBounds.extents.y, 0);
        // Vector3 botPos = ownerColBounds.center + new Vector3(0, ownerColBounds.extents.y, 0);
        // Collider[] overlappedUnits = Physics.OverlapCapsule(botPos, topPos, ownerColBounds.extents.x, allyMask);
        //
        // if (overlappedUnits.Length > 0)
        // {
        //     foreach (Collider overlappedUnit in overlappedUnits)
        //     {
        //         //Don't take self into account
        //         if (overlappedUnit.transform.root == transform.root)
        //             continue;
        //
        //         Vector3 dirToUnit = overlappedUnit.transform.position - transform.position;
        //         dirToUnit.y = 0;
        //
        //         if (Vector3.Dot(dirToUnit, transform.forward) > 0)
        //         {
        //             return true;
        //         }
        //     }
        // }

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
            //Debug.Log(name + " took " + damage + " damage");
        }
    }

    private void OnDrawGizmos()
    {
        m_collider = GetComponentInChildren<Collider>();
    }
}
