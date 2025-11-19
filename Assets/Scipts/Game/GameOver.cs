using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject gameOver;
    public GameObject lose;
    public GameObject newBestScore;
    
    void Start()
    {
        gameOver.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;

    }

    private void OnGameOver(bool _newBestScore)
    {
        gameOver.SetActive(true);
        lose.SetActive(false);
        newBestScore.SetActive(true);
    }

}
