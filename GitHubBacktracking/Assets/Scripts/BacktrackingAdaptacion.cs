using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;


public class BacktrackingAdaptacion : MonoBehaviour
{
    public int tamaño = GameState.gameState.tamaño;
    public Text vectorSolucion, textPlayButton;
    public Text tablero;
    public Text NReinasText;
    public Scrollbar speedNextMove;
    List<DataHistory> boardHistory;
    bool boardSolved = false;
    bool firstTime = true;
    bool play = true;
    bool nextStep = true;
    DataHistory data;
    int i = -1;

    public void printBoard(int[,] board)
    {
        tablero.text = "";
        vectorSolucion.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                tablero.text += board[i, j] + " ";
                if (board[i, j] == 1)
                {
                    vectorSolucion.text += j + " ";
                }
            }
            tablero.text += "\n";
        }
    }
    public bool EsFactible(int[,] board, int k, int n)
    {
        bool salir = true;
        int i = 0;
        while (k - i >= 0 && i < tamaño && salir == true)
        {
            if (board[k - i, n] == 1)
            {
                salir = false;
            }
            i += 1;
        }
        i = 0;
        while (k-i>=0 && salir==true && n - i >= 0)
        {
            if (board[k - i, n - i] == 1)
            {
                salir = false;
            }
            i += 1;
        }
        i = 0;
        while (k-i>=0 && salir==true && n + i < tamaño)
        {
            if (board[k - i, n + i] == 1)
            {
                salir = false;
            }
            i += 1;
        }
        return salir;
    }
    public Tuple<int[,],bool> NReinas(int[,] board, int k, bool esSol, int[,] boardVisualizer)
    {
        if (k >= tamaño)
        {
            esSol = true;
        }
        else
        {
            esSol = false;
            int n = 0;
            while (esSol == false && n < tamaño)
            {
                boardVisualizer[k, n] = 1;
                int[,] aux = new int[tamaño, tamaño];
                for(int i = 0; i < tamaño;i++)
                {
                    for (int j = 0; j < tamaño; j++)
                    {
                        aux[i, j] = boardVisualizer[i, j];
                    }
                }
                data = new DataHistory(aux, k, n);
                boardHistory.Add(data);
                if (EsFactible(board, k, n))
                {
                    board[k, n] = 1;
                    var result = NReinas(board, k + 1, esSol, boardVisualizer);
                    board = result.Item1;
                    esSol = result.Item2;
                    if (esSol == false)
                    {
                        board[k, n] = 0;
                    }
                }
                if (esSol == false)
                {
                    boardVisualizer[k, n] = 0;
                }
                n += 1;
            }
            
        }
        return Tuple.Create(board, esSol);
    }
    // Start is called before the first frame update
    void Start()
    {
        tamaño = GameState.gameState.tamaño;
        int[,] boardVisualizer = new int[tamaño, tamaño];
        printBoard(boardVisualizer);
    }

    // Update is called once per frame
    void Update()
    {
        if (boardSolved)
        {
            if (play)
            {
                if (nextStep && i < boardHistory.Count-1)
                {
                    nextStep = false;
                    i += 1;
                    StartCoroutine(waitForNextMove(boardHistory[i].board));
                }
            }
            else
            {
                if (nextStep)
                {
                    printBoard(boardHistory[i].board);
                }
            }
        }
    }
   
    public void playFunc()
    {
        if (firstTime)
        {
            textPlayButton.text = "Pause";
            firstTime = false;
            tamaño = GameState.gameState.tamaño;
            bool esSol = false;
            int[,] board = new int[tamaño, tamaño];
            int[,] boardVisualizer = new int[tamaño, tamaño];
            boardHistory = new List<DataHistory>();
            NReinas(board, 0, esSol, boardVisualizer);
            boardSolved = true;
        }
        else if (play)
        {
            textPlayButton.text = "Play";
            play = false;
        }
        else if(!play)
        {
            textPlayButton.text = "Pause";
            play = true;
        }
        
    }

    IEnumerator waitForNextMove(int[,] boardVisualizer)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        printBoard(boardVisualizer);
        nextStep = true;
    }

    public void nextStepFunc()
    {
        if (!play)
        {
            if (i < boardHistory.Count - 1)
            {
                nextStep = true;
                i += 1;
            }
        }
    }

    public void backStepFunc()
    {
        if (!play)
        {
            if (i > 0)
            {
                nextStep = true;
                i -= 1;
            }
        }
    }
}
