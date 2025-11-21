using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private ShapeStorage shapeStorage;
    [SerializeField]
    private GameObject gridSquare;

    public int cols = 0;
    public int rows = 0;
    public float sqGap = 0.1f;

    public Vector2 startPos = new Vector2(0.0f, 0.0f);
    public float sqScale = 0.5f;
    public float sqOffset = 0.0f;
    public SquareTexture sqTexture;

    private Vector2 offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> gridSquares = new List<GameObject>();

    private GridLines gridLines;
    private Config.SqColor curSqColor = Config.SqColor.None;
    private List<Config.SqColor> colorsInGrid = new List<Config.SqColor>();

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.UpdateSqColor += OnSqColorUpdate;
    }
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.UpdateSqColor -= OnSqColorUpdate;
    }
    void Start()
    {
        gridLines = GetComponent<GridLines>();
        CreateGrid();
        curSqColor = sqTexture.activeSqTxtures[0].sqColor;
    }

    private void OnSqColorUpdate(Config.SqColor color)
    {
        curSqColor = color;
    }

    private List<Config.SqColor> GetAllSqColorsInGrid()
    {
        var colors = new List<Config.SqColor>();

        foreach (var sq in gridSquares)
        {
            var gridSq = sq.GetComponent<GridSquare>();

            if (gridSq.SquareOccupied)
            {
                var color = gridSq.GetCurColor();
                if (colors.Contains(color) == false)
                    colors.Add(color);


            }
        }
        return colors;
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridPos();
    }

    private void SpawnGridSquares()
    {
        int sqIdx = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                gridSquares.Add(Instantiate(gridSquare) as GameObject);
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SquareIdx = sqIdx;
                gridSquares[gridSquares.Count - 1].transform.SetParent(this.transform);
                gridSquares[gridSquares.Count - 1].transform.localScale = new Vector3(sqScale, sqScale, sqScale);
                gridSquares[gridSquares.Count - 1].transform.GetComponent<GridSquare>().SetImage(gridLines.GetGridSqIdx(sqIdx) % 2 == 0);
                sqIdx++;
            }
        }
    }

    private void SetGridPos()
    {
        int colNum = 0;
        int rowNum = 0;
        Vector2 sqGapNum = new Vector2(0.0f, 0.0f);
        bool rowMoved = false;

        var sqRect = gridSquares[0].GetComponent<RectTransform>();

        offset.x = sqRect.rect.width * sqRect.transform.localScale.x + sqOffset;
        offset.y = sqRect.rect.height * sqRect.transform.localScale.y + sqOffset;

        foreach (GameObject sq in gridSquares)
        {
            if (colNum + 1 > cols)
            {
                sqGapNum.x = 0;
                colNum = 0;
                rowNum++;
                rowMoved = true;
            }

            var posX_Offset = offset.x * colNum + (sqGapNum.x * sqGap);
            var posY_Offset = offset.y * rowNum + (sqGapNum.y * sqGap);

            if (colNum > 0 && colNum % 3 == 0)
            {
                sqGapNum.x++;
                posX_Offset += sqGap;
            }

            if (colNum > 0 && colNum % 3 == 0)
            {
                sqGapNum.x++;
                posX_Offset += sqGap;
            }
            if (rowNum > 0 && colNum % 3 == 0 && rowMoved == false)
            {
                rowMoved = true;
                sqGapNum.y++;
                posY_Offset += sqGap;
            }

            sq.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x + posX_Offset, startPos.y - posY_Offset);
            sq.GetComponent<RectTransform>().localPosition = new Vector3(startPos.x + posX_Offset, startPos.y - posY_Offset, 0.0f);

            colNum++;
        }
    }

    private void CheckIfShapeCanBePlaced()
    {
        var sqIdxs = new List<int>();

        foreach (var square in gridSquares)
        {
            var gridSq = square.GetComponent<GridSquare>();

            if (gridSq.Selected && !gridSq.SquareOccupied)
            {
                sqIdxs.Add(gridSq.SquareIdx);
                gridSq.Selected = false;
            }
        }

        var curSelectedShape = shapeStorage.GetCurSelectedShape();
        if (curSelectedShape == null) return; //선택된거 없을때

        if (curSelectedShape.totalSqNum == sqIdxs.Count)
        {
            foreach (var sqIdx in sqIdxs)
            {
                gridSquares[sqIdx].GetComponent<GridSquare>().PlaceShapeOnBoard(curSqColor);
            }

            var shapeLeft = 0;
            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPos() && shape.IsAnyOfShapeSqActive())
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShape();
            }

            else
            {
                GameEvents.SetShapeInactive();
            }
            Check_AnyLineCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPos();
        }
    }

    public void Check_AnyLineCompleted()
    {
        List<int[]> lines = new List<int[]>();

        //col
        foreach (var col in gridLines.colIdx)
        {
            lines.Add(gridLines.GetVerticalLine(col));
        }

        //row
        for (int row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var idx = 0; idx < 9; idx++)
            {
                data.Add(gridLines.lineData[row, idx]);
            }

            lines.Add(data.ToArray());
        }

        //네모
        for (int sq = 0; sq < 9; sq++)
        {
            List<int> data = new List<int>(9);
            for (var idx = 0; idx < 9; idx++)
            {
                data.Add(gridLines.sqData[sq, idx]);
            }

            lines.Add(data.ToArray());
        }

        colorsInGrid = GetAllSqColorsInGrid();

        var completedLines = Check_SquaresCompleted(lines);

        if (completedLines >= 2)
        {
            GameEvents.ShowMessage();
        }

        var totalScore = 10 * completedLines;
        var bonusScores = PlayColorBonus();
        GameEvents.AddScores(totalScore + bonusScores);
        Check_PlayerLost();
    }

    private int PlayColorBonus()
    {
        var colorsInGrid = GetAllSqColorsInGrid();
        Config.SqColor color = Config.SqColor.None;

        foreach (var col in colorsInGrid)
        {
            if (colorsInGrid.Contains(col) == false)
                color = col;
        }

        if (color == Config.SqColor.None)
        {
            Debug.Log("Can't find color for bonus");
            return 0;
        }

        if (color == curSqColor)
        {
            return 0;
        }
        GameEvents.ShowBonusScreen(color);

        return 50;
    }

    private int Check_SquaresCompleted(List<int[]> data)
    {
        List<int[]> completedSquares = new List<int[]>();

        var sqsCompleted = 0;

        foreach (var sq in data)
        {
            var sqCompleted = true;
            foreach (var sqIdx in sq)
            {
                var comp = gridSquares[sqIdx].GetComponent<GridSquare>();
                if (!comp.SquareOccupied)
                {
                    sqCompleted = false;
                }
            }

            if (sqCompleted)
            {
                completedSquares.Add(sq);
            }
        }

        foreach (var sq in completedSquares)
        {
            var completed = false;

            foreach (var sqIdx in sq)
            {
                var comp = gridSquares[sqIdx].GetComponent<GridSquare>();
                comp.DeactivateSquare();
                completed = true;
            }

            foreach (var sqIdx in sq)
            {
                var comp = gridSquares[sqIdx].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                sqsCompleted++;
            }
        }
        return sqsCompleted;
    }

    private void Check_PlayerLost()
    {
        var validShapes = 0;

        for (var idx = 0; idx < shapeStorage.shapeList.Count; idx++)
        {
            var isShapeActive = shapeStorage.shapeList[idx].IsAnyOfShapeSqActive();

            if (Check_ShapeCanBePlacedOnGrid(shapeStorage.shapeList[idx]) && isShapeActive)
            {
                shapeStorage.shapeList[idx]?.ActivateShape();
                validShapes++;
            }
        }

        if (validShapes == 0)
        {
            GameEvents.GameOver(false);
            //Debug.Log("Game Over");
        }
    }
    private bool Check_ShapeCanBePlacedOnGrid(Shape curShape)
    {
        var curShapeData = curShape.curShapeData;
        var shapeCols = curShapeData.cols;
        var shapeRows = curShapeData.rows;

        //채워진 사각형의 모든 idx
        List<int> origShapeFilledUpSqs = new List<int>();
        var sqIdx = 0;

        for (var rowIdx = 0; rowIdx < shapeRows; rowIdx++)
        {
            for (var colIdx = 0; colIdx < shapeCols; colIdx++)
            {
                if (curShapeData.board[rowIdx].col[colIdx])
                {
                    origShapeFilledUpSqs.Add(sqIdx);
                }
                sqIdx++;
            }
        }

        if (curShape.totalSqNum != origShapeFilledUpSqs.Count)
            Debug.LogError("No. of filled squares != original shape");

        var sqList = GetAllSqCombination(shapeCols, shapeRows);

        bool canBePlaced = false;

        foreach (var num in sqList)
        {
            bool shapeCanBePlaced = true;
            foreach (var sqIdxToCheck in origShapeFilledUpSqs)
            {
                var comp = gridSquares[num[sqIdxToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    shapeCanBePlaced = false;
                }
            }

            if (shapeCanBePlaced)
            {
                canBePlaced = true;
            }
        }

        return canBePlaced;
    }

    private List<int[]> GetAllSqCombination(int cols, int rows)
    {
        var sqList = new List<int[]>();
        var lastColIdx = 0;
        var lastRowIdx = 0;

        int safeIdx = 0;

        while (lastRowIdx + (rows - 1) < 9)
        {
            var rowData = new List<int>();

            for (var row = lastRowIdx; row < lastRowIdx + rows; row++)
            {
                for (var col = lastColIdx; col < lastColIdx + cols; col++)
                {
                    rowData.Add(gridLines.lineData[row, col]);
                }
            }

            sqList.Add(rowData.ToArray());
            lastColIdx++;

            if (lastColIdx + (cols - 1) >= 9)
            {
                lastRowIdx++;
                lastColIdx = 0;
            }

            safeIdx++;

            if (safeIdx > 100)
                break;
        }

        return sqList;
    }
}
