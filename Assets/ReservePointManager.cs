using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReservePointManager : MonoBehaviour
{
    public static ReservePointManager Instance { get; private set; }
    
    //Reserve point offsets. Accessible by unit ID
    public Dictionary<int, Vector3> m_reservePointOffsets = new Dictionary<int, Vector3>();

    private void Awake()
    {
        Instance = this;
    }
}
