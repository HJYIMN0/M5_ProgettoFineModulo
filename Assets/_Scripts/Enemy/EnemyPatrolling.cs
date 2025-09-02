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
