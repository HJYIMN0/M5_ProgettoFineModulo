using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonInteraction : MonoBehaviour
{

    public UnityEvent OnButtonPressed;

    private bool isNearby = false;
    public bool hasBeenInteracted { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenInteracted) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered in trigger area");
            isNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited from trigger area");
            isNearby = false;
        }
    }

    private void Update()
    {

        if (isNearby && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Button pressed");
            OnButtonPressed?.Invoke();
            hasBeenInteracted = true;
        }

    }
}
