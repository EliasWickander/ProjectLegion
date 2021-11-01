using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public Dictionary<ETeam, King> m_kingsInWorld = new Dictionary<ETeam, King>();
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetKingsInWorld();
    }

    //Get reference of all kings in world and the team they belong to
    private void GetKingsInWorld()
    {
        King[] kings = FindObjectsOfType<King>();

        foreach (King king in kings)
        {
            if(m_kingsInWorld.ContainsKey(king.m_team))
                throw new Exception("More than one king of team " + king.m_team + " in world. There should always be one per team.");
            
            m_kingsInWorld.Add(king.m_team, king);
        }
    }
}
