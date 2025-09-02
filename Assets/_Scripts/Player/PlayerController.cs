using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private NavMeshAgent _agent;
    private PlayerControlState _controlState = PlayerControlState.HYBRID;

    private int index;

    private float h;
    private float v;

    private bool isMoving => _agent.velocity.magnitude > 0.1f;
    
    public bool IsMoving => isMoving;



    private void Awake()
    {
        if (_cam == null)
        {
            Debug.Log("No camera assigned, using main camera.");
            _cam = Camera.main;
        }

        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) Debug.LogError("NavMeshAgent component is missing on PlayerController.");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire3")) //left Shift
        {
            index++;
            if (index > 2) index = 0;

            _controlState = (PlayerControlState)index;

            Debug.Log("Control State Changed: " + _controlState);
        }

        if (Input.GetButtonDown("Fire1")) //left mouse button
        {
            Move();
            
        }

        if (!_controlState.Equals(PlayerControlState.MOUSE))
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            Move();           
        }
    }

    public void Move()
    {

        switch (_controlState)
        {
            case PlayerControlState.HYBRID:
                _agent.ResetPath();
                MoveHybrid();
                break;
            case PlayerControlState.KEYBOARD:
                _agent.ResetPath();
                MoveKeyboard(h, v);
                break;
            case PlayerControlState.MOUSE:
                _agent.ResetPath();
                MoveMouse();
                break;
        }
    }

    public void MoveHybrid()
    {
        MoveMouse();
        if (h != 0 || v != 0)
        {
            _agent.ResetPath();
            MoveKeyboard(h,v);
        }
        else
        {
            _agent.ResetPath();
        }
    }

    public void MoveKeyboard(float horizontal, float vertical)
    {
        if (h == 0 && v == 0) return;

        _agent.ResetPath();

        Vector3 _playerInput = new Vector3(horizontal, 0, vertical).normalized;

        //questo è per far muovere il player nella direzione della camera
        //Mi sono accorto dopo che avrei impostato la camera Dall'alto e quindi non serviva
        Vector3 camForward = _cam.transform.forward;
        Vector3 camRight = _cam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * _playerInput.z + camRight * _playerInput.x).normalized;

        // Muove l'agente in tempo reale, rispettando il NavMesh
        _agent.Move(moveDir * _agent.speed * Time.deltaTime);

        // Ruota il personaggio nella direzione di movimento
        if (moveDir != Vector3.zero)
            transform.forward = moveDir;
    }

    public void MoveMouse()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.SetDestination(hit.point);
            }
        }
    }

    public enum PlayerControlState
    {
        HYBRID = 0,
        KEYBOARD = 1,
        MOUSE = 2
    }
}


