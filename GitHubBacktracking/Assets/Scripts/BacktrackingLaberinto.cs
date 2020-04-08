using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BacktrackingLaberinto : MonoBehaviour
{

    public Text laberintoText;
    public TextMeshProUGUI esFactibleFunction,laberintoVAText, ifFText, esSolTrueText, elseFText, esSolFalseText, movText, returnLaberintoText, i0Text, whileNotEsSolText, ifEsFactibleText, esFactibleFunctionText,returnEsFactibleText,i1Text,laberintoKText,laberintoVARexursiveText;
    public TextMeshProUGUI kActualText, colActualText, filActualText, esSolActualText;
    public Image playButton;
    public Scrollbar speedNextMove;
    public GameObject objetoTablero;

    List<DataHistory> laberintoHistory;
    List<TextColor> pilaEjecucion;
    List<TextMeshProUGUI> textos;
    GameObject[,] piezasTablero;
    DataHistory data, inicial, laberintoBase;
    SharedCode menu;
    int tamaño;
    bool nextStep = true;
    bool backStep = false;
    bool firstTime = true;
    bool play = false;
    bool recuperarEstadoCorutina = false;
    int i = -1;
    int fase = 0;
    int Xini = 1;
    int Yini = 1;

    void Start() {
        kActualText.text = "K actual:";
        tamaño = 12;
        pilaEjecucion = new List<TextColor>();
        laberintoHistory = new List<DataHistory>();
        textos = new List<TextMeshProUGUI>();
        menu = new SharedCode();
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
        laberintoBase = new DataHistory(aux, Xini, Yini, 1, false, true);
        var results = resolverLaberintoVA(lab, labVisualizer, Xini, Yini, paso + 1);

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
            piezasTablero[indiceX, indiceY] = objetoTablero.transform.GetChild(i).gameObject;
            indiceY += 1;
        }
        pintarEstadoTablero(laberintoHistory[0].board);
    }

    void Update()
    {
        if (play)
        {
            if (nextStep && i < laberintoHistory.Count - 1)
            {
                nextStep = false;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                if (i == 0)
                {
                    estadoCorutina(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
                }
                else
                {
                    estadoCorutina(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
                }
            }
            else if (recuperarEstadoCorutina)
            {
                recuperarEstadoCorutina = false;
                if (i != 0)
                {
                    estadoCorutina(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
                }
                else
                {
                    estadoCorutina(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
                }
            }
        }

    }

    public void estadoCorutina(DataHistory boardHistory, int fase, bool lastMove, DataHistory laberintoAnterior)
    {
        //En que momento del pintado nos encontramos?

        switch (fase)
        {
            case 0:
                StartCoroutine(waitForDefLaberinto(boardHistory, lastMove, laberintoAnterior));
                break;
            case 1:
                StartCoroutine(waitForIfF(boardHistory, lastMove));
                break;
            case 2:
                StartCoroutine(waitForElseF(boardHistory, lastMove));
                break;
            case 3:
                StartCoroutine(waitForEsSolFalse(boardHistory, lastMove));
                break;
            case 4:
                StartCoroutine(waitForMov(boardHistory, lastMove));
                break;
            case 5:
                StartCoroutine(waitForI0(boardHistory, lastMove));
                break;
            case 6:
                StartCoroutine(waitForWhileNotEsSol(boardHistory, lastMove));
                break;
            case 7:
                StartCoroutine(waitForIfEsFactible(boardHistory, lastMove));
                break;
            case 8:
                StartCoroutine(waitForEsFactibleFunction(boardHistory, lastMove));
                break;
            case 9:
                StartCoroutine(waitForReturnEsFactible(boardHistory, lastMove));
                break;
            case 10:
                StartCoroutine(waitForLaberintoI(boardHistory, lastMove));
                break;
            case 11:
                StartCoroutine(waitForLaberintoVARecursive(boardHistory, lastMove));
                break;
        }
    }

    public void faseDePintado(DataHistory laberintoHistory, int fase, bool lastMove, DataHistory laberintoAnterior = null)
    {
        esSolActualText.text = "EsSol actual: " + laberintoHistory.esSol;
        kActualText.text = "K actual: " + laberintoHistory.k;

        switch (fase)
        {
            case 0:
                //Primer paso: pintado del defSudokuVA y del tablero; 
                inicial = laberintoAnterior;
                printInitialLaberinto(inicial);
                filActualText.text = "Fil actual: " + (inicial.fil + 1);
                colActualText.text = "Col actual: " + (inicial.col + 1);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                laberintoVAText.text = "def labVA(lab, f, c, k):";
                laberintoVAText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(laberintoVAText);
                }
                break;

            case 1:
                //Pintamos el ifCasilla;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                ifFText.text = "if f == np.size(lab, 0)-1 and c == np.size(lab, 1)-1:";
                ifFText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifFText);
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
                    elseFText.text = "else:";
                    elseFText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(elseFText);
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
                    returnLaberintoText.text = "return lab, esSol";
                    returnLaberintoText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(returnLaberintoText);
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
                if (backStep)
                {
                    inicial = laberintoAnterior;
                }
                printInitialLaberinto(inicial);
                filActualText.text = "Fil actual: " + (inicial.fil + 1);
                colActualText.text = "Col actual: " + (inicial.col + 1);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                movText.text = "mov = np.array([[1, 0],[0, 1],[-1, 0],[0, -1]])";
                movText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(movText);
                }
                break;
            case 5:
                //Pinatmos el N = np.size y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                i0Text.text = "i = 0";
                i0Text.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(i0Text);
                }
                break;
            case 6:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                filActualText.text = "Fil actual: " + (laberintoHistory.fil + 1);
                colActualText.text = "Col actual: " + (laberintoHistory.col + 1);
                printLaberintoFirstStep(laberintoHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                whileNotEsSolText.text = "while not esSol and i < np.size(mov, 0):";
                whileNotEsSolText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(whileNotEsSolText);
                }
                break;
            case 7:
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
                ifEsFactibleText.text = "if esFactible(lab, f + mov[i,0], c + mov[i,1]):";
                ifEsFactibleText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifEsFactibleText);
                }
                break;
            case 8:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printLaberintoFirstStep(laberintoHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (!backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                esFactibleFunctionText.text = "def esFactible(lab, f, c):";
                esFactibleFunctionText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(esFactibleFunctionText);
                }
                break;
            case 9:
                //printEsFactibleLaberinto();
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                printLaberintoColorText(laberintoHistory);
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                if (backStep)
                {
                    esFactibleFunction.gameObject.SetActive(true);
                }
                returnEsFactibleText.text = "return f >= 0 and f < np.size(lab, 0) and c >= 0 and c < np.size(lab, 1) and lab[f][c] == 0";
                if (laberintoHistory.esFactible)
                {
                    returnEsFactibleText.color = Color.green;
                }
                else
                {
                    returnEsFactibleText.color = Color.red;
                }
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(returnEsFactibleText);
                }
                break;
            case 10:
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
                if (laberintoHistory.esFactible)
                {
                    printLaberintoFirstStep(laberintoHistory);
                    laberintoKText.text = "lab[f + mov[i,0], c + mov[i,1]] = k";
                    laberintoKText.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(laberintoKText);
                    }
                }
                else
                {
                    filActualText.text = "Fil actual: " + (inicial.fil + 1);
                    colActualText.text = "Col actual: " + (inicial.col + 1);
                    printInitialLaberinto(inicial);
                    i1Text.text = "i += 1";
                    i1Text.color = Color.cyan;
                    if (backStep == false)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(i1Text);
                    }
                }
                break;
            case 11:
                //Pinatmos el if sudoku[fila,col] != 0 y lo añadimos a la pila de ejecucion;
                for (int i = 0; i < menu.piscinaTextos.Count; i++)
                {
                    menu.piscinaTextos[i].text = menu.piscinaTextosOriginal[i];
                    menu.piscinaTextos[i].color = Color.white;
                }
                laberintoVARexursiveText.text = "[lab, esSol] = labVA(lab, f + mov[i,0], c + mov[i,1], k + 1)";
                laberintoVARexursiveText.color = Color.cyan;
                if (backStep == false)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(i1Text);
                }
                break;

        }
    }

    IEnumerator waitForDefLaberinto(DataHistory laberinto, bool lastMove, DataHistory laberintoAnterior)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 0;
        faseDePintado(laberinto, fase, lastMove, laberintoAnterior);
        StartCoroutine(waitForIfF(laberinto, lastMove));
    }

    IEnumerator waitForIfF(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 1;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForElseF(laberinto, lastMove));
    }

    IEnumerator waitForElseF(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 2;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForEsSolFalse(laberinto, lastMove));
    }

    IEnumerator waitForEsSolFalse(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 3;
        faseDePintado(laberinto, fase, lastMove);
        if (lastMove)
        {
            //Ejecucion terminada;
        }
        else
        {
            StartCoroutine(waitForMov(laberinto, lastMove));
        }
    }

    IEnumerator waitForMov(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 4;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForI0(laberinto, lastMove));
    }

    IEnumerator waitForI0(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 5;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForWhileNotEsSol(laberinto, lastMove));
    }
    
    IEnumerator waitForWhileNotEsSol(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 6;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForIfEsFactible(laberinto, lastMove));
    }

    IEnumerator waitForIfEsFactible(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 7;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForEsFactibleFunction(laberinto, lastMove));
    }

    IEnumerator waitForEsFactibleFunction(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 8;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForReturnEsFactible(laberinto, lastMove));
    }

    IEnumerator waitForReturnEsFactible(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 9;
        faseDePintado(laberinto, fase, lastMove);
        StartCoroutine(waitForLaberintoI(laberinto, lastMove));
    }

    IEnumerator waitForLaberintoI(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 10;
        faseDePintado(laberinto, fase, lastMove);
        if (laberinto.esFactible)
        {
            StartCoroutine(waitForLaberintoVARecursive(laberinto, lastMove));
        }
        else
        {
            fase = 6;
            nextStep = true;
        }
    }

    IEnumerator waitForLaberintoVARecursive(DataHistory laberinto, bool lastMove)
    {
        yield return new WaitForSeconds(speedNextMove.value);
        fase = 11;
        faseDePintado(laberinto, fase, lastMove);
        fase = 0;
        nextStep = true;
    }

    public void inicializarTextos()
    {
        textos.Add(laberintoVAText);
        textos.Add(ifFText);
        textos.Add(esSolTrueText);
        textos.Add(elseFText);
        textos.Add(esSolFalseText);
        textos.Add(movText);
        textos.Add(returnLaberintoText);
        textos.Add(i0Text);
        textos.Add(whileNotEsSolText);
        textos.Add(ifEsFactibleText);
        textos.Add(esFactibleFunctionText);
        textos.Add(returnEsFactibleText);
        textos.Add(laberintoKText);
        textos.Add(laberintoVARexursiveText);
        textos.Add(i1Text);
        menu.addTextos(textos);
    }

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
    }

    public void printLaberintoFirstStep(DataHistory laberintoActual)
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
                else
                {
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
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
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
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
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

    public bool esFactible(int[,] lab, int f, int c)
    {
        return f >= 1 && f < tamaño-1 && c >= 0 && c < tamaño-1 && lab[f, c] == 0; 
    }
    
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
            data = new DataHistory(aux, tamaño - 2, tamaño - 2, k, esSol, true);
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
                data = new DataHistory(aux, f + mov[i][0, 0], c + mov[i][0, 1], k, esSol, factible);
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
            if (fase == 10 && !(laberintoHistory[i].esFactible))
            {
                fase = 6;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
            }
            else if (fase == 11)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                pilaEjecucion.Add(tableroEjecucion);
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
                    TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
                }
                else
                {
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
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
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoHistory[i - 1]);
                }
                else
                {
                    faseDePintado(laberintoHistory[i], fase, laberintoHistory.Count - 1 == i, laberintoBase);
                }

            }
        }
    }

}