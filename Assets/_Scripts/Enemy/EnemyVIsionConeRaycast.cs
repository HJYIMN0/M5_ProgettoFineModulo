using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVisionConeRaycast : MonoBehaviour
{
    [SerializeField] private SO_EnemyData _enemyData;

    //[SerializeField] private LayerMask _obstructionMask;
    //[SerializeField] private LayerMask _playerMask;

    [SerializeField] private float _radiusOrigin = 1f; // distanza dell'origine del raggio dall'occhio del nemico


    private AbstractEnemy _enemy;
    private LineRenderer _lineRenderer;

    public Transform _playerTransform { get; private set; } // riferimento al player, se visto

    public bool playerInSight { get; private set; }

    private float timer;

    public void SetPlayerInSight(bool value)
    {
        if (!CheckForPlayer())
            playerInSight = true;
        else
            playerInSight = value;
    }


    public bool _wasPlayerInSight { get; private set; } // per tenere traccia se il player era visto nell'ultimo frame
    public void SetWasPlayerInSight(bool value)
    {
        if (playerInSight) return;
        _wasPlayerInSight = value;
    }

    public Transform _lastKnownPos { get; private set; } // ultima posizione conosciuta del player

    private void Awake()
    {
        _enemy = GetComponent<AbstractEnemy>();
        if (_enemy == null)
        {
            Debug.LogError("AbstractEnemy component is missing on EnemyVisionConeRaycast.");
        }

        #region LineRenderer Setup

        _lineRenderer = GetComponent<LineRenderer>();

        if (_lineRenderer == null)
        {
            Debug.LogError("LineRenderer component is missing on EnemyVisionCone.");
            return;
        }
        _lineRenderer.positionCount = _enemyData.rayCount + 2;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = true;

        #endregion
    }

    private void Start()
    {
        if (_enemyData == null)
        {
            Debug.LogError("Enemy data is not assigned.");
            return;
        }
    }

    void Update()
    {
        //CheckForPlayer();
        DrawVisionCone();
        _enemy.SetEnemyState(EvaluatEnemyState()); // Aggiorna lo stato del nemico in base alla visione del player


        //if (CheckForPlayer())
        //{
        //    Debug.Log("Player avvistato!");
        //    _enemy.SetEnemyState(AbstractEnemy.EnemyState.CHASE); // Cambia lo stato del nemico a inseguimento
        //}
        //else if (!playerInSight && _wasPlayerInSight)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= _enemyData.lastSeenTime && !playerInSight)
        //    {
        //        timer = 0f; // resetta il timer
        //        _enemy.SetEnemyState(AbstractEnemy.EnemyState.SEARCH); // se il player non è visto ma era visto, cambia stato a ricerca
        //    }
        //    else if (timer >= _enemyData.lastSeenTime && playerInSight)
        //    {
        //        _enemy.SetEnemyState(AbstractEnemy.EnemyState.CHASE);
        //    }
        //}
    }
    public bool CheckForPlayer()
    {
        float angleStep = _enemyData.viewAngle / _enemyData.rayCount;
        float startAngle = -_enemyData.viewAngle / 2f;
        Vector3 origin = transform.position + Vector3.up * _radiusOrigin;

        bool seen = false;
        Transform playerHit = null;

        for (int i = 0; i <= _enemyData.rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, _enemyData.viewRadius, _enemyData.obstructionMask | _enemyData.playerMask))
            {
                if (((1 << hit.collider.gameObject.layer) & _enemyData.playerMask) != 0 && hit.collider.CompareTag("Player"))
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    seen = true;
                    playerHit = hit.collider.transform;
                    break;
                }
                else
                {
                    Debug.DrawLine(origin, hit.point, Color.yellow);
                }
            }
            else
            {
                Debug.DrawLine(origin, origin + dir * _enemyData.viewRadius, Color.gray);
            }
        }

        playerInSight = seen;
        if (seen)
        {
            _wasPlayerInSight = true;
            _playerTransform = playerHit;
            _lastKnownPos = playerHit;
        }
        else
        {
            _playerTransform = null;
        }

        return playerInSight;
    }

    public void DrawVisionCone()
    {
        float stepAngle = _enemyData.viewAngle / _enemyData.rayCount;
        float startAngle = -_enemyData.viewAngle / 2;
        float endAngle = startAngle;
        _lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i <= _enemyData.rayCount; i++)
        {
            float angle = startAngle + stepAngle * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 point = transform.position + dir.normalized * _enemyData.viewRadius;

            _lineRenderer.SetPosition(i + 1, point);
        }
    }

    public AbstractEnemy.EnemyState EvaluatEnemyState()
    {
        CheckForPlayer(); // Aggiorna lo stato del player
        if (playerInSight)
        {
            return AbstractEnemy.EnemyState.CHASE; // Se il player è visto, cambia stato a inseguimento
        }
        else if (!playerInSight && _wasPlayerInSight)
        {
            return AbstractEnemy.EnemyState.SEARCH; // Se il player non è visto ma era visto, cambia stato a ricerca
        }
        return AbstractEnemy.EnemyState.BASE; // Altrimenti, torna allo stato di pattuglia
    }
}
