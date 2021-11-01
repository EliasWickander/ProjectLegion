using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private Camera m_camera;

    [SerializeField] private float moveSpeedX = 5; 
    [SerializeField] private float moveSpeedY = 5; 

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = m_camera.ScreenToViewportPoint(Mouse.current.position.ReadValue());

        Vector3 moveDir = Vector2.zero;

        //If mouse position is close enough to edge, move in that direction
        if(mousePos.x > 0.9f)
        {
            moveDir += Vector3.right;
        }
        else if(mousePos.x < 0.1f)
        {
            moveDir -= Vector3.right;
        }	
	
        if(mousePos.y > 0.9f)
        {
            moveDir += Vector3.forward;
        }
        else if(mousePos.y < 0.1f)
        {
            moveDir -= Vector3.forward;
        }

        Vector3 finalVelocity = new Vector3(moveDir.x * moveSpeedX, 0, moveDir.z * moveSpeedY);
        transform.position += finalVelocity * Time.deltaTime;
    }
}
