using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}

public class Scores : MonoBehaviour
{
    public SquareTexture sqTexture;
    public TextMeshProUGUI scoreText;
    private bool newBestScore = false;
    private BestScoreData bestScores = new BestScoreData();
    private int curScore;

    private string bestScoreKey = "HighestScore_Data";

    private void Awake()
    {
        if (BinaryData.Exist(bestScoreKey))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScores = BinaryData.Read<BestScoreData>(bestScoreKey);
        yield return new WaitForEndOfFrame();
        GameEvents.BestScoreUpdate(curScore, bestScores.score);
        Debug.Log("[ReadDataFile] HighestScore:" + bestScores.score);
    }

    void Start()
    {
        curScore = 0;
        newBestScore = false;
        sqTexture.SetStartColor();
        UpdateScoreTxt();
    }
    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
        GameEvents.GameOver += SaveBestScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
        GameEvents.GameOver -= SaveBestScores;
    }

    private void AddScores(int score)
    {
        curScore += score;

        if (curScore > bestScores.score)
        {
            newBestScore = true;
            bestScores.score = curScore;
            SaveBestScores(true);
        }
        UpdateSqColor();
        GameEvents.BestScoreUpdate(curScore, bestScores.score);
        UpdateScoreTxt();
    }

    private void UpdateSqColor()
    {
        if (curScore >= sqTexture.thresholdVal)
        {
            sqTexture.UpdateColors(curScore);
        }
    }

    private void UpdateScoreTxt()
    {
        scoreText.text = curScore.ToString();
    }

    private void SaveBestScores(bool newBestScores)
    {
        BinaryData.Save<BestScoreData>(bestScores, bestScoreKey);
    }

}
