using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public abstract class AbstractEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        BASE = 0,
        CHASE = 2,
        SEARCH = 3,
    }

    [SerializeField] private EnemyState _currentState;
    [SerializeField] private Transform _gizmoOrigin;

    public Transform GizmoOrigin
    {
        get { return _gizmoOrigin; }
    }

    public EnemyVisionConeRaycast _enemyVisionRay { get; private set; }
    public EnemyState GetEnemyState() => _currentState;
    public void SetEnemyState(EnemyState state)
    {
        if (state == _currentState) return;
        _currentState = state;        
    }

    [SerializeField] public NavMeshAgent _agent { get; protected set; }
    [SerializeField] protected SO_EnemyData _enemyData;
    public SO_EnemyData EnemyData
    {
        get { return _enemyData; }
        set
        {
            _enemyData = value;
            if (_agent != null)
            {
                _agent.speed = _enemyData.speed;
            }
        }
    }

    public abstract void Move();

    public virtual void Setup()
    { 
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the enemy.");
        }
        if (_enemyData == null)
        {
            Debug.LogError("Enemy data is not assigned.");
        }
        else
        {
            _agent.speed = _enemyData.speed;
        }
        if (_gizmoOrigin == null)
        {
            _gizmoOrigin = transform;
        }
        _enemyVisionRay = GetComponent<EnemyVisionConeRaycast>();
        if (_enemyVisionRay == null)
        {
            Debug.LogError("EnemyVisionConeRaycast component is missing on the enemy.");
        }
    }


}
