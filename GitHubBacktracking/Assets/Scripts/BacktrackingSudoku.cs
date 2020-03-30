using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BacktrackingSudoku : MonoBehaviour
{
    public Text sudokuText;

    // Start is called before the first frame update
    void Start()
    {
        int[,] sudoku = inicializar();
        var result = resolverSudokuVA(sudoku, 0);
        sudoku = result.Item1;
        bool esSol = result.Item2;
        printSudoku(sudoku);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void printSudoku(int[,] sudoku)
    {
        sudokuText.text = "";
        for(int i = 0; i < 9; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < 9; j++)
            {
                if (j != 8)
                {
                    sudokuText.text += sudoku[i, j] + ", ";
                }
                else
                {
                    sudokuText.text += sudoku[i, j] + "";
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public bool esFactible(int[,] sudoku ,int f ,int c ,int num) {
        //bool filaOk = num not in sudoku[f,:]
        bool filaOk = true;
        for(int i = 0; i < 9; i++)
        {
            if (sudoku[f, i] == num)
            {
                filaOk = false;
            }
        }

        //bool colOk = num not in sudoku[:, c]
        bool colOk = true;
        for (int i = 0; i < 9; i++)
        {
            if (sudoku[i, c] == num)
            {
                colOk = false;
            }
        }

        int filaIni = 3 * (f / 3);
        int filaFin = filaIni + 3;
        int colIni = 3 * (c / 3);
        int colFin = colIni + 3;
        //bool cuadroOk = num not in sudoku[filaIni: filaFin, colIni: colFin];
        bool cuadroOk = true;
        for (int i = filaIni; i < filaFin; i++)
        {
            for (int j = colIni; j < colFin; j++)
            {
                if (sudoku[i, j] == num)
                {
                    cuadroOk = false;
                }
            }
        }

        return (filaOk && colOk && cuadroOk);
    }

    public Tuple<int[,], bool> resolverSudokuVA(int[,]sudoku, int casilla) {

        bool esSol = false;
        if (casilla >= sudoku.Length) {
            esSol = true;
        }
        else {
            esSol = false;
            int N = 9;
            int fila = casilla / N;
            int col = casilla % N;

            if (sudoku[fila, col] != 0) {
                var result = resolverSudokuVA(sudoku, casilla+1);
                sudoku = result.Item1;
                esSol = result.Item2;
            }
            else{
                int num = 1;
                while (!esSol && num <= N) {
                    if (esFactible(sudoku, fila, col, num))
                    {
                        sudoku[fila, col] = num;
                        var result = resolverSudokuVA(sudoku, casilla + 1);
                        sudoku = result.Item1;
                        esSol = result.Item2;
                        if (!esSol)
                        {
                            sudoku[fila, col] = 0;
                        }
                    }
                    num += 1;
                }
            }
        }
        
        return Tuple.Create(sudoku, esSol);
   }

    public int[,] inicializar() {
        int[,] instancia = new int[9, 9];
        instancia[0, 1] = 6;
        instancia[0, 3] = 1;
        instancia[0, 5] = 4;
        instancia[0, 7] = 5;
        instancia[1, 2] = 8;
        instancia[1, 3] = 3;
        instancia[1, 5] = 5;
        instancia[1, 6] = 6;
        instancia[2, 0] = 2;
        instancia[2, 8] = 1;
        instancia[3, 0] = 8;
        instancia[3, 3] = 4;
        instancia[3, 5] = 7;
        instancia[3, 8] = 6;
        instancia[4, 2] = 6;
        instancia[4, 6] = 3;
        instancia[5, 0] = 7;
        instancia[5, 3] = 9;
        instancia[5, 5] = 1;
        instancia[5, 8] = 4;
        instancia[6, 0] = 5;
        instancia[6, 8] = 2;
        instancia[7, 2] = 7;
        instancia[7, 3] = 2;
        instancia[7, 5] = 6;
        instancia[7, 6] = 9;
        instancia[8, 1] = 4;
        instancia[8, 3] = 5;
        instancia[8, 5] = 8;
        instancia[8, 7] = 7;
        return instancia;
      }
}
