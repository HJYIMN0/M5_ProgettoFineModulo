using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _livesOnStart = 3;

    public bool maxHpOnStart { get; private set; } = true;
    public bool isAlive = true;

    public Action<int> OnLifeChanged;
    public Action OnPlayerDeath;

    private void Awake()
    {
        // Rimuovi tutto il codice del singleton
        Debug.Log("PlayerLife Instance created");
    }

    public void LoseLife()
    {
        Debug.Log("Player lost a life!");
        this._lives--;
        _lives = Mathf.Max(0, _lives);
        Debug.Log(_lives);

        if (this._lives <= 0)
        {
            RestartGame();
        }
        else
        {
            RestartScene();
        }
        OnLifeChanged?.Invoke(this._lives);
    }

    public void GainLife(int amount)
    {
        this._lives += amount;
        _lives = Mathf.Min(_lives, _livesOnStart);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting...");
        isAlive = false;
        OnLifeChanged?.Invoke(_lives);
        OnPlayerDeath?.Invoke();
        Destroy(this.gameObject); // Distruggi l'istanza corrente
    }

    public void RestartScene()
    {
        Debug.Log("Restarting scene...");
        OnPlayerDeath?.Invoke();
        Destroy(this.gameObject); // Distruggi l'istanza corrente
    }

    public void GoBackToMainMenu()
    {
        Debug.Log("QuiMancaAncoraLaLogica!");
        //Scene...
    }



}
