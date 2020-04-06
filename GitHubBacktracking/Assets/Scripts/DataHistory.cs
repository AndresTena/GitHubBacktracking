using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHistory
{
    public int[,] board;
    public int fil;
    public int col;
    public int num;
    public bool diag1Ok, diag2Ok, colOk,filOk,cuadroOk,numeroBase,esSol;
    public string colorDiag1 = "Black";
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

    public DataHistory(int[,] sudoku, int fil, int col, int num, bool numeroBase, bool esSol, bool filOk=true, bool colOk=true, bool cuadroOk=true)
    {
        this.board = sudoku;
        this.fil = fil;
        this.col = col;
        this.num = num;
        this.filOk = filOk;
        this.colOk = colOk;
        this.cuadroOk = cuadroOk;
        this.numeroBase = numeroBase;
        this.esSol = esSol;

    }
}
