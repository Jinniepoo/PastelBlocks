using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class ShapeData : ScriptableObject
{

    [System.Serializable]
    public class Row
    {
        public bool[] col;
        private int size = 0;
        public Row() { }

        public Row(int _size)
        {
            CreateRow(_size);
        }


        public void CreateRow(int _size)
        {
            size = _size;
            col = new bool[size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (int i = 0; i < size; i++)
            {
                col[i] = false;
            }
        }
    }

    public int cols = 0;
    public int rows = 0;

    public Row[] board;

    public void Clear()
    {
        for(var i = 0; i < rows; i++)
        {
            board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        board = new Row[rows];

        for (var i = 0; i < rows; i++)
        {
            board[i] = new Row(cols);
        }
    }
}
