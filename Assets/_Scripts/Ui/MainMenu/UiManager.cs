using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    PlayerLife playerLifeInstance;
    CanvasGroup canvasGroup;

    [SerializeField] private float fadeSpeed = 1f;

    private void Start()
    {
        playerLifeInstance = PlayerLife.playerLifeInstance;
        if (playerLifeInstance != null)
        {
            playerLifeInstance.OnLifeChanged += UpdateUi;
            playerLifeInstance.OnPlayerDeath += PlayerDeathMenu;
        }
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component not found on UiManager GameObject.");
        }
    }

    public void PlayerDeathMenu()
    {
        StopAllCoroutines();
        StartCoroutine("ShowDeathMenu");
        Debug.Log("Starting to fade...");
        canvasGroup.interactable = true;
    }

    public IEnumerator ShowDeathMenu()
    {

        // Fade in fino a alpha = 1
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        Time.timeScale = 0f;
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
    }

}
