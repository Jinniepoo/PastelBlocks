using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    private void OnEnable()
    {
        GameEvents.RequestNewShape += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShape = RequestNewShapes;
    }

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIdx = UnityEngine.Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIdx]);
        }
    }

    public Shape GetCurSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPos() == false && shape.IsAnyOfShapeSqActive())
                return shape;

        }

        Debug.LogError("No Shape Selected");
        return null;
    }  

    private void RequestNewShapes()
    {
        foreach (var shape in shapeList)
        {
            var shapeIdx = UnityEngine.Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIdx]);
        }
    }
}
