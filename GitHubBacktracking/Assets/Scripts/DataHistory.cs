using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHistory
{
    //Declaracion de las variables que utilizarán los distintos tableros para guardar sus valores correspondientes
    public int[,] board;
    public int fil;
    public int col;
    public int num, k, dir;
    public bool diag1Ok, diag2Ok, colOk, filOk, cuadroOk, numeroBase, esSol, esFactible;
    public string colorDiag1 = "Black";
    public string colorDiag2 = "Black";
    public string colorCol = "Black";
    public List<int> vectorSolucion = new List<int>();

    //Constructor utilizado por del NReinas backtracking
    public DataHistory(int[,] board, int fil, int col, bool diag1Ok, bool diag2Ok, bool colOk, bool esSol = false)
    {
        this.board = board;
        this.fil = fil;
        this.col = col;
        this.diag1Ok = diag1Ok;
        this.diag2Ok = diag2Ok;
        this.colOk = colOk;
        this.esSol = esSol;

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

    //Constructor utilizado por del Sudoku backtracking
    public DataHistory(int[,] sudoku, int fil, int col, int num, bool numeroBase, bool esSol, bool filOk = true, bool colOk = true, bool cuadroOk = true)
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

    //Constructor utilizado por del Laberinto backtracking
    public DataHistory(int[,] laberinto, int fil, int col, int k, bool esSol, bool esFactible, int dir)
    {
        this.board = laberinto;
        this.fil = fil;
        this.col = col;
        this.k = k;
        this.esSol = esSol;
        this.esFactible = esFactible;
        this.dir = dir;
    }

}
