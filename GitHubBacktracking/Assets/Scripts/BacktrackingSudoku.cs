using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacktrackingSudoku : MonoBehaviour
{
    //Declaracion de variables públicas.
    public Text sudokuText;
    public TextMeshProUGUI esFactibleFunction, sudokuVAText, ifCasillaText, esSolTrueText, elseCasillaText, esSolFalseText, Ntext, filaText, colText, ifSudokuText, sudokuVARecursiveText, elseSudokuText, num1Text, whileNotEsSolText, ifEsFactibleText, sudokuNumText, sudokuVARecursive2Text, ifNotEsSolText, sudoku0Text, num1v2Text, returnSudokuText, esFactibleFunctionText, filaOKText, colOkText, filaIniText9x9, filaFinText9x9, colIniText9x9, colFinText9x9, filaIniText4x4, filaFinText4x4, colIniText4x4, colFinText4x4, cuadroOkText, returnEsFactibleText;
    public TextMeshProUGUI esSolActualText, filActualText, colActualText, numActualText, progresoText;
    public GameObject filCol9x9, filCol4x4;
    public Image playButton;
    public Scrollbar speedNextMove;
    public List<GameObject> objetoTablero;
    public Slider slider;

    //Declaracion de variables privadas.
    TextMeshProUGUI filaIniText, filaFinText, colIniText, colFinText;
    List<DataHistory> sudokuHistory;
    List<List<TextMeshProUGUI>> textos;
    List<List<string>> textosSinPintar;
    GameObject[,] piezasTablero;
    DataHistory data;
    SharedCode menu;
    int tamaño = GameState.gameState.tamaño;
    bool nextStep = true;
    bool nextText = false;
    bool firstTime = true;
    bool backStep = false;
    bool play = false;
    bool recuperarEstadoCorutina = false;
    int problem = 2;
    int i = -1;
    int fase = 0;
    int[,] sudokuInicial;
    int valorTamaño = 2;
    
    void Start()
    {
        //Inicializacion de variables dependiendo del tablero que se vaya a mostrar.
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
            sudoku = inicializar9x9();
            sudokuInicial = inicializar9x9();
            sudokuVisualizer = inicializar9x9();
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
        //Inicializamos variables.
        textos = new List<List<TextMeshProUGUI>>();
        textosSinPintar = new List<List<string>>();
        menu = new SharedCode();
        sudokuHistory = new List<DataHistory>();
        piezasTablero = new GameObject[tamaño, tamaño];
        inicializarTextos();
        //Resolvemos el problema y guardamos cada uno de los sudokus en una lista.
        resolverSudokuVA(sudoku, sudokuVisualizer, 0);

        //Guardamos la referencia visual de cada una de las piezas del sudoku en piezasTablero, para poder acceder a ellas.
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
        //Pintamos el sudoku inicial.
        printInitialSudoku(sudokuHistory[0], sudokuHistory[0].board, true);
    }
    
    void Update()
    {
        slider.value = (float)i / (float)(sudokuHistory.Count - 1);
        progresoText.text = "Paso " + (i + 1) + " / " + sudokuHistory.Count;
        if (play)
        {
            //Cuando una llamada recursiva se de.
            if (nextStep && i < sudokuHistory.Count - 1)
            {
                nextStep = false;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añadimos una nueva pila de ejecución para esta llamada recursiva.
                menu.pilaEjecucion.Add(tableroEjecucion);
                if (i != 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuInicial));
                }
            }
            else if (nextText)
            {
                nextText = false;
                //Actualizamos la fase, es decir avanzamos hacia el siguiente paso. 
                fase += 1;
                if (i != 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuInicial));
                }

            }
            else if (recuperarEstadoCorutina)
            {
                //Retomamos la corutina en la que estabamos antes de pausar la ejecucion.
                recuperarEstadoCorutina = false;
                if (i != 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(sudokuHistory[i], sudokuHistory.Count - 1 == i, sudokuInicial));
                }
            }
        }
    }

    //Este método pintará la fase actual de la ejecucion, además de elegir el camino en caso de encontrarnos en un if o else, en base a los valores guardados en nuestro sudokuHistory, el cual guarda todos los valores de cada una de las fases del sudoku.
    public void faseDePintado(DataHistory sudokuHistory, int fase, bool lastMove, int[,] sudokuAnterior=null) {

        //Actualizamos los textos mostrados por pantalla
        filActualText.text = "Fil actual: " + (sudokuHistory.fil + 1);
        colActualText.text = "Col actual: " + (sudokuHistory.col + 1);
        esSolActualText.text = "EsSol actual: "+ sudokuHistory.esSol;
        if (sudokuHistory.num == 0)
        {
            numActualText.text = "Num actual: null";
        }
        int camino = 0;
        
        switch (fase)
        {
            case 0:
                printInitialSudoku(sudokuHistory, sudokuAnterior);
                break;
            case 2:
                //Si es el ultimo  tablero, es decir la solucion, finalizará la ejecucion, sino seguira con ella, este se trata del primer if del codigo.
                if (lastMove)
                {
                    camino = 0;
                }
                else
                {
                    camino = 1;
                }
                break;
            case 3:
                //Continuacion del caso anterior
                if (lastMove)
                {
                    camino = 0;
                }
                else
                {
                    camino = 1;
                }
                break;
            case 8:
                //Segundo if de la ejecucion, aqui miramos si el numero del sudoku venia dado por el problema, en tal caso, no podemos modificarlo y por tanto se pasa a la llamada recursiva.
                if (sudokuHistory.numeroBase)
                {
                    camino = 0;
                }
                else
                {
                    printInitialSudoku(sudokuHistory, sudokuAnterior);
                    camino = 1;
                }
                break;
            case 10:
                printSudokuFirstStep(sudokuHistory);
                numActualText.text = "Num actual: "+ sudokuHistory.num;
                break;
            case 11:
                //Desactivamos el esFactible para que no sea visible si es un paso atras.
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                break;
            case 12:
                //Activamos el esFactible para que no sea visible si no es un paso atras.
                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                break;
            case 13:
                //Coloreamos la fila actual de nuestro sudoku.
                printSudokuFilOK(sudokuHistory);
                break;
            case 14:
                //Coloreamos la columna actual de nuestro sudoku.
                printSudokuColOK(sudokuHistory);
                break;
            case 15:
                printSudokuFirstStep(sudokuHistory);
                break;
            case 19:
                //Coloreamos el cuadrado de nuestro sudoku.
                printSudokuCuadroOK(sudokuHistory);
                break;
            case 20:
                printSudokuFirstStep(sudokuHistory);
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                break;
            case 21:
                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                //Tercer if de la ejecucion si es factible dicho movimiento, entonces pasaremos a la llamada recursiva, sino continuaremos con la ejecucion normal
                if (sudokuHistory.colOk && sudokuHistory.filOk && sudokuHistory.cuadroOk)
                {
                    camino = 0;
                }
                else {
                    camino = 1;
                }
                break;
        }
        
        menu.pila(fase, camino, i, backStep, textosSinPintar, sudokuHistory, problem);
    }

    //Este método pintará el siguiente estado del sudoku y esperara un numero de segundos.
    IEnumerator waitForNextText(DataHistory sudoku, bool lastMove, int[,] sudokuAnterior)
    {
        faseDePintado(sudoku, fase, lastMove, sudokuAnterior);
        yield return new WaitForSeconds(speedNextMove.value);
        var results = menu.waitForNextText(lastMove, fase, nextText, nextStep, sudoku, problem);
        nextStep = results.Item1;
        nextText = results.Item2;
        fase = results.Item3;
    }

    //Este metodo muestra el estado inicial del sudoku en cada ejecucion.
    public void printInitialSudoku(DataHistory sudokuActual, int[,] sudokuAnterior,bool firstTime=false)
    {
        sudokuText.text = "";
        for(int i = 0; i < tamaño; i++)
        {
            sudokuText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                string color;
                Color color2;
                if (firstTime)
                {
                    color = "white";
                    color2 = Color.black;
                }
                else
                {
                    color = "cyan";
                    color2 = Color.cyan;
                }
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = sudokuAnterior[i, j] + "";
                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.black;
                if (sudokuActual.fil == i && sudokuActual.col == j)
                {
                    //Pinta la parte visual
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = color2;
                    //Pinta la matriz
                    if (tamaño == 9)
                    {
                        if (j != 8)
                        {
                            sudokuText.text += "<color=" + color + ">" + sudokuAnterior[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=" + color + ">" + sudokuAnterior[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 3)
                        {
                            sudokuText.text += "<color=" + color + ">" + sudokuAnterior[i, j] + "</color>, ";
                        }
                        else
                        {
                            sudokuText.text += "<color=" + color + ">" + sudokuAnterior[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    //Pinta la parte visual
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    //Pinta la matriz
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

    //Pintamos el primer paso del sudoku en el que estamos es decir el numero con el que vamos a probar
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
                    //Pinta la parte visual
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color=Color.cyan;
                    //Pinta la matriz
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
                    //Pinta la parte visual
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    //Pinta la matriz
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

    //Pinta la fila actual en la que nos encontramos, bien de color verde si es factible poner el numero en dicha fila o bien de color rojo si no lo es.
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
                    //Pinta la parte visual
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            //Pinta la matriz
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
                        //Pinta la parte visual
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        //Pinta la matriz
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

    //Pinta la columna actual en la que nos encontramos, bien de color verde si es factible poner el numero en dicha columna o bien de color rojo si no lo es.
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
                    //Pinta la parte visual
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            //Pinta la matriz
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
                        //Pinta la parte visual
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        //Pinta la matriz
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

    //Pinta el cuadrado actual en el que nos encontramos, bien de color verde si es factible poner el numero en dicho cuadrado o bien de color rojo si no lo es.
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
                    //Pinta la parte visual
                    piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                            //Pinta la matriz
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
                            //Pinta la parte visual
                            piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                            //Pinta la matriz
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
                        //Pinta la parte visual
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                        //Pinta la matriz
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

    //Metodo esfactible para resolver el problema
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

    //Codigo resolverSudoku
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
            data = new DataHistory(aux, tamaño-1, tamaño-1, 0, false,esSol,true);
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
                data = new DataHistory(aux, fila, col, 0, numeroBase,esSol, true);
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

    //Inicializa y guarda referencia a todos los textos del codigo, tanto los pintados como los que no lo están.
    public void inicializarTextos()
    {
        List<TextMeshProUGUI> aux1 = new List<TextMeshProUGUI>();
        aux1.Add(sudokuVAText);
        aux1.Add(sudokuVAText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(ifCasillaText);
        aux1.Add(ifCasillaText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(esSolTrueText);
        aux1.Add(elseCasillaText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(returnSudokuText);
        aux1.Add(esSolFalseText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(Ntext);
        aux1.Add(Ntext);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(filaText);
        aux1.Add(filaText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(colText);
        aux1.Add(colText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(ifSudokuText);
        aux1.Add(ifSudokuText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(sudokuVARecursiveText);
        aux1.Add(elseSudokuText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(num1Text);
        aux1.Add(num1Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(whileNotEsSolText);
        aux1.Add(whileNotEsSolText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(ifEsFactibleText);
        aux1.Add(ifEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(esFactibleFunctionText);
        aux1.Add(esFactibleFunctionText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(filaOKText);
        aux1.Add(filaOKText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(colOkText);
        aux1.Add(colOkText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(filaIniText);
        aux1.Add(filaIniText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(filaFinText);
        aux1.Add(filaFinText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(colIniText);
        aux1.Add(colIniText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(colFinText);
        aux1.Add(colFinText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(cuadroOkText);
        aux1.Add(cuadroOkText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(returnEsFactibleText);
        aux1.Add(returnEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(sudokuNumText);
        aux1.Add(num1v2Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(sudokuVARecursive2Text);
        aux1.Add(sudokuVARecursive2Text);
        textos.Add(aux1);

        List<string> aux = new List<string>();
        aux.Add("def sudokuVA(sudoku, casilla):");
        aux.Add("def sudokuVA(sudoku, casilla):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if casilla >= np.size(sudoku):");
        aux.Add("if casilla >= np.size(sudoku):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("esSol = True");
        aux.Add("else:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("return sudoku, esSol");
        aux.Add("esSol = False");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("N = np.size(sudoku, 0)");
        aux.Add("N = np.size(sudoku, 0)");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("fila = casilla // N");
        aux.Add("fila = casilla // N");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("col = casilla % N");
        aux.Add("col = casilla % N");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if sudoku[fila, col] != 0:");
        aux.Add("if sudoku[fila, col] != 0:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("[sudoku, esSol] = sudokuVA(sudoku, casilla + 1)");
        aux.Add("else:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("num = 1");
        aux.Add("num = 1");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("while not esSol and num <= N:");
        aux.Add("while not esSol and num <= N:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if esFactible(sudoku, fila, col, num):");
        aux.Add("if esFactible(sudoku, fila, col, num):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("def EsFactible(tab, f, c):");
        aux.Add("def EsFactible(tab, f, c):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("filaOk = num not in sudoku[f, :]");
        aux.Add("filaOk = num not in sudoku[f, :]");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("colOk = num not in sudoku[:, c]");
        aux.Add("colOk = num not in sudoku[:, c]");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("filaIni = " + valorTamaño + " * (f // " + valorTamaño + ")");
        aux.Add("filaIni = " + valorTamaño + " * (f // " + valorTamaño + ")");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("filaFin = filaIni + " + valorTamaño + "");
        aux.Add("filaFin = filaIni + " + valorTamaño + "");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("colIni = " + valorTamaño + " * (c // " + valorTamaño + ")");
        aux.Add("colIni = " + valorTamaño + " * (c // " + valorTamaño + ")");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("colFin = colIni + " + valorTamaño);
        aux.Add("colFin = colIni + " + valorTamaño);
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("cuadroOk = num not in sudoku[filaIni:filaFin, colIni:colFin]");
        aux.Add("cuadroOk = num not in sudoku[filaIni:filaFin, colIni:colFin]");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("return filaOk and colOk and cuadroOk");
        aux.Add("return filaOk and colOk and cuadroOk");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("sudoku[fila, col] = num");
        aux.Add("num += 1");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("[sudoku, esSol] = sudokuVA(sudoku, casilla + 1)");
        aux.Add("[sudoku, esSol] = sudokuVA(sudoku, casilla + 1)");
        textosSinPintar.Add(aux);

        menu.addTextos(textos);
    }

    public int[,] inicializar9x9() {
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

    //Llama la funcion play del codigo comnpartido
    public void playFunc()
    {
        if (gameObject.active)
        {
            if (play)
            {
                StopAllCoroutines();
            }
            var results = menu.playFunc(play, backStep, firstTime, nextStep, recuperarEstadoCorutina, playButton, nextText);
            play = results.Item1;
            backStep = results.Item2;
            firstTime = results.Item3;
            recuperarEstadoCorutina = results.Item4;
            nextText = results.Item5;
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
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
            }
            else if (fase == 22)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuHistory[i - 1].board);
            }
            else if (fase == 8 && sudokuHistory[i].numeroBase)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
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
                    BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
                else
                {
                    faseDePintado(sudokuHistory[i], fase, sudokuHistory.Count - 1 == i, sudokuInicial);
                }
            }
        }
    }

    //Llama la funcion backStepFunc del codigo comnpartido
    public void backStepFunc()
    {
        if (gameObject.active && !play)
        {
            var results = menu.backStepFunc(backStep, fase, i);
            backStep = results.Item1;
            fase = results.Item2;
            i = results.Item3;
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
