using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BestScore : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI bestScoreText;
    private void OnEnable()
    {
        GameEvents.BestScoreUpdate += BestScoreUpdate;
    }

    private void OnDisable()
    {
        GameEvents.BestScoreUpdate -= BestScoreUpdate;
    }

    private void BestScoreUpdate(int curScore, int bestScore)
    {
        float curPercentage = (float)curScore / (float)bestScore;
        fillImage.fillAmount = curPercentage;
        bestScoreText.text = bestScore.ToString();
    }
}
