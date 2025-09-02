using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogError("PlayerController component not found on the GameObject.");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
            _animator = GetComponentInChildren<Animator>();
            if (_animator != null)
            {
                Debug.Log("Animator found in children of the GameObject.");
            }
        }
    }

    private void Update()
    {
        _animator.SetBool("isMoving", _playerController.IsMoving);
    }

}
