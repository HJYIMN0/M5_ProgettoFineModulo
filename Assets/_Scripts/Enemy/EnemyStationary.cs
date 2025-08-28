using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStationary : AbstractEnemy
{

    [SerializeField] private Transform startPos;

    private Quaternion _targetRotation;

    private bool _isMoving;
    private bool _canRotate;

    public override void Setup()
    {
        if (startPos == null)
        {
            Debug.Log("Start position is not assigned for the stationary enemy.");
            return;
        }
        base.Setup();        
    }

    public override void Update()
    {
        base.Update();
        // Se stiamo ruotando, interpoliamo la rotazione
        if (_canRotate)
        {
            Rotate();
        }
        Interact();
    }

    public void Rotate()
    {
        if (!_canRotate) return;

        transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        _targetRotation,
        _enemyData.rotationSpeed * Time.deltaTime // rotationSpeed in gradi/sec dentro SO_EnemyData
);

        if (Quaternion.Angle(transform.rotation, _targetRotation) < 0.5f)
        {
            _canRotate = false;
        }
    }
    public override void BaseMove()
    {
        if (!_isCoroutineRunning)
        {
            StartCoroutine("Stationary");
        }
    }

    public IEnumerator Stationary()
    {
        if (_isCoroutineRunning) yield break;
        _isCoroutineRunning = true;

        // Se non siamo abbastanza vicini allo startPos → muoviti lì
        if (Vector3.Distance(transform.position, startPos.position) > _enemyData.acceptedDistance)
        {
            _isMoving = true;
            _agent.stoppingDistance = _enemyData.acceptedDistance;
            _agent.SetDestination(startPos.position);

            // Aspetta che arrivi
            yield return new WaitUntil(() => !_agent.pathPending &&
                                             _agent.remainingDistance <= _agent.stoppingDistance + 0.05f);

            _isMoving = false;
        }
        else
        {
            _isMoving = false;
        }

        Debug.Log($"isMoving = {_isMoving}");

        // Loop da fermo: ruota a intervalli
        while (_isCoroutineRunning && !_isMoving)
        {
            _agent.ResetPath();

            // attesa prima della rotazione
            yield return new WaitForSeconds(_enemyData.coroutineTime);

            // calcola direzione opposta e avvia rotazione graduale
            Vector3 newDir = -transform.forward;
            _targetRotation = Quaternion.LookRotation(newDir, Vector3.up);
            _canRotate = true;

            // aspetta che finisca la rotazione
            yield return new WaitUntil(() => !_canRotate);
        }

        _isCoroutineRunning = false;
    }
}