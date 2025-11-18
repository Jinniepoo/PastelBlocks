using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq.Expressions;
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

    private Vector2 offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> gridSquares = new List<GameObject>();

    private GridLines gridLines;

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }
    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }
    void Start()
    {
        gridLines = GetComponent<GridLines>();
        CreateGrid();
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
                gridSquares[sqIdx].GetComponent<GridSquare>().PlaceShapeOnBoard();
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

        var completedLines = Check_SquaresCompleted(lines);

        if (completedLines > 2)
        {
            //bonus animation 추가하기
        }

        //점수 추가하기
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
}
