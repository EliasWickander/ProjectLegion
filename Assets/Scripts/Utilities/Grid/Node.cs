using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool m_onSurface;
    public Vector3 m_worldPos;

    public Node(Vector3 worldPos, bool onSurface)
    {
        m_onSurface = onSurface;
        m_worldPos = worldPos;
    }
}
