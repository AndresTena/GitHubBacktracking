using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHistory 
{
    public int[,] board;
    public int fil;
    public int col;
    public bool diag1Ok, diag2Ok, colOk;
    public string colorDiag1="Black";
    public string colorDiag2 = "Black";
    public string colorCol = "Black";
    public List<int> vectorSolucion = new List<int>();

    public DataHistory(int[,] board, int fil, int col, bool diag1Ok, bool diag2Ok, bool colOk)
    {
        this.board = board;
        this.fil = fil;
        this.col = col;
        this.diag1Ok = diag1Ok;
        this.diag2Ok = diag2Ok;
        this.colOk = colOk;

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(0); j++)
            {
                if (board[i, j] == 1)
                {
                    vectorSolucion.Add(j);
                }
            }
        }
    }
}
