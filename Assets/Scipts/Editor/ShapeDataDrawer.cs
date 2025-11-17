using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
   private ShapeData ShapeDataInstance => target as ShapeData;

    public override void OnInspectorGUI()
    {
        
    }

    private void ClearBoardButton()
    {

    }

    private void DrawColumnsInputFields()
    {

    }
}
