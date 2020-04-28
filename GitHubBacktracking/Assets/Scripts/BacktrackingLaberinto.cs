using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacktrackingLaberinto : MonoBehaviour
{
    //Declaracion de variables publicas.
    public Text laberintoText;
    public TextMeshProUGUI esFactibleFunction,laberintoVAText, ifFText, esSolTrueText, elseFText, esSolFalseText, movText, returnLaberintoText, i0Text, whileNotEsSolText, ifEsFactibleText, esFactibleFunctionText,returnEsFactibleText,i1Text,laberintoKText,laberintoVARexursiveText;
    public TextMeshProUGUI kActualText, colActualText, filActualText, esSolActualText, progresoText;
    public Image playButton;
    public Scrollbar speedNextMove;
    public GameObject objetoTablero;
    public Slider slider;

    //Declaracion de variables privadas
    List<DataHistory> laberintoHistory;
    List<List<TextMeshProUGUI>> textos;
    List<List<string>> textosSinPintar;
    GameObject[,] piezasTablero;
    DataHistory data, inicial, laberintoBase;
    SharedCode menu;
    int tamaño;
    bool nextStep = true;
    bool nextText = false;
    bool backStep = false;
    bool firstTime = true;
    bool play = false;
    bool recuperarEstadoCorutina = false;
    int problem = 3;
    int i = -1;
    int fase = 0;
    int Xini = 1;
    int Yini = 1;

    void Start() {
        //Actualizamos el texto compartido
        kActualText.text = "K actual:";
        //Inicializacion de variables
        tamaño = 12;
        laberintoHistory = new List<DataHistory>();
        textos = new List<List<TextMeshProUGUI>>();
        textosSinPintar = new List<List<string>>();
        menu = new SharedCode();
        //Inicializacion de laberintos y textos
        int[,] lab = inicializarLaberinto();
        int[,] labVisualizer = inicializarLaberinto();
        inicializarTextos();
        int paso = 1;
        lab[Xini, Yini] = paso;
        labVisualizer[Xini, Yini] = paso;
        int[,] aux = new int[tamaño, tamaño];
        for (int u = 0; u < tamaño; u++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                aux[u, j] = lab[u, j];
            }
        }
        //Laberinto inicial.
        laberintoBase = new DataHistory(aux, Xini, Yini, 1, false, true);
        //Solucionamos el problema
        resolverLaberintoVA(lab, labVisualizer, Xini, Yini, paso + 1);

        piezasTablero = new GameObject[tamaño, tamaño];
        //Guardamos la referencia visual de cada una de las piezas del laberinto en piezasTablero, para poder acceder a ellas.
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
            piezasTablero[indiceX, indiceY] = objetoTablero.transform.GetChild(i).gameObject;
            indiceY += 1;
        }
        pintarEstadoTablero(laberintoHistory[0].board);
    }

    void Update()
    {
        slider.value = (float)i / (float)(laberintoHistory.Count - 1);
        progresoText.text = "Paso " + (i + 1) + " / " + laberintoHistory.Count;
        if (play)
        {
            //Cuando una llamada recursiva se de.
            if (nextStep && i < laberintoHistory.Count - 1)
            {
                nextStep = false;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añadimos una nueva pila de ejecución para esta llamada recursiva.
                menu.pilaEjecucion.Add(tableroEjecucion);
                if (i == 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoBase));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]));
                }
            }
            else if (nextText)
            {
                nextText = false;
                //Actualizamos la fase, es decir avanzamos hacia el siguiente paso. 
                fase += 1;
                if (i == 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoBase));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]));
                }

            }
            else if (recuperarEstadoCorutina)
            {
                //Retomamos la corutina en la que estabamos antes de pausar la ejecucion.
                recuperarEstadoCorutina = false;
                if (i == 0)
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoBase));
                }
                else
                {
                    //Empezamos una nueva corutina.
                    StartCoroutine(waitForNextText(laberintoHistory[i], laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]));
                }
            }
        }

    }

    //Este método pintará la fase actual de la ejecucion, además de elegir el camino en caso de encontrarnos en un if o else, en base a los valores guardados en nuestro laberintoHistory, el cual guarda todos los valores de cada una de las fases del laberinto.
    public void faseDePintado(DataHistory laberintoHistory, int fase, bool lastMove, DataHistory laberintoAnterior = null)
    {
        //Actualizamos los textos mostrados por pantalla
        esSolActualText.text = "EsSol actual: " + laberintoHistory.esSol;
        kActualText.text = "K actual: " + laberintoHistory.k;
        int camino = 0;
        switch (fase)
        {
            case 0:

                inicial = laberintoAnterior;
                printLaberintoFirstStep(inicial, inicial);
                //Actualizamos los textos de la fila y la columna actual
                filActualText.text = "Fil actual: " + (inicial.fil + 1);
                colActualText.text = "Col actual: " + (inicial.col + 1);
                camino = 0;
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
                    camino = 1;
                }
                else
                {
                    camino = 0;
                }

                break;
            case 4:

                if (backStep)
                {
                    inicial = laberintoAnterior;
                }
                printLaberintoFirstStep(inicial, inicial);
                //Actualizamos los textos de la fila y la columna actual
                filActualText.text = "Fil actual: " + (inicial.fil + 1);
                colActualText.text = "Col actual: " + (inicial.col + 1);
                camino = 0;
                break;
            case 6:

                //Actualizamos los textos de la fila y la columna actual
                filActualText.text = "Fil actual: " + (laberintoHistory.fil + 1);
                colActualText.text = "Col actual: " + (laberintoHistory.col + 1);
                //Pintamos el primer paso del laberinto
                printLaberintoFirstStep(laberintoHistory, inicial);
                camino = 0;
                break;
            case 7:
                //Dejamos de mostrar el esFactible si el paso es de retroceso
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                camino = 0;
                break;
            case 8:
                printLaberintoFirstStep(laberintoHistory, inicial);
                //Mostramos el esFactible si el paso es de retroceso
                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                camino = 0;
                break;
            case 9:
                //Pintamos la casilla del laberinto en base al esfactible de la casilla
                printLaberintoColorText(laberintoHistory);
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                camino = 0;
                break;
            case 10:

                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(false);
                }
                //Segundo if de la ejecucion si es factible dicho movimiento, entonces pasaremos a la llamada recursiva, sino continuaremos con la ejecucion normal
                if (laberintoHistory.esFactible)
                {
                    printLaberintoFirstStep(laberintoHistory, inicial);
                    camino = 0;
                }
                else
                {
                    filActualText.text = "Fil actual: " + (inicial.fil + 1);
                    colActualText.text = "Col actual: " + (inicial.col + 1);
                    printLaberintoFirstStep(inicial, inicial);
                    camino = 1;

                }
                break;
        }

        menu.pila(fase, camino, i, backStep, textosSinPintar, laberintoHistory, problem);
    }

    //Este método pintará el siguiente estado del laberinto y esperara un numero de segundos.
    IEnumerator waitForNextText(DataHistory laberinto, bool lastMove, DataHistory laberintoAnterior)
    {
        faseDePintado(laberinto, fase, lastMove, laberintoAnterior);
        yield return new WaitForSeconds(speedNextMove.value);
        var results = menu.waitForNextText(lastMove, fase, nextText, nextStep, laberinto, problem);
        nextStep = results.Item1;
        nextText = results.Item2;
        fase = results.Item3;
    }

    //Pintamos las casillas del laberinto dependiendo (el color) de si es una pared (negro), una casilla transitable (blanco) o si es la inicial (gris).
    public void pintarEstadoTablero(int[,] lab) {

        for(int i = 0; i < tamaño; i++)
        {
            for(int j = 0; j < tamaño; j++)
            {
                if (lab[i, j] == -1)
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.black;
                }else if(lab[i,j] == 0)
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.grey;
                }
            }
        }
    }
    /*
    public void printInitialLaberinto(DataHistory laberintoActual)
    {
        laberintoText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            laberintoText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                if (laberintoActual.fil == i && laberintoActual.col == j)
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.cyan;
                    if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                    {
                        if (j != 11)
                        {
                            laberintoText.text += " <color=cyan>" + laberintoActual.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            laberintoText.text += " <color=cyan>" + laberintoActual.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 11)
                        {
                            laberintoText.text += "<color=cyan>" + laberintoActual.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            laberintoText.text += "<color=cyan>" + laberintoActual.board[i, j] + "</color>";
                        }
                    }
                }
                else {
                    if (laberintoActual.board[i, j] == -1)
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.black;
                    }else if (laberintoActual.board[i, j] == 0)
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.grey;
                    }
                    if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                    {
                        if (j != 11)
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + "";
                        }
                    }
                    else
                    {
                        if (j != 11)
                        {
                            laberintoText.text += laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += laberintoActual.board[i, j] + "";
                        }
                    }
                }

            }
            laberintoText.text += "]\n";
        }
    }*/
    //Pinta el primer paso del laberinto en cada llamada recursiva indicando en que casilla estamos.
    public void printLaberintoFirstStep(DataHistory laberintoActual, DataHistory laberintoAnterior)
    {
        laberintoText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            laberintoText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                if (laberintoActual.fil == i && laberintoActual.col == j)
                {
                    //Pinta la parte visual
                    piezasTablero[i, j].GetComponent<Image>().color = Color.cyan;
                    //Pinta la matriz
                    if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                    {
                        if (j != 11)
                        {
                            laberintoText.text += " <color=cyan>" + laberintoActual.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            laberintoText.text += " <color=cyan>" + laberintoActual.board[i, j] + "</color>";
                        }
                    }
                    else
                    {
                        if (j != 11)
                        {
                            laberintoText.text += "<color=cyan>" + laberintoActual.board[i, j] + "</color>, ";
                        }
                        else
                        {
                            laberintoText.text += "<color=cyan>" + laberintoActual.board[i, j] + "</color>";
                        }
                    }
                }
                else
                {
                    Color color = piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().color;
                    //Pinta la parte visual
                    if (laberintoActual.board[i, j] == -1)
                    {
                        color.a = 0f;
                        piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
                        piezasTablero[i, j].GetComponent<Image>().color = Color.black;
                    }
                    else if (laberintoActual.board[i, j] == 0)
                    {
                        color.a = 0f;
                        piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        //Dibujamos el camino con flechas
                        int[][,] mov = new int[][,] { new int[,] { { 1, 0 } }, new int[,] { { 0, 1 } }, new int[,] { { -1, 0 } }, new int[,] { { 0, -1 } } };
                        if (laberintoActual.esFactible && (i != 1 || j != 1))
                        {
                            if (laberintoActual.board[i, j] == laberintoActual.board[(i + mov[0][0, 0]), (j + mov[0][0, 1])] - 1)
                            {
                                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("flecha_down");
                            }
                            if (laberintoActual.board[i, j] == laberintoActual.board[(i + mov[1][0, 0]), (j + mov[1][0, 1])] - 1)
                            {
                                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("flecha_right");
                            }
                            if (laberintoActual.board[i, j] == laberintoActual.board[(i + mov[2][0, 0]), (j + mov[2][0, 1])] - 1)
                            {
                                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("flecha_up");
                            }
                            if (laberintoActual.board[i, j] == laberintoActual.board[(i + mov[3][0, 0]), (j + mov[3][0, 1])] - 1)
                            {
                                piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("flecha_left");
                            }
                            color.a = 1f;
                            piezasTablero[i, j].transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
                        }
                        
                        piezasTablero[i, j].GetComponent<Image>().color = Color.grey;
                    }
                    //Pinta la matriz
                    if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                    {
                        if (j != 11)
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + "";
                        }
                    }
                    else
                    {
                        if (j != 11)
                        {
                            laberintoText.text += laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += laberintoActual.board[i, j] + "";
                        }
                    }
                }

            }
            laberintoText.text += "]\n";
        }
    }

    //Pinta el laberinto, marcando la casilla que estamos testeando para ver si nos podemos mover a ella o no, si es factible moverse a ella la coloreara de verde, lo hara de rojo si no lo es.
    public void printLaberintoColorText(DataHistory laberintoActual)
    {
        laberintoText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            laberintoText.text += "[";
            for (int j = 0; j < tamaño; j++)
            {
                if (laberintoActual.fil == i && laberintoActual.col == j)
                {
                    if (laberintoActual.esFactible)
                    {
                        //Pinta lo visual
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        //Pinta la matriz
                        if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                        {
                            if (j != 11)
                            {
                                laberintoText.text += " <color=green>" + laberintoActual.board[i, j] + "</color>, ";
                            }
                            else
                            {
                                laberintoText.text += " <color=green>" + laberintoActual.board[i, j] + "</color>";
                            }
                        }
                        else
                        {
                            if (j != 11)
                            {
                                laberintoText.text += "<color=green>" + laberintoActual.board[i, j] + "</color>, ";
                            }
                            else
                            {
                                laberintoText.text += "<color=green>" + laberintoActual.board[i, j] + "</color>";
                            }
                        }
                    }
                    else
                    {
                        //Pinta lo visual
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        //Pinta la matriz
                        if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                        {
                            if (j != 11)
                            {
                                laberintoText.text += " <color=red>" + laberintoActual.board[i, j] + "</color>, ";
                            }
                            else
                            {
                                laberintoText.text += " <color=red>" + laberintoActual.board[i, j] + "</color>";
                            }
                        }
                        else
                        {
                            if (j != 11)
                            {
                                laberintoText.text += "<color=red>" + laberintoActual.board[i, j] + "</color>, ";
                            }
                            else
                            {
                                laberintoText.text += "<color=red>" + laberintoActual.board[i, j] + "</color>";
                            }
                        }

                    }
                }
                else
                {
                    //Pinta lo visual
                    if (laberintoActual.board[i, j] == -1)
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.black;
                    }
                    else if (laberintoActual.board[i, j] == 0)
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.grey;
                    }
                    //Pinta la matriz
                    if (laberintoActual.board[i, j] != -1 && laberintoActual.board[i, j] < 10)
                    {
                        if (j != 11)
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += " " + laberintoActual.board[i, j] + "";
                        }
                    }
                    else
                    {
                        if (j != 11)
                        {
                            laberintoText.text += laberintoActual.board[i, j] + ", ";
                        }
                        else
                        {
                            laberintoText.text += laberintoActual.board[i, j] + "";
                        }
                    }
                }

            }
            laberintoText.text += "]\n";
        }
    }

    public int[,] inicializarLaberinto() {
        int[,] laberinto = new int[tamaño, tamaño];
        int[][,] paredes = new int[][,] { new int[,] { { 0, 2 } }, new int[,] { { 0, 7 } }, new int[,] { { 1, 0 } }, new int[,] { { 1, 2 } }, new int[,] { { 1, 5 } }, new int[,] { { 1, 6 } }, new int[,] { { 1, 8 } }, new int[,] { { 2, 6 } }, new int[,] { { 2, 8 } }, new int[,] { { 3, 1 } }, new int[,] { { 3, 4 } }, new int[,] { { 3, 5 } }, new int[,] { { 3, 6 } }, new int[,] { { 4, 2 } }, new int[,] { { 4, 3 } }, new int[,] { { 4, 7 } }, new int[,] { { 5, 5 } }, new int[,] { { 5, 7 } }, new int[,] { { 6, 0 } }, new int[,] { { 6, 3 } }, new int[,] { { 6, 4 } }, new int[,] { { 6, 7 } }, new int[,] { { 6, 9 } }, new int[,] { { 7, 1 } }, new int[,] { { 7, 2 } }, new int[,] { { 7, 8 } }, new int[,] { { 7, 9 } }, new int[,] { { 8, 2 } }, new int[,] { { 8, 4 } }, new int[,] { { 8, 5 } } };
        foreach(var pared in paredes)
        {
            laberinto[pared[0,0] + 1,pared[0,1] + 1]= -1;
        }
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                if (i == 0 || i == tamaño - 1 || j == 0 || j == tamaño - 1) {
                    laberinto[i, j] = -1;
                }
            }
        }
        return laberinto;
    }

    //Inicializa y guarda referencia a todos los textos del problema
    public void inicializarTextos()
    {
        List<TextMeshProUGUI> aux1 = new List<TextMeshProUGUI>();
        aux1.Add(laberintoVAText);
        aux1.Add(laberintoVAText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(ifFText);
        aux1.Add(ifFText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(esSolTrueText);
        aux1.Add(elseFText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(esSolFalseText);
        aux1.Add(returnLaberintoText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(movText);
        aux1.Add(movText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(i0Text);
        aux1.Add(i0Text);
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
        aux1.Add(returnEsFactibleText);
        aux1.Add(returnEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(laberintoKText);
        aux1.Add(i1Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(laberintoVARexursiveText);
        aux1.Add(laberintoVARexursiveText);
        textos.Add(aux1);

        List<string> aux = new List<string>();
        aux.Add("def labVA(lab, f, c, k):");
        aux.Add("def labVA(lab, f, c, k):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if f == np.size(lab, 0)-1 and c == np.size(lab, 1)-1:");
        aux.Add("if f == np.size(lab, 0)-1 and c == np.size(lab, 1)-1:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("esSol = True");
        aux.Add("else:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("esSol = False");
        aux.Add("return lab, esSol");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("mov = np.array([[1, 0],[0, 1],[-1, 0],[0, -1]])");
        aux.Add("mov = np.array([[1, 0],[0, 1],[-1, 0],[0, -1]])");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("i = 0");
        aux.Add("i = 0");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("while not esSol and i < np.size(mov, 0):");
        aux.Add("while not esSol and i < np.size(mov, 0):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if esFactible(lab, f + mov[i,0], c + mov[i,1]):");
        aux.Add("if esFactible(lab, f + mov[i,0], c + mov[i,1]):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("def esFactible(lab, f, c):");
        aux.Add("def esFactible(lab, f, c):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("return f >= 0 and f < np.size(lab, 0) and c >= 0 and c < np.size(lab, 1) and lab[f][c] == 0");
        aux.Add("return f >= 0 and f < np.size(lab, 0) and c >= 0 and c < np.size(lab, 1) and lab[f][c] == 0");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("lab[f + mov[i,0], c + mov[i,1]] = k");
        aux.Add("i += 1");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("[lab, esSol] = labVA(lab, f + mov[i,0], c + mov[i,1], k + 1)");
        aux.Add("[lab, esSol] = labVA(lab, f + mov[i,0], c + mov[i,1], k + 1)");
        textosSinPintar.Add(aux);

        menu.addTextos(textos);
    }

    //Metodo esfactible para resolver el problema
    public bool esFactible(int[,] lab, int f, int c)
    {
        return f >= 1 && f < tamaño-1 && c >= 0 && c < tamaño-1 && lab[f, c] == 0; 
    }
    
    //Codigo resolverLaberinto
    public Tuple<int[,], bool> resolverLaberintoVA(int[,] laberinto, int[,] labVisualizer, int f, int c, int k)
    {
        bool esSol;
        if (f == tamaño - 2 && c == tamaño - 2){
            esSol = true;
            int[,] aux = new int[tamaño, tamaño];
            for (int i = 0; i < tamaño; i++)
            {
                for (int j = 0; j < tamaño; j++)
                {
                    aux[i, j] = labVisualizer[i, j];
                }
            }
            data = new DataHistory(aux, tamaño - 2, tamaño - 2, k, esSol, true, i);
            //Guardamos el ultimo tablero y lo añadimos a la lista.
            laberintoHistory.Add(data);
        }
        else{
            esSol = false;
            int[][,] mov = new int[][,] { new int[,] { { 1, 0 } }, new int[,] { { 0, 1 } }, new int[,] { { -1, 0 } }, new int[,] { { 0, -1 } } };
            int i = 0;
            while (!esSol && i < mov.Length)
            {
                bool factible = esFactible(laberinto, f + mov[i][0, 0], c + mov[i][0, 1]);
                if (factible)
                {
                    labVisualizer[f + mov[i][0, 0], c + mov[i][0, 1]] = k;
                }
                int[,] aux = new int[tamaño, tamaño];
                for (int u  = 0; u < tamaño; u++)
                {
                    for (int j = 0; j < tamaño; j++)
                    {
                        aux[u, j] = labVisualizer[u, j];
                    }
                }
                data = new DataHistory(aux, f + mov[i][0, 0], c + mov[i][0, 1], k, esSol, factible, i);
                //Añadimos cada uno de los tableros a la lista, con su correspondientes variables.
                laberintoHistory.Add(data);
                if (factible)
                {
                    laberinto[f + mov[i][0, 0], c + mov[i][0, 1]] = k;
                    var results = resolverLaberintoVA(laberinto, labVisualizer, f + mov[i][0, 0], c + mov[i][0, 1], k + 1);
                    laberinto = results.Item1;
                    esSol = results.Item2;
                    if (!esSol) {
                        laberinto[f + mov[i][0, 0], c + mov[i][0, 1]] = 0;
                        labVisualizer[f + mov[i][0, 0], c + mov[i][0, 1]] = 0;
                    }
                }
                i += 1;
            }
        }
        return Tuple.Create(laberinto, esSol);
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
            if (fase == 10 && !(laberintoHistory[i].esFactible))
            {
                fase = 6;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
            }
            else if (fase == 11)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
            }
            else if (fase == 3 && laberintoHistory.Count - 1 == i)
            {
                //Ejecucion terminada;
            }
            else
            {
                fase += 1;
                if (i != 0)
                {
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
                }
                else if (i < 0)
                {
                    i += 1;
                    List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                    List<int> fasesAsociadas = new List<int>();
                    BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
                }
                else
                {
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
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
                faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
            }
            else
            {
                faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
            }
        }
    }

}