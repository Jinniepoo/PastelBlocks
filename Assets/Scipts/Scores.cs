using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int curScore;
    void Start()
    {
        curScore = 0;
        UpdateScoreTxt();
    }
    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
    }

    private void AddScores(int score)
    {
        curScore += score;
        UpdateScoreTxt();
    }
    
    private void UpdateScoreTxt()
    {
        scoreText.text = curScore.ToString();
    }

}
