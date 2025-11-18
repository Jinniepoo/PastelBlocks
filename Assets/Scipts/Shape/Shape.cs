using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0, 700f);

    [HideInInspector]
    public ShapeData curShapeData;
    public int totalSqNum { get; set; }

    private List<GameObject> currentShape = new List<GameObject>();
    private Vector3 shapeStartScale;
    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPos += MoveShapeToStartPos;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPos -= MoveShapeToStartPos;
    }
    private RectTransform shapeTransform;
    private bool shapeDrag = true;
    private Canvas shapeCanvas;
    private Vector3 startPos;
    private bool activeShape = true;

    public void Awake()
    {
        shapeStartScale = this.GetComponent<RectTransform>().localScale;
        shapeTransform = this.GetComponent<RectTransform>();
        shapeCanvas = this.GetComponentInParent<Canvas>();
        shapeDrag = true;
        startPos = transform.localPosition; 
        activeShape = true;
    }

    public bool IsOnStartPos()
    {
        return shapeTransform.localPosition == startPos;
    }

    public bool IsAnyOfShapeSqActive()
    {
        foreach (var square in currentShape)
        {
            if (square.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void DeactivateShape()
    {
        if (activeShape)
        {
            foreach (var square in currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }
        activeShape = false;
    }

    public void ActivateShape()
    {
        if (!activeShape)
        {
            foreach (var square in currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }
        activeShape = true;
    }

    void Start()
    {
        
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        shapeTransform.localPosition = startPos;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        curShapeData = shapeData;

        totalSqNum = GetNumberOfSquares(shapeData);

        while (currentShape.Count <= totalSqNum)
        {
            currentShape.Add(Instantiate(squareImage, transform) as GameObject);
        }

        foreach (var square in currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareImage.GetComponent<RectTransform>();
        var moveDist = new Vector2(squareRect.rect.width * squareRect.localScale.x,
           squareRect.rect.height * squareRect.localScale.y);

        int curIdx = 0; //list¿¡¼­
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var col = 0; col < shapeData.cols; col++)
            {
                if (shapeData.board[row].col[col])
                {
                    currentShape[curIdx].SetActive(true);
                    currentShape[curIdx].GetComponent<RectTransform>().localPosition = 
                        new Vector2(GetXPositionForShapeSquare(shapeData, col, moveDist),
                        GetYPositionForShapeSquare(shapeData, row, moveDist));

                    curIdx++;
                }
            }
        }

    }
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDist)
    {
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var midSquareIdx = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < midSquareIdx)
                {
                    shiftOnY = moveDist.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > midSquareIdx)
                {
                    shiftOnY = moveDist.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var midSquareIdx2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var midSquareIdx1 = (shapeData.rows == 2) ? 0 : (shapeData.rows - 2);
                var multiplier = shapeData.rows / 2;

                if (row == midSquareIdx1 || row == midSquareIdx2)
                {
                    if (row == midSquareIdx2)
                        shiftOnY = (moveDist.y / 2) * -1;

                    if (row == midSquareIdx1)
                        shiftOnY = (moveDist.y / 2);
                }
                if (row < midSquareIdx1 && row < midSquareIdx2)
                {
                    shiftOnY = moveDist.x * -1;
                    shiftOnY *= multiplier;
                }
                else if (row > midSquareIdx1 && row > midSquareIdx2)
                {
                    shiftOnY = moveDist.x * 1;
                    shiftOnY *= multiplier;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int col, Vector2 moveDist)
    {
        float shiftOnX = 0f;

        if (shapeData.cols > 1)
        {
            if (shapeData.cols % 2 != 0)
            {
                var middleSquareIdx = (shapeData.cols - 1) / 2;
                var multiplier = (shapeData.cols - 1) / 2;
                if (col < middleSquareIdx)
                {
                    shiftOnX = moveDist.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (col > middleSquareIdx)
                {
                    shiftOnX = moveDist.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIdx2 = (shapeData.cols == 2) ? 1 : (shapeData.cols / 2);
                var middleSquareIdx1 = (shapeData.cols == 2) ? 0 : (shapeData.cols - 1);
                var multiplier = shapeData.cols / 2;

                if (col == middleSquareIdx2 || col == middleSquareIdx1)
                {
                    if (col == middleSquareIdx2)
                        shiftOnX = moveDist.x / 2;
                    if (col == middleSquareIdx1)
                        shiftOnX = (moveDist.x / 2) * -1;
                }

                if (col < middleSquareIdx1 && col < middleSquareIdx2)
                {
                    shiftOnX = moveDist.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (col > middleSquareIdx1 && col > middleSquareIdx2)
                {
                    shiftOnX = moveDist.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int num = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.col)
            {
                if (active)
                    num++;
            }
        }
        return num;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        shapeTransform.anchorMin = new Vector2(0, 0);
        shapeTransform.anchorMax = new Vector2(0, 0);
        shapeTransform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(shapeCanvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        shapeTransform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    private void MoveShapeToStartPos()
    {
        shapeTransform.transform.localPosition = startPos;
    }
}
