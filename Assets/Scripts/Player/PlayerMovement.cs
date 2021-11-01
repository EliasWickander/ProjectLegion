using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 5;

    private CharacterController m_characterController;

    private bool m_isMoving = false;
    private Vector3 m_location;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (m_isMoving)
        {
            Vector3 targetPos = m_location;
            targetPos.y = transform.position.y;

            Vector3 dirToTarget = targetPos - transform.position;
            
            if (dirToTarget.magnitude > 0.1f)
            {
                m_characterController.Move(dirToTarget.normalized * m_moveSpeed * Time.deltaTime);
            }
            else
            {
                m_isMoving = false;
            }
        }
    }

    public void MoveToLocation(Vector3 location)
    {
        m_isMoving = true;
        m_location = location;
    }
}
