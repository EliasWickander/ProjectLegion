using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement MovementScript { get; private set; }

    private Camera m_playerCam;

    private InputActionControls m_inputActions;

    private void Awake()
    {
        MovementScript = GetComponent<PlayerMovement>();

        m_playerCam = Camera.main;
        
        m_inputActions = new InputActionControls();
        
        m_inputActions.Player.Movement.performed += OnMovement;
        
        m_inputActions.Enable();
        
    }
    

    private void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Get cursor location in world and provide it as a parameter to movement function
            Ray rayToCursor = m_playerCam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(rayToCursor, out RaycastHit hit, Mathf.Infinity))
            {
                MovementScript.MoveToLocation(hit.point);   
            }   
        }
    }
    
}   
