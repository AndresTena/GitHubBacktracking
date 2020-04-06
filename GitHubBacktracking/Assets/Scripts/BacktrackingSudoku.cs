using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacktrackingSudoku : MonoBehaviour
{

    public Text sudokuText;
    public TextMeshProUGUI esFactibleFunction, sudokuVAText, ifCasillaText, esSolTrueText, elseCasillaText, esSolFalseText, Ntext, filaText, colText, ifSudokuText, sudokuVARecursiveText, elseSudokuText, num1Text, whileNotEsSolText, ifEsFactibleText, sudokuNumText, sudokuVARecursive2Text, ifNotEsSolText, sudoku0Text, num1v2Text, returnSudokuText, esFactibleFunctionText, filaOKText, colOkText, filaIniText9x9, filaFinText9x9, colIniText9x9, colFinText9x9, filaIniText4x4, filaFinText4x4, colIniText4x4, colFinText4x4, cuadroOkText, returnEsFactibleText;
    public TextMeshProUGUI esSolActualText, filActualText, colActualText, numActualText;
    public GameObject filCol9x9, filCol4x4;
    public Image playButton;
    public Scrollbar speedNextMove;
    public List<GameObject> objetoTablero;

    TextMeshProUGUI filaIniText, filaFinText, colIniText, colFinText;
    List<DataHistory> sudokuHistory;
    List<TextMeshProUGUI> textos;
    List<TextColor> pilaEjecucion;
    GameObject[,] piezasTablero;
    DataHistory data;
    SharedCode menu;
    int tamaño = GameState.gameState.tamaño;
    bool nextStep = true;
    bool firstTime = true;
    bool backStep = false;
    bool play = false;
    bool recuperarEstadoCorutina = false;
    int i = -1;
    int fase = 0;
    int[,] sudokuInicial;
    int valorTamaño = 2;


    // Start is called before the first frame update
    void Start()
    {
        tamaño = GameState.gameState.tamaño;
        int[,] sudoku, sudokuVisualizer;
        if (tamaño == 9)
        {
            filCol4x4.gameObject.SetActive(false);
            filCol9x9.gameObject.SetActive(true);
            filaIniText = filaIniText9x9;
            filaFinText = filaFinText9x9;
            colIniText = colIniText9x9;
            colFinText = colFinText9x9;
            valorTamaño = 3;
            sudoku = inicializar();
            sudokuInicial = inicializar();
            sudokuVisualizer = inicializar();
        }
        else
        {
            filCol9x9.gameObject.SetActive(false);
            filCol4x4.gameObject.SetActive(true);
            filaIniText = filaIniText4x4;
            filaFinText = filaFinText4x4;
            colIniText = colIniText4x4;
            colFinText = colFinText4x4;
            valorTamaño = 2;
            sudoku = inicializar4x4();
            sudokuInicial = inicializar4x4();
            sudokuVisualizer = inicializar4x4();
        }
        pilaEjecucion = new List<TextColor>();
        textos = new List<TextMeshProUGUI>();
        menu = new SharedCode();
        sudokuHistory = new List<DataHistory>();
        pilaEjecucion = new List<TextColor>();
        inicializarTextos();
        var result = resolverSudokuVA(sudoku, sudokuVisualizer, 0);
        sudoku = result.Item1;
        bool esSol = result.Item2;

        piezasTablero = new GameObject[tamaño, tamaño];
        int indiceX = 0;
        int indiceY = 0;
        int i = 0;
        for (i = 0; i < tamaño * tamaño; i++)
        {
            if (indiceY >= tamaño)
            {
                indiceX += 1;
                indiceY = 0;
            }
            if (tamaño == 4) {
                objetoTablero[0].gameObject.SetActive(true);
                objetoTablero[1].gameObject.SetActive(false);
                piezasTablero[indiceX, indiceY] = objetoTablero[0].transform.GetChild(i).gameObject;
            }
            else
            {
                objetoTablero[1].gameObject.SetActive(true);
                objetoTablero[0].gameObject.SetActive(false);
                piezasTablero[indiceX, indiceY] = objetoTablero[1].transform.GetChild(i).gameObject;
            }
            indiceY += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (play) {
            if (nextStep && i < sudokuHistory.Count - 1)
            {
                nextStep = false;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                if (i != 0)
                {
                    estadoCorutina(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
                }
                else {
                    estadoCorutina(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
            }
            else if (recuperarEstadoCorutina)
            {
                recuperarEstadoCorutina = false;
                if (i != 0)
                {
                    estadoCorutina(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
                }
                else
                {
                    estadoCorutina(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
            }
        }
    }

    public void estadoCorutina(DataHistory boardHistory, int fase, bool lastMove, int[,] sudokuAnterior)
    {
        //En que momento del pintado nos encontramos?

        switch (fase)
        {
            case 0:
                StartCoroutine(waitForDefSudoku(boardHistory, lastMove, sudokuAnterior));
                break;
            case 1:
                StartCoroutine(waitForIfCasilla(boardHistory, lastMove));
                break;
            case 2:
                StartCoroutine(waitForElseCasilla(boardHistory, lastMove));
                break;
            case 3:
                StartCoroutine(waitForEsSolFalse(boardHistory, lastMove));
                break;
            case 4:
                StartCoroutine(waitForN(boardHistory, lastMove));
                break;
            case 5:
                StartCoroutine(waitForFilaCasilla(boardHistory, lastMove));
                break;
            case 6:
                StartCoroutine(waitForColCasilla(boardHistory, lastMove));
                break;
            case 7:
                StartCoroutine(waitForIfSudoku(boardHistory, lastMove));
                break;
            case 8:
                StartCoroutine(waitForElseSudoku(boardHistory, lastMove));
                break;
            case 9:
                StartCoroutine(waitForNum1(boardHistory, lastMove));
                break;
            case 10:
                StartCoroutine(waitForWhileNotEsSol(boardHistory, lastMove));
                break;
            case 11:
                StartCoroutine(waitForIfEsFactible(boardHistory, lastMove));
                break;
            case 12:
                StartCoroutine(waitForEsFactibleFunction(boardHistory, lastMove));
                break;
            case 13:
                StartCoroutine(waitForFilaOK(boardHistory, lastMove));
                break;
            case 14:
                StartCoroutine(waitForColOK(boardHistory, lastMove));
                break;
            case 15:
                StartCoroutine(waitForFilIni(boardHistory, lastMove));
                break;
            case 16:
                StartCoroutine(waitForFilFin(boardHistory, lastMove));
                break;
            case 17:
                StartCoroutine(waitForColIni(boardHistory, lastMove));
                break;
            case 18:
                StartCoroutine(waitForColFin(boardHistory, lastMove));
                break;
            case 19:
                StartCoroutine(waitForCuadroOk(boardHistory, lastMove));
                break;
            case 20:
                StartCoroutine(waitForReturnEsFactible(boardHistory, lastMove));
                break;
            case 21:
                StartCoroutine(waitForSudokuNum(boardHistory, lastMove));
                break;
            case 22:
                StartCoroutine(waitForSudokuVARecursive2(boardHistory, lastMove));
                break;
        }
    }

    public void faseDePintado(DataHistory sudokuHistory, int fase, bool lastMove, int[,] sudokuAnterior=null) {

        filActualText.text = "Fil actual: " + (sudokuHistory.fil + 1);
        colActualText.text = "Col actual: " + (sudokuHistory.col + 1);
        esSolActualText.text = "EsSol actual: "+ sudokuHistory.esSol;
        if (sudokuHistory.num == 0)
        {
            numActualText.text = "Num actual: null";
        }
        switch (fase)
        {
            case 0:
                //Primer paso: pintado del defSudokuVA y del tablero;
                printInitialSudoku(sudokuHistory, sudokuAnterior);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                sudokuVAText.text = "def sudokuVA(sudoku, casilla):";
                sudokuVAText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(sudokuVAText);
                }
                break;

            case 1:
                //Pintamos el ifCasilla;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                ifCasillaText.text = "if casilla >= np.size(sudoku):";
                ifCasillaText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifCasillaText);
                }

                break;
            case 2:
                //Pinatmos el esSol True si es el ultimo movimiento o el else si no lo es;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (lastMove)
                {
                    esSolTrueText.text = "esSol = True";
                    esSolTrueText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(esSolTrueText);
                    }
                }
                else
                {
                    elseCasillaText.text = "else:";
                    elseCasillaText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(elseCasillaText);
                    }
                }
                break;
            case 3:
                //Pinatmos el esSol False;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (lastMove)
                {
                    returnSudokuText.text = "return sudoku, esSol";
                    returnSudokuText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(returnSudokuText);
                    }
                }
                else
                {
                    esSolFalseText.text = "esSol = False";
                    esSolFalseText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(esSolFalseText);
                    }
                }
                break;
            case 4:
                //Pinatmos el N = np.size y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                Ntext.text = "N = np.size(sudoku, 0)";
                Ntext.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(Ntext);
                }
                break;
            case 5:
                //Pinatmos el fila = casilla // N y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                filaText.text = "fila = casilla // N";
                filaText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(filaText);
                }
                break;
            case 6:
                //Pinatmos el col = casilla % N y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                colText.text = "col = casilla % N";
                colText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(colText);
                }
                break;
            case 7:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                ifSudokuText.text = "if sudoku[fila, col] != 0:";
                ifSudokuText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifSudokuText);
                }
                break;
            case 8:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (sudokuHistory.numeroBase)
                {
                    sudokuVARecursiveText.text = "[sudoku, esSol] = sudokuVA(sudoku, casilla + 1)";
                    sudokuVARecursiveText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(sudokuVARecursiveText);
                    }
                }
                else
                {
                    elseSudokuText.text = "else:";
                    elseSudokuText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(elseSudokuText);
                    }
                }
                break;
            case 9:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                num1Text.text = "num = 1";
                num1Text.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(num1Text);
                }
                break;
            case 10:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printSudokuFirstStep(sudokuHistory);
                numActualText.text = "Num actual: "+ sudokuHistory.num;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                whileNotEsSolText.text = "while not esSol and num <= N:";
                whileNotEsSolText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(whileNotEsSolText);
                }
                break;
            case 11:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                ifEsFactibleText.text = "if esFactible(sudoku, fila, col, num):";
                ifEsFactibleText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifEsFactibleText);
                }
                break;
            case 12:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if(!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                esFactibleFunctionText.text = "def EsFactible(tab, f, c):";
                esFactibleFunctionText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(esFactibleFunctionText);
                }
                break;
            case 13:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printSudokuFilOK(sudokuHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                filaOKText.text = "filaOk = num not in sudoku[f, :]";
                if (sudokuHistory.filOk)
                {
                    filaOKText.color = Color.green;
                }
                else
                {
                    filaOKText.color = Color.red;
                }
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(filaOKText);
                }
                break;
            case 14:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printSudokuColOK(sudokuHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                colOkText.text = "colOk = num not in sudoku[:, c]";
                if (sudokuHistory.colOk)
                {
                    colOkText.color = Color.green;
                }
                else
                {
                    colOkText.color = Color.red;
                }
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(colOkText);
                }
                break;
            case 15:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printSudokuFirstStep(sudokuHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                filaIniText.text = "filaIni = "+valorTamaño+" * (f // "+ valorTamaño+")";
                filaIniText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(filaIniText);
                }
                break;
            case 16:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                filaFinText.text = "filaFin = filaIni + " + valorTamaño + "";
                filaFinText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(filaFinText);
                }
                break;
            case 17:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                colIniText.text = "colIni = " + valorTamaño + " * (c // " + valorTamaño + ")";
                colIniText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(colIniText);
                }
                break;
            case 18:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                colFinText.text = "colFin = colIni + " + valorTamaño;
                colFinText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(colFinText);
                }
                break;
            case 19:
                printSudokuCuadroOK(sudokuHistory);
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                cuadroOkText.text = "cuadroOk = num not in sudoku[filaIni:filaFin, colIni:colFin]";
                if (sudokuHistory.cuadroOk)
                {
                    cuadroOkText.color = Color.green;
                }
                else
                {
                    cuadroOkText.color = Color.red;
                }
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(cuadroOkText);
                }
                break;
            case 20:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printSudokuFirstStep(sudokuHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                returnEsFactibleText.text = "return filaOk and colOk and cuadroOk";
                returnEsFactibleText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(returnEsFactibleText);
                }
                break;
            case 21:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                if (sudokuHistory.colOk && sudokuHistory.filOk && sudokuHistory.cuadroOk)
                {
                    sudokuNumText.text = "sudoku[fila, col] = num";
                    sudokuNumText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(sudokuNumText);
                    }
                }
                else {
                    num1v2Text.text = "num += 1";
                    num1v2Text.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(num1v2Text);
                    }
                }
                break;
            case 22:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                sudokuVARecursive2Text.text = "[sudoku, esSol] = sudokuVA(sudoku, casilla + 1)";
                sudokuVARecursive2Text.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(sudokuVARecursive2Text);
                }
                break;
        }

    }

    IEnumerator waitForDefSudoku(DataHistory sudoku,bool lastMove, int[,] sudokuAnterior)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 0;
        faseDePintado(sudoku, fase, lastMove, sudokuAnterior);
        StartCoroutine(waitForIfCasilla(sudoku,lastMove));
    }

    IEnumerator waitForIfCasilla(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 1;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForElseCasilla(sudoku,lastMove));
    }

    IEnumerator waitForElseCasilla(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 2;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForEsSolFalse(sudoku, lastMove));
    }

    IEnumerator waitForEsSolFalse(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 3;
        faseDePintado(sudoku, fase, lastMove);
        if (lastMove)
        {
            //Ejecucion terminada;
        }
        else
        {
            StartCoroutine(waitForN(sudoku, lastMove));
        }
    }

    IEnumerator waitForN(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 4;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForFilaCasilla(sudoku, lastMove));
    }

    IEnumerator waitForFilaCasilla(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 5;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForColCasilla(sudoku, lastMove));
    }

    IEnumerator waitForColCasilla(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 6;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForIfSudoku(sudoku, lastMove));
    }

    IEnumerator waitForIfSudoku(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 7;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForElseSudoku(sudoku, lastMove));
    }

    IEnumerator waitForElseSudoku(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 8;
        faseDePintado(sudoku, fase, lastMove);
        if (sudoku.numeroBase)
        {
            fase = 0;
            nextStep = true;
        }
        else
        {
            StartCoroutine(waitForNum1(sudoku, lastMove));
        }
    }

    IEnumerator waitForNum1(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 9;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForWhileNotEsSol(sudoku, lastMove));
    }

    IEnumerator waitForWhileNotEsSol(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 10;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForIfEsFactible(sudoku, lastMove));
    }

    IEnumerator waitForIfEsFactible(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 11;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForEsFactibleFunction(sudoku, lastMove));
    }

    IEnumerator waitForEsFactibleFunction(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 12;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForFilaOK(sudoku, lastMove));
    }

    IEnumerator waitForFilaOK(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 13;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForColOK(sudoku, lastMove));
    }

    IEnumerator waitForColOK(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 14;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForFilIni(sudoku, lastMove));
    }

    IEnumerator waitForFilIni(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 15;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForFilFin(sudoku, lastMove));
    }

    IEnumerator waitForFilFin(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 16;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForColIni(sudoku, lastMove));
    }

    IEnumerator waitForColIni(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 17;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForColFin(sudoku, lastMove));
    }

    IEnumerator waitForColFin(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 18;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForCuadroOk(sudoku, lastMove));
    }

    IEnumerator waitForCuadroOk(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 19;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForReturnEsFactible(sudoku, lastMove));
    }

    IEnumerator waitForReturnEsFactible(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 20;
        faseDePintado(sudoku, fase, lastMove);
        StartCoroutine(waitForSudokuNum(sudoku, lastMove));
    }
    
    IEnumerator waitForSudokuNum(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 21;
        faseDePintado(sudoku, fase, lastMove);
        if (sudoku.colOk && sudoku.filOk && sudoku.cuadroOk)
        {
            StartCoroutine(waitForSudokuVARecursive2(sudoku, lastMove));
        }
        else
        {
            fase = 10;
            nextStep = true;
        }
    }

    IEnumerator waitForSudokuVARecursive2(DataHistory sudoku, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 22;
        faseDePintado(sudoku, fase, lastMove);
        fase = 0;
        nextStep = true;
    }

    public void inicializarTextos(){
        textos.Add(sudokuVAText);
        textos.Add(ifCasillaText);
        textos.Add(esSolTrueText);
        textos.Add(elseCasillaText);
        textos.Add(esSolFalseText);
        textos.Add(Ntext);
        textos.Add(filaText);
        textos.Add(colText);
        textos.Add(ifSudokuText);
        textos.Add(sudokuVARecursiveText);
        textos.Add(elseSudokuText);
        textos.Add(num1Text);
        textos.Add(whileNotEsSolText);
        textos.Add(ifEsFactibleText);
        textos.Add(sudokuNumText);
        textos.Add(sudokuVARecursive2Text);
        textos.Add(ifNotEsSolText);
        textos.Add(sudoku0Text);
        textos.Add(num1v2Text);
        textos.Add(returnSudokuText);
        textos.Add(esFactibleFunctionText);
        textos.Add(filaOKText);
        textos.Add(colOkText);
        textos.Add(filaIniText);
        textos.Add(filaFinText);
        textos.Add(colIniText);
        textos.Add(colFinText);
        textos.Add(cuadroOkText);
        textos.Add(returnEsFactibleText);
        menu.addTextos(textos);
    }

    public void printInitialSudoku(DataHistory sudokuActual, int[,] sudokuAnterior)
    {
        sudokuText.text = "";
        for(int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuAnterior[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuActual.fil == i && sudokuActual.col == j)
                {
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuAnterior[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuAnterior[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuAnterior[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuAnterior[i, j] + "</color>";
                        }

                    }
                }
                else
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += sudokuAnterior[i, j] + ", ";
                        }
                        else
                        {
                            sudokuText.text += sudokuAnterior[i, j] + "";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += sudokuAnterior[i, j] + ", ";
                        }
                        else
                        {
                            sudokuText.text += sudokuAnterior[i, j] + "";
                        }
                    }
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public void printSudokuFirstStep(DataHistory sudokuHistory)
    {
        sudokuText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuHistory.board[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuHistory.fil == i && sudokuHistory.col == j)
                {
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color=Color.cyan;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += sudokuHistory.board[i, j] + ", ";
                        }
                        else
                        {
                            sudokuText.text += sudokuHistory.board[i, j] + "";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += sudokuHistory.board[i, j] + ", ";
                        }
                        else
                        {
                            sudokuText.text += sudokuHistory.board[i, j] + "";
                        }
                    }
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public void printSudokuFilOK(DataHistory sudokuHistory)
    {
        sudokuText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuHistory.board[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuHistory.fil == i && sudokuHistory.col == j)
                {
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    if (i == sudokuHistory.fil)
                    {
                        if (sudokuHistory.filOk)
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                            else
                            {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                        }
                        else
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";

                                }
                            }
                            else {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";

                                }
                            }
                        }
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        if (tamaño == 9)
                        {
                            if (j != 8)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }
                        else
                        {
                            if (j != 3)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }

                    }
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public void printSudokuColOK(DataHistory sudokuHistory)
    {
        sudokuText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuHistory.board[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuHistory.fil == i && sudokuHistory.col == j)
                {
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    if (j == sudokuHistory.col)
                    {
                        if (sudokuHistory.colOk)
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                            else
                            {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                        }
                        else
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";

                                }
                            }
                            else
                            {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                        }
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        if (tamaño == 9)
                        {
                            if (j != 8)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }
                        else
                        {
                            if (j != 3)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }
                    }
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public void printSudokuCuadroOK(DataHistory sudokuHistory)
    {
        int filaIni = 0;
        int filaFin = 0;
        int colIni = 0;
        int colFin = 0;

        if (tamaño == 9)
        {
            filaIni = 3 * (sudokuHistory.fil / 3);
            filaFin = filaIni + 3;
            colIni = 3 * (sudokuHistory.col / 3);
            colFin = colIni + 3;
        }
        else if (tamaño == 4)
        {
            filaIni = 2 * (sudokuHistory.fil / 2);
            filaFin = filaIni + 2;
            colIni = 2 * (sudokuHistory.col / 2);
            colFin = colIni + 2;
        }

        sudokuText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuHistory.board[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuHistory.fil == i && sudokuHistory.col == j)
                {
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=cyan>" + sudokuHistory.board[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    if (i < filaFin && i >= filaIni && j < colFin && j >= colIni)
                    {
                        if (sudokuHistory.cuadroOk)
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                            else
                            {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=green>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                        }
                        else
                        {
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            if (tamaño == 9)
                            {
                                if (j != 8)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";

                                }
                            }
                            else
                            {
                                if (j != 3)
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>, ";
                                }
                                else
                                {
                                    sudokuText.text += "<color=red>" + sudokuHistory.board[i, j] + "</color>";
                                }
                            }
                        }
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        if (tamaño == 9)
                        {
                            if (j != 8)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }
                        else
                        {
                            if (j != 3)
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + ", ";
                            }
                            else
                            {
                                sudokuText.text += sudokuHistory.board[i, j] + "";
                            }
                        }
                    }
                }
            }
            sudokuText.text += "]\n";
        }
    }

    public Tuple<bool, bool, bool> esFactible(int[,] sudoku ,int f ,int c ,int num) {
        //bool filaOk = num not in sudoku[f,:]
        bool filaOk = true;
        for(int i = 0; i < tamaño; i++)
        {
            if (sudoku[f, i] == num)
            {
                filaOk = false;
            }
        }

        //bool colOk = num not in sudoku[:, c]
        bool colOk = true;
        for (int i = 0; i < tamaño; i++)
        {
            if (sudoku[i, c] == num)
            {
                colOk = false;
            }
        }

        bool cuadroOk = true;
        if (tamaño == 9)
        {
            int filaIni = 3 * (f / 3);
            int filaFin = filaIni + 3;
            int colIni = 3 * (c / 3);
            int colFin = colIni + 3;
            //bool cuadroOk = num not in sudoku[filaIni: filaFin, colIni: colFin];
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
        }
        else {
            int filaIni = 2 * (f / 2);
            int filaFin = filaIni + 2;
            int colIni = 2 * (c / 2);
            int colFin = colIni + 2;
            //bool cuadroOk = num not in sudoku[filaIni: filaFin, colIni: colFin];
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
        }

        return Tuple.Create(filaOk, colOk, cuadroOk);
    }

    public Tuple<int[,], bool> resolverSudokuVA(int[,]sudoku, int[,]sudokuVisualizer, int casilla) {

        bool esSol = false;
        if (casilla >= sudoku.Length) {
            esSol = true;
            int[,] aux = new int[tamaño, tamaño];
            for (int i = 0; i < tamaño; i++)
            {
                for (int j = 0; j < tamaño; j++)
                {
                    aux[i, j] = sudokuVisualizer[i, j];
                }
            }
            data = new DataHistory(aux, tamaño-1, tamaño-1, 0, false,esSol);
            sudokuHistory.Add(data);
        }
        else {
            esSol = false;
            int N = tamaño;
            int fila = casilla / N;
            int col = casilla % N;
            bool numeroBase = false;

            if (sudoku[fila, col] != 0) {
                int[,] aux = new int[tamaño, tamaño];
                for (int i = 0; i < tamaño; i++)
                {
                    for (int j = 0; j < tamaño; j++)
                    {
                        aux[i, j] = sudokuVisualizer[i, j];
                    }
                }
                numeroBase = true;
                data = new DataHistory(aux, fila, col, 0, numeroBase,esSol);
                sudokuHistory.Add(data);
                var result = resolverSudokuVA(sudoku, sudokuVisualizer, casilla+1);
                sudoku = result.Item1;
                esSol = result.Item2;
            }
            else{
                int num = 1;
                while (!esSol && num <= N)
                {
                    sudokuVisualizer[fila, col] = num;
                    int[,] aux = new int[tamaño, tamaño];
                    for (int i = 0; i < tamaño; i++)
                    {
                        for (int j = 0; j < tamaño; j++)
                        {
                            aux[i, j] = sudokuVisualizer[i, j];
                        }
                    }
                    var results = esFactible(sudoku, fila, col, num);
                    numeroBase = false;
                    data = new DataHistory(aux, fila, col, num, numeroBase, esSol,results.Item1, results.Item2, results.Item3);
                    sudokuHistory.Add(data);
                    if (results.Item1 && results.Item2 && results.Item3)
                    {
                        sudoku[fila, col] = num;
                        var result = resolverSudokuVA(sudoku, sudokuVisualizer, casilla + 1);
                        sudoku = result.Item1;
                        esSol = result.Item2;
                        if (!esSol)
                        {
                            sudoku[fila, col] = 0;
                        }
                    }
                    if (!esSol)
                    {
                        sudokuVisualizer[fila, col] = 0;
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

    public int[,] inicializar4x4()
    {
        int[,] instancia = new int[4, 4];
        instancia[0, 0] = 4;
        instancia[0, 3] = 1;
        instancia[1, 1] = 1;
        instancia[1, 2] = 3;
        instancia[2, 1] = 4;
        instancia[2, 2] = 1;
        instancia[3, 0] = 1;
        instancia[3, 3] = 3;
        return instancia;
    }

    public void playFunc()
    {
        if (play && gameObject.active)
        {
            playButton.sprite = Resources.Load<Sprite>("play");
            play = false;
            StopAllCoroutines();
        }
        else if (!play && this.gameObject.activeSelf)
        {
            playButton.sprite = Resources.Load<Sprite>("pause");
            play = true;
            backStep = false;
            firstTime = false;
            if (!nextStep)
            {
                recuperarEstadoCorutina = true;
            }
        }

    }

    public void nextStepFunc()
    {
        if (!play && gameObject.active && !firstTime)
        {
            backStep = false;
            if (fase == 21 && !(sudokuHistory[i].colOk && sudokuHistory[i].filOk && sudokuHistory[i].cuadroOk))
            {
                fase = 10;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
            }
            else if (fase == 22)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
            }
            else if (fase == 8 && sudokuHistory[i].numeroBase)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
            }
            else if (fase == 3 && sudokuHistory.Count - 1 == i)
            {
                //Ejecucion terminada;
            }else
            {
                fase += 1;
                if (i != 0)
                {
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
                }
                else if (i < 0) {
                    i += 1;
                    List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                    List<int> fasesAsociadas = new List<int>();
                    TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
                else
                {
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
            }
        }
    }

    public void backStepFunc()
    {
        if (!play && gameObject.active)
        {
            if (i >= 0)
            {
                backStep = true;
                if (pilaEjecucion[i].pilaPintado.Count > 1)
                {
                    //Eliminamos el ultimo valor del ultimo objeto.
                    int contador = pilaEjecucion[i].pilaPintado.Count;
                    TextMeshProUGUI ultimoObjetoText = new TextMeshProUGUI();
                    foreach (TextMeshProUGUI t in pilaEjecucion[i].pilaPintado)
                    {
                        ultimoObjetoText = t;
                    }
                    ultimoObjetoText.color = Color.white;
                    pilaEjecucion[i].pilaPintado.Remove(ultimoObjetoText);

                    int ultimoObjetoInt = 0;
                    foreach (int t in pilaEjecucion[i].fases)
                    {
                        ultimoObjetoInt = t;
                    }
                    pilaEjecucion[i].fases.Remove(ultimoObjetoInt);

                    int lastObject = 0;
                    foreach (int t in pilaEjecucion[i].fases)
                    {
                        lastObject = t;
                    }
                    fase = lastObject;

                    /*if (contador >= 2)
                    {
                        //Coloreamos el penultimo objeto.
                        foreach (TextMeshProUGUI t in pilaEjecucion[i].pilaPintado)
                        {
                            ultimoObjetoText = t;
                        }
                        ultimoObjetoText.color = Color.cyan;
                    }
                    else if (i > 0)
                    {
                        //Coloreamos el ultimo objeto del tablero anterior.
                        foreach (TextMeshProUGUI t in pilaEjecucion[i - 1].pilaPintado)
                        {
                            ultimoObjetoText = t;
                        }
                        ultimoObjetoText.color = Color.cyan;
                    }
                    */
                }
                else
                {
                    if (i > 0)
                    {
                        TextMeshProUGUI ultimoObjetoText = new TextMeshProUGUI();
                        foreach (TextMeshProUGUI t in pilaEjecucion[i].pilaPintado)
                        {
                            ultimoObjetoText = t;
                        }
                        ultimoObjetoText.color = Color.white;
                        pilaEjecucion[i].pilaPintado.Remove(ultimoObjetoText);

                        int ultimoObjetoInt = 0;
                        foreach (int t in pilaEjecucion[i].fases)
                        {
                            ultimoObjetoInt = t;
                        }
                        pilaEjecucion[i].fases.Remove(ultimoObjetoInt);

                        i -= 1;

                        int lastObject = 0;
                        foreach (int t in pilaEjecucion[i].fases)
                        {
                            lastObject = t;
                        }
                        fase = lastObject;

                        //No entiendo porque no me deja borrar objetos.
                        /*List<TextMeshProUGUI> aux = new List<TextMeshProUGUI>();
                        List<int> fasesAsociadas = new List<int>();
                        TextColor lastObject = new TextColor(fasesAsociadas, aux);
                        foreach (TextColor t in pilaEjecucion)
                        {
                            lastObject = t;
                        }
                        pilaEjecucion.Remove(lastObject);*/
                    }
                }
                if (i != 0)
                {
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
                }
                else
                {
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }

            }
        }
    }

}
