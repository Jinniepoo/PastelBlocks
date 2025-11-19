using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<bool> GameOver;
    public static Action<int> AddScores;
    public static Action CheckIfShapeCanBePlaced;
    public static Action MoveShapeToStartPos;
    public static Action RequestNewShape;
    public static Action SetShapeInactive;

    public static Action<int, int> BestScoreUpdate;
}
