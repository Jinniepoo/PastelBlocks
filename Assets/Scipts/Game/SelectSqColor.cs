using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSqColor : MonoBehaviour
{
    public SquareTexture SqTexture;
    public bool updateImage = false;

    private void OnEnable()
    {
        UpdateSqColor();

        if (updateImage)
            GameEvents.UpdateSqColor += UpdateColor;
    }

    private void OnDisable()
    {
        if (updateImage)
            GameEvents.UpdateSqColor -= UpdateColor;
    }

    private void UpdateSqColor() //현재 점수에 따라서
    {
        foreach (var sqTxture in SqTexture.activeSqTxtures)
        {
            if (SqTexture.curColor == sqTxture.sqColor)
                GetComponent<Image>().sprite = sqTxture.texture;
        }
    }

    private void UpdateColor(Config.SqColor color)
    {
        foreach (var sqTxture in SqTexture.activeSqTxtures)
        {
            if (color == sqTxture.sqColor)
            {
                GetComponent<Image>().sprite = sqTxture.texture;
            }

        }
    }
}
