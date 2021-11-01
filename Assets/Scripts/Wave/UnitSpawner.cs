using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private ETeam m_team;
    
    [SerializeField] private UnitBase m_unitToSpawn;
    [SerializeField] private Transform m_spawnPoint;
    [SerializeField] private float m_spawnOffset = 1;

    [SerializeField] private bool m_canSpawn = false;
    [SerializeField] private float m_timeBetweenSpawns = 1;
    [SerializeField] private int m_spawnCap = 5;

    [SerializeField] private Transform[] m_pathPoints;

    private List<UnitBase> m_spawnedUnits = new List<UnitBase>();
    private float m_spawnTimer = 0;

    private void Update()
    {
        if (m_canSpawn)
        {
            //Spawn units until cap is met
            if (m_spawnedUnits.Count < m_spawnCap)
            {
                if (m_spawnTimer < m_timeBetweenSpawns)
                {
                    m_spawnTimer += Time.deltaTime;
                }
                else
                {
                    UnitBase spawnedUnit = Instantiate(m_unitToSpawn, GetRandomSpawnPoint(), Quaternion.identity);
                    spawnedUnit.m_team = m_team;
                    spawnedUnit.m_stateMachine.SetState(EUnitState.State_FollowSpline, m_pathPoints, null);
                    
                    m_spawnedUnits.Add(spawnedUnit);
                    m_spawnTimer = 0;
                }   
            }
        }
    }

    //Get random spawn point from offset
    private Vector3 GetRandomSpawnPoint()
    {
        float randOffset = Random.Range(-m_spawnOffset, m_spawnOffset);

        return m_spawnPoint.position + m_spawnPoint.right * randOffset;
    }
}
