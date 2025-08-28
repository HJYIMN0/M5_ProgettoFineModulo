using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canva;

    private void Start()
    {
        _canva.alpha = 0;
        _canva.interactable = false;
    }
    private void OnTriggerEnter(Collider other)
    {        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the goal! You win!");
            _canva.alpha = 1;
            _canva.interactable = true;
            _canva.blocksRaycasts = true;
            Destroy(other.gameObject);
        }
    }
}
