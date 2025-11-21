using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class SquareTexture : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public Config.SqColor sqColor;
    }

    public int thresholdVal;
    private const int StartTHVal = 100;
    public List<TextureData> activeSqTxtures;

    public Config.SqColor curColor;
    private Config.SqColor nextColor;

    private int GetCurColorIdx()
    {
        var curIdx = 0;

        for (int idx = 0; idx < activeSqTxtures.Count; idx++)
        {
            if (activeSqTxtures[idx].sqColor == curColor)
            {
                curIdx = idx;
            }
        }
        return curIdx;
    }

    public void UpdateColors(int curScore)
    {
        curColor = nextColor;
        var curColorIdx = GetCurColorIdx();

        if (curColorIdx == activeSqTxtures.Count - 1)
        {
            nextColor = activeSqTxtures[0].sqColor;
        }
        else
        {
            nextColor = activeSqTxtures[curColorIdx + 1].sqColor;
        }
        thresholdVal = StartTHVal + curScore;
    }

    public void SetStartColor()
    {
        thresholdVal = StartTHVal;
        curColor = activeSqTxtures[0].sqColor;
        nextColor = activeSqTxtures[1].sqColor;
    }

    private void Awake()
    {
        SetStartColor();
    }
    private void OnEnable()
    {
        SetStartColor();
    }
}
