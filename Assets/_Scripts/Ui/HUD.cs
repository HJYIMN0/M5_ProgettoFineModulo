using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [SerializeField] private Image[] _images;
    [SerializeField] private PlayerLife _playerLife;

    public void ChangeColor(int value)
    {
        if (value == 0) return;
        if (value > _images.Length) return;

        foreach (Image image in _images)
        {
            if (image.color != Color.red)
            {
                image.color = Color.red;
                return;
            }
        }
    }

    private void Start()
    {
        // Prova prima il riferimento dall'Inspector
        if (_playerLife == null)
        {
            _playerLife = FindAnyObjectByType<PlayerLife>();
        }
        _playerLife.OnLifeChanged += ChangeColor;
    }

    private void OnDestroy()
    {
        if (_playerLife != null)
        {
            _playerLife.OnLifeChanged -= ChangeColor;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("o")) ChangeColor(1);
    }
    //[SerializeField] private Image[] _images;

    //private PlayerLife PlayerLifeInstance;

    //public void ChangeColor(int value)
    //{
    //    if (value == 0) return;
    //    if (value > _images.Length) return;

    //    foreach (Image image in _images) 
    //    {
    //        if (image.color != Color.red)
    //        {
    //            image.color = Color.red;
    //            return;                
    //        }
    //    }        
    //}

    //private void Start()
    //{
    //    //if (PlayerLifeInstance.Instance.maxHpOnStart)
    //    if (PlayerLifeInstance == null) PlayerLifeInstance = PlayerLife.playerLifeInstance;
    //    if (PlayerLifeInstance.maxHpOnStart)
    //    {
    //        foreach (Image image in _images)
    //        {
    //            image.color = Color.green;

    //        }
    //    }

    //    PlayerLifeInstance.OnLifeChanged += ChangeColor;
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown("o")) ChangeColor(1);  
    //}
}
