using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AbstractEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        BASE = 0,
        PATROL = 1,
        CHASE = 2,
        SEARCH = 3,
    }

    [SerializeField] private EnemyState _currentState;
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

    public abstract void Setup();
}
