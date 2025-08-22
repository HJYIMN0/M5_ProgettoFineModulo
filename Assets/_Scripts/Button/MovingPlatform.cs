using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MovingPlatform : MonoBehaviour
{
    ButtonInteraction _buttonInteraction;
    private bool isMoving;
    private NavMeshSurface _navMesh;

    [SerializeField] private Transform _elevator;
    [SerializeField] private Transform finalPos;

    [SerializeField] private float acceptedImprecision;

    [SerializeField] private float speed = 2f;


    private void Awake()
    {
        _navMesh = GetComponent<NavMeshSurface>();
        if (_navMesh == null)
        {
            Debug.LogError("NavMeshSurface component not found on the GameObject.");
        }
    }
    private void Start()
    {
        isMoving = false;
        //_buttonInteraction.OnButtonPressed += MovePlatform;
        
    }

    public void MovePlatform()
    {
        Debug.Log("ELevator called!");
        //StartCoroutine("Elevator");
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            if (Mathf.Abs(_elevator.transform.position.y - finalPos.transform.position.y) >= acceptedImprecision)
            {
                Vector3 newPos = _elevator.transform.position;

                newPos.y = Mathf.MoveTowards(_elevator.transform.position.y, finalPos.transform.position.y, speed * Time.deltaTime);

                _elevator.transform.position = newPos;
            }
            else
            {
                isMoving = false;
                _navMesh.BuildNavMesh();
                Debug.Log("Platform reached final position");
            }
        }
    }

    //public IEnumerator Elevator()
    //{
    //    while (Vector3.Distance(transform.position, finalPos.transform.position) > acceptedImprecision)
    //    {            
    //        transform.position = Vector3.MoveTowards(transform.position, finalPos.transform.position, 2f * Time.deltaTime);
    //    }
    //    yield return null;
    //}
}
