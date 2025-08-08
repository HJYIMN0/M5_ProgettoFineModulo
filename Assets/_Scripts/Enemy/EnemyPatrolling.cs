using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolling : AbstractEnemy
{

    [SerializeField] private Transform[] _patrolPoints;



    private EnemyVisionCone _enemyVision;
    private EnemyVisionConeRaycast _enemyVisionRay;

    private int _currentPatrolIndex = 0;

    private void Awake()
    {
        Setup();
    }
    private void Update()
    {
        Move();
    }


    public override void Setup()
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

        _enemyVision = GetComponent<EnemyVisionCone>();
        if (_enemyVision == null)
        {
            Debug.Log("The vision Cone is Missing!");
            _enemyVisionRay = GetComponent<EnemyVisionConeRaycast>();
            if (_enemyVisionRay == null)
            {
                Debug.LogError("EnemyVisionConeRaycast component is missing on the enemy.");
            }
        }
    }
    public override void Move()
    {
        switch (GetEnemyState()) 
        {
             case EnemyState.SEARCH:
                 
                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed;
                SearchMove();
                
                  break;
                 
             case EnemyState.PATROL:
                  
                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed;
                if (_patrolPoints.Length <= 0 || _patrolPoints == null)
                {
                    Debug.LogError("No patrol points assigned for the enemy.");
                    return;
                }
                PatrolMove();
                break;
                 
             case EnemyState.BASE:
             
                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed;
                SetEnemyState(EnemyState.PATROL);
                break;
             
             case EnemyState.CHASE:
             
                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed * _enemyData.speedMultiplier;
                ChaseMove();
                break;
                
        }

    }

    #region SearchStatus
    public void SearchMove()
    {
        _agent.ResetPath();
        StopAllCoroutines();
        StartCoroutine("SearchPlayer"); //non mi sembra una grande idea mettere una coroutine nell'update
        _agent.Move(_enemyVisionRay._lastKnownPos.transform.position * _enemyData.speed * Time.deltaTime);        
    }

    public IEnumerator SearchPlayer()
    {
        while (!_enemyVisionRay.playerInSight)
        {
            if (!_enemyVisionRay._wasPlayerInSight) break;
            yield return new WaitForSeconds(_enemyData.coroutineTime);    
            _enemyVisionRay.SetWasPlayerInSight(_enemyVisionRay.playerInSight);
            this.SetEnemyState(EnemyState.BASE);
            break;
        }
    }
    #endregion

    #region ChaseStatus
    public void ChaseMove()
    {
        if (!_enemyVisionRay.playerInSight)
        {
            if (_enemyVisionRay._wasPlayerInSight)
            {
                SetEnemyState(EnemyState.SEARCH);
                return;
            }            
        }
        else
        {
            _enemyVisionRay.SetWasPlayerInSight(_enemyVisionRay.playerInSight);
            _agent.Move(_enemyVisionRay._lastKnownPos.transform.position * _enemyData.speed * Time.deltaTime);
        }
    }
    #endregion

    #region PatrolStatus
    public void PatrolMove()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
            _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
            _agent.Move(_patrolPoints[_currentPatrolIndex].position * _enemyData.speed * Time.deltaTime);
        }
    }

    public IEnumerator Patrol()
            {
        while (GetEnemyState() == EnemyState.PATROL)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                yield return new WaitForSeconds(_enemyData.coroutineTime);
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
                _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
            }
            yield return null;
        }
    }
    #endregion
}
