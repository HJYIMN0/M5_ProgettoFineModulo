using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private PlayerLife _playerLife; // Riferimento da assegnare nell'Inspector
    private CanvasGroup canvasGroup;

    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Button _retryButton;

    [SerializeField] private bool _isInteractable = false;

    private void Start()
    {
        // Prova prima il riferimento dall'Inspector
        if (_playerLife == null)
        {
            _playerLife = FindAnyObjectByType<PlayerLife>();
        }

        if (_playerLife != null)
        {
            _playerLife.OnPlayerDeath += PlayerDeathMenu;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on UiManager GameObject.");
        }
        else
        {
            canvasGroup.interactable = _isInteractable;
            canvasGroup.blocksRaycasts = _isInteractable;
            canvasGroup.alpha = _isInteractable ? 1f : 0f;
        }
    }

    private void OnDestroy()
    {
        if (_playerLife != null)
        {
            _playerLife.OnLifeChanged -= UpdateUi;
            _playerLife.OnPlayerDeath -= PlayerDeathMenu;
        }
    }

    public void PlayerDeathMenu()
    {
        StopAllCoroutines();
        StartCoroutine("ShowDeathMenu");
        canvasGroup.interactable = true;
        if (_retryButton != null)
        {
            if (_playerLife != null && !_playerLife.isAlive)
            {
                _retryButton.interactable = false;
            }
            else if (_playerLife != null && _playerLife.isAlive)
            {
                _retryButton.interactable = true;
            }
        }
    }

    public IEnumerator ShowDeathMenu()
    {
        // Fade in fino a alpha = 1
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed; // Usa unscaledDeltaTime per funzionare anche con timeScale = 0
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        Time.timeScale = 0f; // Metti in pausa il gioco
    }

    public void UpdateUi(int currentLife)
    {
        Debug.Log("Player Life: " + currentLife);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        SceneManager.LoadScene("MainMenu");
    }

    // Metodo di test per verificare se i button funzionano
    public void TestMethod()
    {
        Debug.Log("TestMethod called!");
        Debug.Log("Time.timeScale: " + Time.timeScale);
        Debug.Log("CanvasGroup interactable: " + (canvasGroup != null ? canvasGroup.interactable.ToString() : "null"));
        Debug.Log("CanvasGroup blocksRaycasts: " + (canvasGroup != null ? canvasGroup.blocksRaycasts.ToString() : "null"));
    }
}
