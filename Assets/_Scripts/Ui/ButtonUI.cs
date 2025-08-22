using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canva;

    private ButtonInteraction _button;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_button.hasBeenInteracted)
        {
            _canva.alpha = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canva.alpha = 0f;
        }
    }

    private void Start()
    {
        _button = GetComponent<ButtonInteraction>();
        if (_button == null)
        {
            Debug.LogError("ButtonInteraction component not found on the GameObject.");
        }
    }

    private void Update()
    {
        if (_button.hasBeenInteracted)
        {
            _canva.alpha = 0f;
        }
    }
}
