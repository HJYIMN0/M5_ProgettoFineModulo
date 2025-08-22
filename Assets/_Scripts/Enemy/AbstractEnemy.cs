using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
public abstract class AbstractEnemy : MonoBehaviour, iInteractable
{
    #region Variables
    public enum EnemyState
    {
        BASE = 0,
        CHASE = 2,
        SEARCH = 3,
    }

    protected float timer = 0f;
    protected bool _isCoroutineRunning = false;


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

    public Action OnTouchedPlayer;

    #endregion

    public virtual void Setup()
    {
        if (PlayerLife.playerLifeInstance != null)
        {
            OnTouchedPlayer += PlayerLife.playerLifeInstance.LoseLife;
        }
        else {
            Debug.LogError("PlayerLife instance not found. Ensure there is a PlayerLife script in the scene."); 
        }

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

    #region MoveSection

    public virtual void Move()
    {
        switch (GetEnemyState())
        {
            case EnemyState.SEARCH:

                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed;
                SearchMove();

                break;

            case EnemyState.BASE:
                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed;
                BaseMove();
                //if (_patrolPoints.Length <= 0 || _patrolPoints == null)
                //{
                //    Debug.LogError("No patrol points assigned for the enemy.");
                //    return;
                //}
                ////PatrolMove();
                //if (!_isCoroutineRunning) StartCoroutine("Patrol");
                break;

            case EnemyState.CHASE:

                Debug.Log(GetEnemyState());
                _agent.speed = _enemyData.speed * _enemyData.speedMultiplier;
                ChaseMove();
                break;

        }


    }
    public abstract void BaseMove();

    public virtual void ChaseMove()
    {
        if (!_enemyVisionRay.playerInSight)
        {
            if (_enemyVisionRay._wasPlayerInSight)
            {
                timer += Time.deltaTime;
                if (timer >= _enemyData.lastSeenTime)
                {
                    timer = 0f;
                    _enemyVisionRay.SetWasPlayerInSight(false);
                    SetEnemyState(EnemyState.SEARCH);
                    return;
                }
            }
        }
        else
        {
            timer = 0f;
            _enemyVisionRay.SetWasPlayerInSight(true);
            if (_enemyVisionRay._playerTransform != null)
            {
                _agent.SetDestination(_enemyVisionRay._playerTransform.position);
            }
        }
    }

    public virtual void SearchMove()
    {
        if (_enemyVisionRay._wasPlayerInSight && !_enemyVisionRay.playerInSight)
        {
            if (_enemyVisionRay.playerInSight)
            {
                this.SetEnemyState(EnemyState.CHASE);
                return;
            }
            _agent.ResetPath();
            if (!_isCoroutineRunning) StartCoroutine("SearchPlayer"); //non mi sembra una grande idea mettere una coroutine nell'update
            _agent.SetDestination(_enemyVisionRay._lastKnownPos.transform.position);
        }
        else if (_enemyVisionRay.playerInSight) this.SetEnemyState(EnemyState.CHASE);
    }

    public IEnumerator SearchPlayer()
    {
        if (_isCoroutineRunning) yield break;
        _isCoroutineRunning = true;

        while (!_enemyVisionRay.playerInSight)
        {
            if (!_enemyVisionRay._wasPlayerInSight) break;

            yield return new WaitForSeconds(_enemyData.coroutineTime);

            if (!_enemyVisionRay.CheckForPlayer())
            {
                _enemyVisionRay.SetWasPlayerInSight(_enemyVisionRay.playerInSight);
                this.SetEnemyState(EnemyState.BASE);
            }
            break;
        }

        _isCoroutineRunning = false;
    }

    #endregion

    #region iInteractable

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(GizmoOrigin.position, _enemyData.interactionRadius);
        Gizmos.DrawWireCube(GizmoOrigin.position, (Vector3.one * _enemyData.interactionRadius) * 2);

    }
    public bool IsInRange()
    {
        //if (Physics.CheckSphere(GizmoOrigin.position, _enemyData.interactionRadius, _enemyData.playerMask))
        if (Physics.CheckBox(GizmoOrigin.position, (Vector3.one * _enemyData.interactionRadius), Quaternion.identity, _enemyData.playerMask))
            return true;
        else
            return false;
    }
    public void Interact()
    {
        //if (!_hasUiText) return;
        if (!IsInRange())
        {
            Debug.Log("Il player non è in range per interagire con il nemico.");
            return;
        }
        OnTouchedPlayer?.Invoke();
    }
    #endregion

    public virtual void Update()
    {
        Move();
    }

    public virtual void Awake()
    {
        Setup();
    }

    private void OnDestroy()
    {
        OnTouchedPlayer -= PlayerLife.playerLifeInstance.LoseLife;
    }


}
