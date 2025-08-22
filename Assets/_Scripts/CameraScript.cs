using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = new Vector3(0, 5, -10);

    [SerializeField] private Transform _player;

    private void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Player or PlayerController reference is missing in CameraScript.");
            return;
        }
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    public void UpdateCameraPosition()
    {
        transform.position = _player.transform.position + _offset;
    }

    
}
