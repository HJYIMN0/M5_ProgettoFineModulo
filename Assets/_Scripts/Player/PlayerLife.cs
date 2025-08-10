using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _livesOnStart = 3;

    public static PlayerLife playerLifeInstance { get; private set; }

    public Action<int> OnLifeChanged;

    private void Awake()
    {
        if (playerLifeInstance != null && playerLifeInstance != this)
        {
            Destroy(playerLifeInstance.gameObject);
            return;
        }
        else        
        {
            Debug.Log("PlayerLife Instance created");
            playerLifeInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }

    }

    public void LoseLife()
    {
        this._lives--;
        _lives = Mathf.Max(0, _lives);

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

    }
    public void RestartScene()
    {
        _lives = _livesOnStart;
    }
    public void GoBackToMainMenu()
    {
        Debug.Log("QuiMancaAncoraLaLogica!");
        //Scene...
    }



}
