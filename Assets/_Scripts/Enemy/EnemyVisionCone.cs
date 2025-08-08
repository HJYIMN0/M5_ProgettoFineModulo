using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyVisionCone : MonoBehaviour
{
    [SerializeField] private SO_EnemyData _enemyData;
    [SerializeField] private AbstractEnemy _enemy;

    private LineRenderer _linerenderer;

    private Mesh visionMesh;
    private MeshCollider meshCollider;

    public Vector3 _lastKnownPos { get; private set; }

    void Awake()
    {
        _enemy = GetComponent<AbstractEnemy>();
        if ( _enemy == null)
        {
            Debug.LogError("AbstractEnemy component is missing on EnemyVisionCone.");
        }

        #region LineRenderer Setup

        _linerenderer = GetComponent<LineRenderer>();

        if (_linerenderer == null)
        {
            Debug.LogError("LineRenderer component is missing on EnemyVisionCone.");
            return;
        }
        _linerenderer.positionCount = _enemyData.rayCount + 2;
        _linerenderer.useWorldSpace = true;
        _linerenderer.loop = true;

        #endregion

        #region Mesh Setup        
        visionMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = visionMesh;

        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        #endregion
    }

    public void Start()
    {
        DrawVisionCone();
        GenerateVisionMesh();
    }
    public void Update()
    {
        DrawVisionCone();
    }

    void DrawVisionCone()
    {
        float stepAngle = _enemyData.viewAngle / _enemyData.rayCount;
        float startAngle = - _enemyData.viewAngle / 2;
        float endAngle = startAngle;
        _linerenderer.SetPosition(0, transform.position);

        for (int i = 0; i <= _enemyData.rayCount; i++)
        {
            float angle = startAngle + stepAngle * i;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 point = transform.position + dir.normalized * _enemyData.viewRadius;

            _linerenderer.SetPosition(i + 1, point);
        }
    }

    public void GenerateVisionMesh()
    {
        Vector3[] vertices = new Vector3[_enemyData.rayCount + 2];
        int[] triangles = new int[_enemyData.rayCount * 3];

        vertices[0] = Vector3.zero; // centro

        float angleStep = _enemyData.viewAngle / _enemyData.rayCount;
        float startAngle = -_enemyData.viewAngle / 2f;

        for (int i = 0; i <= _enemyData.rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            vertices[i + 1] = dir.normalized * _enemyData.viewRadius;
        }

        for (int i = 0; i < _enemyData.rayCount; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
        visionMesh.RecalculateBounds();

        meshCollider.sharedMesh = null; // forzare aggiornamento
        meshCollider.sharedMesh = visionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter chiamato con: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrato nel cono visivo!");
            _enemy.SetEnemyState(AbstractEnemy.EnemyState.CHASE);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit chiamato con: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player uscito dal cono visivo!");
            _enemy.SetEnemyState(AbstractEnemy.EnemyState.SEARCH);
            _lastKnownPos = other.transform.position;
        }
    }
}
