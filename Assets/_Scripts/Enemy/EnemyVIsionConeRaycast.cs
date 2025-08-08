using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVisionConeRaycast : MonoBehaviour
{
    [SerializeField] private SO_EnemyData _enemyData;

    [SerializeField] private LayerMask _obstructionMask;
    [SerializeField] private LayerMask _playerMask;

    [SerializeField] private float _radiusOrigin = 1f; // distanza dell'origine del raggio dall'occhio del nemico

    private AbstractEnemy _enemy;
    private LineRenderer _lineRenderer; 

    public bool playerInSight { get; private set; }

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
        CheckForPlayer();
        DrawVisionCone();

        if (playerInSight)
        {
            Debug.Log("Player avvistato!");
            _enemy.SetEnemyState(AbstractEnemy.EnemyState.CHASE); // Cambia lo stato del nemico a inseguimento
        }
        else if (!playerInSight && _wasPlayerInSight)
        {
            _enemy.SetEnemyState(AbstractEnemy.EnemyState.SEARCH); // se il player non è visto ma era visto, cambia stato a ricerca
        }
    }

    public bool CheckForPlayer()
    {
        //_wasPlayerInSight = playerInSight; // Salva lo stato precedente del player

        float angleStep = _enemyData.viewAngle / _enemyData.rayCount;
        float startAngle = -_enemyData.viewAngle / 2f;

        Vector3 origin = transform.position + Vector3.up * _radiusOrigin; // occhio del nemico

        for (int i = 0; i <= _enemyData.rayCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Raycast contro il mondo
            if (Physics.Raycast(origin, dir, out RaycastHit hit, _enemyData.viewRadius, _obstructionMask | _playerMask))
            {
                if (((1 << hit.collider.gameObject.layer) & _playerMask) != 0 && hit.collider.CompareTag("Player"))
                {
                    playerInSight = true;
                    _wasPlayerInSight = playerInSight;
                    _lastKnownPos = hit.collider.transform;
                    Debug.DrawLine(origin, hit.point, Color.red); // hit player
                    _lastKnownPos = hit.collider.transform; // aggiorna l'ultima posizione vista del player
                    return playerInSight; // non serve continuare, il player è visto
                }
                else
                {
                    Debug.DrawLine(origin, hit.point, Color.yellow); // ostacolo
                    playerInSight = false;  // se colpisce un ostacolo, il player non è visto
                }
                
            }

            else
            {
                Debug.DrawLine(origin, origin + dir * _enemyData.viewRadius, Color.gray); // niente colpito
                playerInSight = false; // se non colpisce nulla, il player non è visto
                return playerInSight;
            }
            
        }
        return playerInSight; // ritorna lo stato del player
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
}
