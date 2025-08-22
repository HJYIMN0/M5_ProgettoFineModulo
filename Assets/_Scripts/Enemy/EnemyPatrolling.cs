using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolling : AbstractEnemy
{

    [SerializeField] private Transform[] _patrolPoints;

    private int _currentPatrolIndex = 0;

    //private bool _isCoroutineRunning = false;

    //private float timer;

    //[SerializeField] private bool _hasUiText;


    public override void Update()
    {
        base.Update();
        Interact();
    }
    
    //public override void Move()
    //{
    //    switch (GetEnemyState()) 
    //    {
    //         case EnemyState.SEARCH:
                 
    //            Debug.Log(GetEnemyState());
    //            _agent.speed = _enemyData.speed;
    //            SearchMove();

    //            break;
                 
    //         case EnemyState.BASE:
    //            Debug.Log(GetEnemyState());
    //            _agent.speed = _enemyData.speed;
    //            if (_patrolPoints.Length <= 0 || _patrolPoints == null)
    //            {
    //                Debug.LogError("No patrol points assigned for the enemy.");
    //                return;
    //            }
    //            //PatrolMove();
    //            //if (!_isCoroutineRunning) StartCoroutine("Patrol");
    //            BaseMove();
    //            break;

    //        case EnemyState.CHASE:
             
    //            Debug.Log(GetEnemyState());
    //            _agent.speed = _enemyData.speed * _enemyData.speedMultiplier;
    //            ChaseMove();
    //            break;
                
    //    }

    //}

    #region SearchStatus
    //public void SearchMove()
    //{
    //    if (_enemyVisionRay._wasPlayerInSight && !_enemyVisionRay.playerInSight) 
    //    {
    //        if (_enemyVisionRay.playerInSight)
    //        {
    //            this.SetEnemyState(EnemyState.CHASE);
    //            return;
    //        }
    //        _agent.ResetPath();
    //        if (!_isCoroutineRunning) StartCoroutine("SearchPlayer"); //non mi sembra una grande idea mettere una coroutine nell'update
    //        _agent.SetDestination(_enemyVisionRay._lastKnownPos.transform.position);
    //    }
    //    else if (_enemyVisionRay.playerInSight) this.SetEnemyState(EnemyState.CHASE);
    //}

    //public IEnumerator SearchPlayer()
    //{
    //    if (_isCoroutineRunning) yield break;
    //    _isCoroutineRunning = true;

    //    while (!_enemyVisionRay.playerInSight)
    //    {
    //        if (!_enemyVisionRay._wasPlayerInSight) break;

    //        yield return new WaitForSeconds(_enemyData.coroutineTime);

    //        if (!_enemyVisionRay.CheckForPlayer())
    //        {
    //            _enemyVisionRay.SetWasPlayerInSight(_enemyVisionRay.playerInSight);
    //            this.SetEnemyState(EnemyState.BASE);
    //        }
    //        break;
    //    }

    //    _isCoroutineRunning = false;
    //}

    #endregion

    #region ChaseStatus
    //public void ChaseMove()
    //{
    //    if (!_enemyVisionRay.playerInSight)
    //    {
    //        if (_enemyVisionRay._wasPlayerInSight)
    //        {
    //            timer += Time.deltaTime;
    //            if (timer >= _enemyData.lastSeenTime)
    //            {
    //                timer = 0f;
    //                _enemyVisionRay.SetWasPlayerInSight(false);
    //                SetEnemyState(EnemyState.SEARCH);
    //                return;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        timer = 0f;
    //        _enemyVisionRay.SetWasPlayerInSight(true);
    //        if (_enemyVisionRay._playerTransform != null)
    //        {
    //            _agent.SetDestination(_enemyVisionRay._playerTransform.position);
    //        }
    //    }
    //}
    #endregion

    #region PatrolStatus

    public override void BaseMove()
    {
        if (!_isCoroutineRunning) StartCoroutine("Patrol");
    }
    public void PatrolMove()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        }
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position);
    }

    public IEnumerator Patrol()
    {
        if (_isCoroutineRunning) yield break;
        _isCoroutineRunning = true;
        while (GetEnemyState() == EnemyState.BASE)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                yield return new WaitForSeconds(_enemyData.coroutineTime);
                PatrolMove();
            }
            yield return null;
        }
        _isCoroutineRunning = false;
    }
    #endregion


}
