using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask m_surfaceMask;
    
    public Vector2 m_gridWorldSize;
    public float m_nodeRadius;
    private Node[,] m_grid;

    private float m_nodeDiameter;
    private int m_gridSizeX;
    private int m_gridSizeY;

    private void OnValidate()
    {
        Start();
    }

    private void Start()
    {
        m_nodeDiameter = m_nodeRadius * 2;
        m_gridSizeX = Mathf.RoundToInt(m_gridWorldSize.x / m_nodeDiameter);
        m_gridSizeY = Mathf.RoundToInt(m_gridWorldSize.y / m_nodeDiameter);
        CreateGrid();
    }

    private void CreateGrid()
    {
        m_grid = new Node[m_gridSizeX, m_gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * m_gridWorldSize.x / 2 -
                                  Vector3.forward * m_gridWorldSize.y / 2;

        for (int x = 0; x < m_gridSizeX; x++)
        {
            for (int y = 0; y < m_gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * m_nodeDiameter + m_nodeRadius) +
                                     Vector3.forward * (y * m_nodeDiameter + m_nodeRadius);
                bool onSurface = Physics.CheckSphere(worldPoint, m_nodeRadius, m_surfaceMask);
                m_grid[x, y] = new Node(worldPoint, onSurface);
            }
        }
    }

    public Node GetNode(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        
        float percentX = (worldPosition.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x;
        float percentY = (worldPosition.z + m_gridWorldSize.y / 2) / m_gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);
        return m_grid[x, y];
    }

    private void OnDrawGizmos()
    {
        if (m_grid == null)
            return;

        foreach (Node node in m_grid)
        {
            Gizmos.color = node.m_onSurface ? Color.cyan : Color.black;
            Gizmos.DrawCube(node.m_worldPos, new Vector3(m_nodeRadius, 0.1f, m_nodeRadius));
        }
    }
}
