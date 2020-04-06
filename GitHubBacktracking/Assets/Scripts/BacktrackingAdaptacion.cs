using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BacktrackingAdaptacion : MonoBehaviour
{
    public int tamaño = 4;
    public Text vectorSolucionText;
    public Image playButton;
    public TextMeshProUGUI esFactibleText, colText, diag1Text, diag2Text, defNReinasText, ifKText, esSolTrueText, elseText, esSolFalseText, col0Text, whileNotEsSolText, IfEsFactibleText, defEsFactibleFunc, defEsFactibleText, returnEsFactibleText, tab1Text, NReinasRecursiveText, col1Text, returnDefReinasText;
    public TextMeshProUGUI filActualText, colActualText, esSolActualText;
    public Text tablero;
    public Scrollbar speedNextMove;
    public GameObject targetPosition;
    public List<GameObject> objetoTablero;
    public List<GameObject> llamadasRecursivas;
    public List<TextMeshProUGUI> pieLlamada;
    public List<GameObject> posicionesIniciales;


 
    public List<DataHistory> boardHistory;
    public GameObject[,] piezasTablero;
    public GameObject[] reinas;
    public bool boardSolved = false;
    public bool firstTime = true;
    public bool play = true;
    public bool nextStep = true;
    public bool backStep = true;
    public bool nextN = false;
    public bool recuperarEstadoCorutina = false;
    public bool finished = false;
    public bool moverReina = false;
    public DataHistory data;
    public int i = -1;
    public int filaActual = 0;
    public int colActual = 0;
    public int fase = 0;
    public int filAnt;
    public float movementSpeed = 50f;
    public Vector3 towardsTarget;
    public float posicionXInicial;
    public List<int> vectorSolucion;
    public List<TextColor> pilaEjecucion;
    public List<TextMeshProUGUI> piscinaTextos;
    public List<string> piscinaTextosOriginal;

    // Start is called before the first frame update
    void Start()
    {
        tamaño = GameState.gameState.tamaño;
        pilaEjecucion = new List<TextColor>();
        piscinaTextos = new List<TextMeshProUGUI>();
        piscinaTextosOriginal = new List<string>();
        addTextos(piscinaTextos);
        //Inicializacion de las piezas del tablero
        piezasTablero = new GameObject[tamaño, tamaño];
        int indiceX = 0;
        int indiceY = 0;
        int i = 0;
        for (i = 0; i < tamaño*tamaño; i++) {

            if (indiceY >= tamaño)
            {
                indiceX += 1;
                indiceY = 0;
            }
            piezasTablero[indiceX, indiceY] = objetoTablero[tamaño - 4].transform.GetChild(i).gameObject;
            indiceY += 1;

        }
        //Inicializacion de las reinas del tablero.
        reinas = new GameObject[tamaño];
        for (int j=0; j < tamaño; j++)
        {
            reinas[j] = objetoTablero[tamaño - 4].transform.GetChild(i).gameObject;
            i += 1;
        }

        posicionXInicial = posicionesIniciales[tamaño - 4].gameObject.transform.position.x;
        //Activamos el tablero
        objetoTablero[tamaño - 4].gameObject.SetActive(true);
        int[,] boardVisualizer = new int[tamaño, tamaño];
        printInitialBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (boardSolved)
        {
            //Actualizacion de la posicion inicial de las reinas por si cambia el tamaño de la pantalla.
            posicionXInicial = posicionesIniciales[tamaño - 4].gameObject.transform.position.x;
            if (!finished)
            {
                esSolActualText.text = "EsSol: False";
                if (moverReina)
                {
                    moverReinas();
                }
                else
                {
                    if (play)
                    {
                        if (nextStep && i < boardHistory.Count - 1)
                        {
                            i += 1;
                            List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                            List<int> fasesAsociadas = new List<int>();
                            TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                            pilaEjecucion.Add(tableroEjecucion);
                            faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            estadoCorutina(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            foreach (GameObject funcion in llamadasRecursivas)
                            {
                                funcion.gameObject.SetActive(false);
                            }
                            filActualText.text = "Fil actual: " + boardHistory[i].vectorSolucion.Count;
                            foreach (int ultimo in boardHistory[i].vectorSolucion)
                            {
                                colActual = ultimo + 1;
                            }
                            colActualText.text = "Col actual: " + colActual;
                            for (int j = 0; j < boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                llamadasRecursivas[j].gameObject.SetActive(true);
                            }
                            for (int j = 0; j <= boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                pieLlamada[j].text = "" + (boardHistory[i].vectorSolucion.Count - j);
                            }
                            nextStep = false;
                        }
                        else if (nextN && i < boardHistory.Count - 1)
                        {
                            i += 1;
                            List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                            List<int> fasesAsociadas = new List<int>();
                            TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                            pilaEjecucion.Add(tableroEjecucion);
                            faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            estadoCorutina(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            foreach (GameObject funcion in llamadasRecursivas)
                            {
                                funcion.gameObject.SetActive(false);
                            }
                            filActualText.text = "Fil actual: " + boardHistory[i].vectorSolucion.Count;
                            foreach (int ultimo in boardHistory[i].vectorSolucion)
                            {
                                colActual = ultimo + 1;
                            }
                            colActualText.text = "Col actual: " + colActual;
                            for (int j = 0; j < boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                llamadasRecursivas[j].gameObject.SetActive(true);
                            }
                            for (int j = 0; j <= boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                pieLlamada[j].text = "" + (boardHistory[i].vectorSolucion.Count - j);
                            }
                            nextN = false;
                        }
                        else if (recuperarEstadoCorutina)
                        {
                            recuperarEstadoCorutina = false;
                            faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            estadoCorutina(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            foreach (GameObject funcion in llamadasRecursivas)
                            {
                                funcion.gameObject.SetActive(false);
                            }
                            filActualText.text = "Fil actual: " + boardHistory[i].vectorSolucion.Count;
                            foreach (int ultimo in boardHistory[i].vectorSolucion)
                            {
                                colActual = ultimo + 1;
                            }
                            colActualText.text = "Col actual: " + colActual;
                            for (int j = 0; j < boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                llamadasRecursivas[j].gameObject.SetActive(true);
                            }
                            for (int j = 0; j <= boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                pieLlamada[j].text = "" + (boardHistory[i].vectorSolucion.Count - j);
                            }
                        }
                    }
                    else
                    {
                        if (nextStep || backStep)
                        {
                            foreach (GameObject funcion in llamadasRecursivas)
                            {
                                funcion.gameObject.SetActive(false);
                            }
                            filActualText.text = "Fil actual: " + boardHistory[i].vectorSolucion.Count;
                            foreach (int ultimo in boardHistory[i].vectorSolucion)
                            {
                                colActual = ultimo + 1;
                            }
                            colActualText.text = "Col actual: " + colActual;
                            faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                            for (int j = 0; j < boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                llamadasRecursivas[j].gameObject.SetActive(true);
                            }
                            for (int j = 0; j <= boardHistory[i].vectorSolucion.Count - 1; j++)
                            {
                                pieLlamada[j].text = "" + (boardHistory[i].vectorSolucion.Count - j);
                            }
                            nextStep = false;
                            backStep = false;
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < boardHistory[i].vectorSolucion.Count - 1; j++)
                {
                    llamadasRecursivas[j].gameObject.SetActive(false);
                }
                pieLlamada[0].text = "" + 1;
                esSolActualText.text = "EsSol: True";
            }
        }
    }

    public void moverReinas() {
        towardsTarget = targetPosition.transform.position - reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position;
        reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position += towardsTarget.normalized * movementSpeed * Time.deltaTime;
        float error = 0.03f;
        if (reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position.x > targetPosition.transform.position.x -error && reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position.x < targetPosition.transform.position.x + error)
        {
            reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position = targetPosition.transform.position;
            moverReina = false;
        }
        Debug.DrawLine(reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position, targetPosition.transform.position, Color.green);
    }
   
    //Contiene todos los textos del codigo.
    public void addTextos(List<TextMeshProUGUI> piscinaTextos)
    {
        piscinaTextos.Add(esFactibleText);
        piscinaTextos.Add(colText);
        piscinaTextos.Add(diag1Text);
        piscinaTextos.Add(diag2Text);
        piscinaTextos.Add(defNReinasText);
        piscinaTextos.Add(ifKText);
        piscinaTextos.Add(esSolTrueText);
        piscinaTextos.Add(elseText);
        piscinaTextos.Add(esSolFalseText);
        piscinaTextos.Add(col0Text);
        piscinaTextos.Add(whileNotEsSolText);
        piscinaTextos.Add(IfEsFactibleText);
        piscinaTextos.Add(defEsFactibleText);
        piscinaTextos.Add(returnEsFactibleText);
        piscinaTextos.Add(tab1Text);
        piscinaTextos.Add(NReinasRecursiveText);
        piscinaTextos.Add(col1Text);
        piscinaTextos.Add(returnDefReinasText);

        piscinaTextosOriginal.Add(esFactibleText.text);
        piscinaTextosOriginal.Add(colText.text);
        piscinaTextosOriginal.Add(diag1Text.text);
        piscinaTextosOriginal.Add(diag2Text.text);
        piscinaTextosOriginal.Add(defNReinasText.text);
        piscinaTextosOriginal.Add(ifKText.text);
        piscinaTextosOriginal.Add(esSolTrueText.text);
        piscinaTextosOriginal.Add(elseText.text);
        piscinaTextosOriginal.Add(esSolFalseText.text);
        piscinaTextosOriginal.Add(col0Text.text);
        piscinaTextosOriginal.Add(whileNotEsSolText.text);
        piscinaTextosOriginal.Add(IfEsFactibleText.text);
        piscinaTextosOriginal.Add(defEsFactibleText.text);
        piscinaTextosOriginal.Add(returnEsFactibleText.text);
        piscinaTextosOriginal.Add(tab1Text.text);
        piscinaTextosOriginal.Add(NReinasRecursiveText.text);
        piscinaTextosOriginal.Add(col1Text.text);
        piscinaTextosOriginal.Add(returnDefReinasText.text);
    }

    public void estadoCorutina(DataHistory boardHistory, int fase, bool lastMove)
    {
        //En que momento del pintado nos encontramos?

        switch (fase)
        {
            case 0:
                //IniciarPintado
                if (nextStep || play)
                {
                    StartCoroutine(waitForNextMove(boardHistory, lastMove));
                    nextStep = false;
                }
                else if (nextN || play) {
                    StartCoroutine(waitForWhileNotEsSol(boardHistory, lastMove));
                    nextN = false;
                }
                break;
            case 1:
                //Pintar el tablero sin las columnas o diagonales factibles
                StartCoroutine(waitForNextMove(boardHistory, lastMove));
                break;
            case 2:
                //Pintar la cabecera de la funcion NReinas.
                StartCoroutine(waitForDefNReinas(boardHistory, lastMove));
                //defNReinasText.color = Color.cyan;
                break;
            case 3:
                StartCoroutine(waitForIFK(boardHistory, lastMove));
                //defNReinasText.color = Color.white;
                //ifKText.color = Color.cyan;
                break;
            case 4:
                if (lastMove)
                {
                    StartCoroutine(waitForEsSolTrue(boardHistory, lastMove));
                    //ifKText.color = Color.white;
                    //esSolTrueText.color = Color.cyan;
                }
                else
                {
                    StartCoroutine(waitForElse(boardHistory, lastMove));
                    //ifKText.color = Color.white;
                    //elseText.color = Color.cyan;
                }
                break;
            case 5:

                if (lastMove)
                {
                    StartCoroutine(waitForReturnDefReinas(boardHistory, lastMove));
                    //esSolTrueText.color = Color.white;
                    //returnDefReinasText.color = Color.cyan;
                    //Acaba la ejecucion.
                }
                else
                {
                    StartCoroutine(waitForEsSolFalse(boardHistory, lastMove));
                    //elseText.color = Color.white;
                    //esSolFalseText.color = Color.cyan;
                }
                break;
            case 6:
                StartCoroutine(waitForN0(boardHistory, lastMove));
                //esSolFalseText.color = Color.white;
                //n0Text.color = Color.cyan;
                break;

            case 7:
                StartCoroutine(waitForWhileNotEsSol(boardHistory, lastMove));
                //printBoardFirstStep(boardHistory);
                //n0Text.color = Color.white;
                //whileNotEsSolText.color = Color.cyan;
                break;

            case 8:
                StartCoroutine(waitForIfEsFactible(boardHistory, lastMove));
                //whileNotEsSolText.color = Color.white;
                //IfEsFactibleText.color = Color.cyan;
                break;

            case 9:
                StartCoroutine(waitForDefEsFactible(boardHistory, lastMove));
                //IfEsFactibleText.color = Color.white;
                //defEsFactibleText.color = Color.cyan;
                break;

            case 10:
                StartCoroutine(waitForFactibleTint(boardHistory, lastMove));
                //defEsFactibleText.color = Color.white;
                //printBoard(boardHistory);
                break;

            case 11:
                StartCoroutine(waitForReturnEsFactible(boardHistory, lastMove));
                //colText.color = Color.white;
                //diag1Text.color = Color.white;
                //diag2Text.color = Color.white;
                //returnEsFactibleText.color = Color.cyan;
                break;

            case 12:
                if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
                {
                    //Reseteo de es factible y vuelta a la funcion nReinas.
                    StartCoroutine(waitForTupla1Text(boardHistory, lastMove));
                    //printBoardFirstStep(boardHistory);
                    //returnEsFactibleText.color = Color.white;
                    //tupla1Text.color = Color.cyan;
                }
                else
                {
                    //Reseteo de es factible y vuelta a la funcion nReinas.
                    StartCoroutine(waitForn1Text(boardHistory, lastMove));
                    //printBoardFirstStep(boardHistory);
                    //returnEsFactibleText.color = Color.white;
                    //n1Text.color = Color.cyan;
                }

                break;

            case 13:
                if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
                {
                    StartCoroutine(waitForNReinasText(boardHistory, lastMove));
                    //tupla1Text.color = Color.white;
                    //NReinasRecursiveText.color = Color.cyan;
                }
                else
                {
                    StartCoroutine(waitForAdvanceCol(boardHistory, lastMove));
                    //n1Text.color = Color.white;
                }

                break;

            case 14:
                StartCoroutine(waitForAdvanceFila(boardHistory, lastMove));
                //NReinasRecursiveText.color = Color.white;
                break;
        }
    }

    public void faseDePintado(DataHistory boardHistory,int fase, bool lastMove) {
        //En que momento del pintado nos encontramos?

        switch (fase) {
            case 0:
                //IniciarPintado
                for(int i = 0; i<piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                pilaEjecucion[i].pilaPintado.Clear();
                break;
            case 1:
                //Pintar el tablero sin las columnas o diagonales factibles
                //waitForNextMove(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                break;
            case 2:
                //Pintar la cabecera de la funcion NReinas.
                //waitForDefNReinas(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                defNReinasText.text = "def NReinasVA(tab, fil):";
                defNReinasText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(defNReinasText);
                }
                break;
            case 3:
                //waitForIFK(boardHistory, lastMove)
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                ifKText.text = "if fil == np.size(tab, 0):";
                ifKText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(ifKText);
                }
                break;
            case 4:
                if (lastMove)
                {
                    //waitForEsSolTrue(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    esSolTrueText.text = "esSol = True";
                    esSolTrueText.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(esSolTrueText);
                    }
                }
                else
                {
                    //waitForElse(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    elseText.text = "else:";
                    elseText.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(elseText);
                    }
                }
                break;
            case 5:

                if (lastMove)
                {
                    //waitForReturnDefReinas(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    returnDefReinasText.text = "return tab, esSol";
                    returnDefReinasText.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(returnDefReinasText);
                    }
                    finished = true;
                    //Acaba la ejecucion.
                }
                else
                {
                    //waitForEsSolFalse(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    esSolFalseText.text = "esSol = False";
                    esSolFalseText.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(esSolFalseText);
                    }
                }
                break;
            case 6:
                //waitForN0(boardHistory, lastMove)
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                col0Text.text = "col = 0";
                col0Text.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(col0Text);
                }
                break;

            case 7:
                //waitForWhileNotEsSol(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                whileNotEsSolText.text = "while not esSol and col < np.size(tab, 1):";
                whileNotEsSolText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(whileNotEsSolText);
                }
                break;

            case 8:
                //waitForIfEsFactible(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                if (backStep)
                {
                    defEsFactibleFunc.gameObject.SetActive(false);
                }
                IfEsFactibleText.text = "if EsFactible(tab, fil, col):";
                IfEsFactibleText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(IfEsFactibleText);
                }
                break;

            case 9:
                //waitForDefEsFactible(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                defEsFactibleFunc.gameObject.SetActive(true);
                defEsFactibleText.text = "def EsFactible(tab, f, c):";
                defEsFactibleText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(defEsFactibleText);
                }
                break;

            case 10:
                //waitForFactibleTint(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                printBoard(boardHistory);

                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(colText);
                }
                break;

            case 11:
                //waitForReturnEsFactible(boardHistory, lastMove)
                printBoardFirstStep(boardHistory);
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                if (backStep)
                {
                    defEsFactibleFunc.gameObject.SetActive(true);
                }
                returnEsFactibleText.text = "return colOk and diag1Ok and diag2Ok";
                returnEsFactibleText.color = Color.cyan;
                if (nextStep || play)
                {
                    pilaEjecucion[i].fases.Add(fase);
                    pilaEjecucion[i].pilaPintado.Add(returnEsFactibleText);
                }
                
                break;

            case 12:
                printBoardFirstStep(boardHistory);
                defEsFactibleFunc.gameObject.SetActive(false);
                if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
                {
                    //Reseteo de es factible y vuelta a la funcion nReinas.
                    //waitForTupla1Text(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    tab1Text.text = "tab[fil][col] = 1";
                    tab1Text.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(tab1Text);
                    }
                }
                else
                {
                    //Reseteo de es factible y vuelta a la funcion nReinas.
                    //waitForn1Text(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    col1Text.text = "col += 1";
                    col1Text.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(col1Text);
                    }
                }

                break;

            case 13:
                if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
                {
                    //waitForNReinasText(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                    NReinasRecursiveText.text = "[tab, esSol] = nReinasVA(tab, fil + 1)";
                    NReinasRecursiveText.color = Color.cyan;
                    if (nextStep || play)
                    {
                        pilaEjecucion[i].fases.Add(fase);
                        pilaEjecucion[i].pilaPintado.Add(NReinasRecursiveText);
                    }
                }
                else
                {
                    //waitForAdvanceCol(boardHistory, lastMove)
                    for (int i = 0; i < piscinaTextos.Count; i++)
                    {
                        piscinaTextos[i].text = piscinaTextosOriginal[i];
                        piscinaTextos[i].color = Color.white;
                    }
                }

                break;

            case 14:
                //waitForAdvanceFila(boardHistory, lastMove)
                for (int i = 0; i < piscinaTextos.Count; i++)
                {
                    piscinaTextos[i].text = piscinaTextosOriginal[i];
                    piscinaTextos[i].color = Color.white;
                }
                break;
        }
    }

    IEnumerator waitForNextMove(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForDefNReinas(boardHistory, lastMove));
        }
    }

    IEnumerator waitForDefNReinas(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForIFK(boardHistory, lastMove));
        }
    }

    IEnumerator waitForIFK(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase,lastMove);
            if (lastMove)
            {
                StartCoroutine(waitForEsSolTrue(boardHistory, lastMove));
            }
            else
            {
                StartCoroutine(waitForElse(boardHistory, lastMove));
            }
        }
    }

    IEnumerator waitForEsSolTrue(DataHistory boardHistory, bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForReturnDefReinas(boardHistory, lastMove));
        }
    }

    IEnumerator waitForElse(DataHistory boardHistory, bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForEsSolFalse(boardHistory, lastMove));
        }
    }

    IEnumerator waitForReturnDefReinas(DataHistory boardHistory, bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            //Acaba la ejecucion.
        }
    }

    IEnumerator waitForEsSolFalse(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForN0(boardHistory, lastMove));
        }
    }

    IEnumerator waitForN0(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForWhileNotEsSol(boardHistory, lastMove));
        }
    }

    IEnumerator waitForWhileNotEsSol(DataHistory boardHistory, bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForIfEsFactible(boardHistory, lastMove));
        }
    }

    IEnumerator waitForIfEsFactible(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForDefEsFactible(boardHistory, lastMove));
        }
    }
    
    IEnumerator waitForDefEsFactible(DataHistory boardHistory, bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForFactibleTint(boardHistory, lastMove));
        }
    }

    IEnumerator waitForFactibleTint(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            //Se pinta la columna y diagonales del tablero y código.
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForReturnEsFactible(boardHistory, lastMove));
        }
    }

    IEnumerator waitForReturnEsFactible(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            //Reseteo de los colores del tablero.
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
            {
                StartCoroutine(waitForTupla1Text(boardHistory, lastMove));
            }
            else
            {
                StartCoroutine(waitForn1Text(boardHistory, lastMove));
            }
        }
    }

    IEnumerator waitForTupla1Text(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForNReinasText(boardHistory, lastMove));
        }
    }

    IEnumerator waitForn1Text(DataHistory boardHistory, bool lastMove)
    {
        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForAdvanceCol(boardHistory, lastMove));
        }
    }

    IEnumerator waitForNReinasText(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            StartCoroutine(waitForAdvanceFila(boardHistory, lastMove));
        }
    }

    IEnumerator waitForAdvanceCol(DataHistory boardHistory,bool lastMove)
    {
        if (play)
        {
            //Llamada recursiva a la funcion de NReinas.
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            fase = 7;
            nextN = true;
        }
    }
    
    IEnumerator waitForAdvanceFila(DataHistory boardHistory,bool lastMove)
    {

        if (play)
        {
            //Avanza la reina a la siguiente columna.
            yield return new WaitForSeconds(speedNextMove.value);
            fase += 1;
            faseDePintado(boardHistory, fase, lastMove);
            fase = 0;
            if (lastMove)
            {
                StartCoroutine(waitForNextMove(boardHistory, lastMove));
            }
            nextStep = true;
        }
    }

    public void printInitialBoard()
    {
        tablero.text = "Tab\n";
        for (int i = 0; i < tamaño; i++)
        {
            for (int j = 0; j < tamaño; j++)
            {
                tablero.text += "0 ";
            }
            tablero.text += "\n";
        }
    }

    public void printBoardFirstStep(DataHistory boardHistory)
    {
        tablero.text = "Tab\n";
        vectorSolucionText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            reinas[i].GetComponent<Image>().color = Color.white;
            if (i > boardHistory.fil)
            {
                reinas[i].transform.position = new Vector3(posicionXInicial, reinas[i].transform.position.y, reinas[i].transform.position.z);
            }
            for (int j = 0; j < tamaño; j++)
            {
                if (boardHistory.vectorSolucion != null && i == boardHistory.fil && j == boardHistory.col)
                {
                    targetPosition.transform.position = piezasTablero[i, j].transform.position;
                    moverReina = true;
                    //reinas[i].transform.position = piezasTablero[i, j].transform.position;
                    reinas[i].GetComponent<Image>().color = Color.cyan;
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white; 
                    tablero.text += "<color=cyan>" + boardHistory.board[i, j] + "</color> ";
                }
                else
                {
                    piezasTablero[i, j].GetComponent<Image>().color = Color.white;
                    tablero.text += boardHistory.board[i, j] + " ";
                }
            }
            tablero.text += "\n";
        }
        
        if (boardHistory.vectorSolucion != null)
        {
            for (int i = 0; i < boardHistory.vectorSolucion.Count; i++)
            {
                vectorSolucionText.text += boardHistory.vectorSolucion[i] + "  ";
            }

        }

        colText.color = Color.white;
        diag1Text.color = Color.white;
        diag2Text.color = Color.white;
    }

    public void printBoard(DataHistory boardHistory)
    {
        tablero.text = "Tab\n";
        vectorSolucionText.text = "";
        for (int i = 0; i < tamaño; i++)
        {
            reinas[i].GetComponent<Image>().color = Color.white;
            for (int j = 0; j < tamaño; j++)
            {
                if (boardHistory.vectorSolucion != null && i == boardHistory.fil && j == boardHistory.col)
                {
                    targetPosition.transform.position = piezasTablero[i, j].transform.position;
                    moverReina = true;
                    //reinas[i].transform.position = piezasTablero[i, j].transform.position;
                    if (boardHistory.diag1Ok && boardHistory.diag2Ok && boardHistory.colOk)
                    {
                        reinas[i].GetComponent<Image>().color = Color.green;
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        tablero.text += "<color=green>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else
                    {
                        reinas[i].GetComponent<Image>().color = Color.red;
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        tablero.text += "<color=red>" + boardHistory.board[i, j] + "</color> ";
                    }

                }
                else
                {
                    EsFactibleTint(boardHistory);
                    if (boardHistory.colOk && printCol(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        tablero.text += "<color=green>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.colOk == false && printCol(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        tablero.text += "<color=red>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.diag1Ok && printDiag1(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        tablero.text += "<color=green>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.diag1Ok == false && printDiag1(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        tablero.text += "<color=red>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.diag2Ok && printDiag2(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        tablero.text += "<color=green>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.diag2Ok == false && printDiag2(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        tablero.text += "<color=red>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else
                    {
                        tablero.text += boardHistory.board[i, j] + " ";
                    }


                }
            }
            tablero.text += "\n";
        }
        if (boardHistory.vectorSolucion != null)
        {
            for (int i = 0; i < boardHistory.vectorSolucion.Count; i++)
            {
                vectorSolucionText.text += boardHistory.vectorSolucion[i] + "  ";
            }

        }
    }

    public void EsFactibleTint(DataHistory boardHistory)
    {
        colText.text = "colOk = np.max(tab[:, c]) == 0 ";
        diag1Text.text = "diag1Ok = True\n"+
                         "indF = f - 1\n"+
                         "indC = c - 1\n"+
                         "while diag1Ok and indF >= 0 and indC >= 0:\n"+
                         "      diag1Ok = tab[indF, indC] == 0\n"+
                         "      indF -= 1\n"+
                         "      indC -= 1\n";
        diag2Text.text = "diag2Ok = True\n" +
                         "indF = f - 1\n" +
                         "indC = c + 1\n" +
                         "while diag2Ok and indF >= 0 and indC < np.size(tab,1):\n" +
                         "      diag2Ok = tab[indF, indC] == 0\n" +
                         "      indF -= 1\n" +
                         "      indC += 1\n";
        if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
        {
            colText.color = Color.green;
            diag1Text.color = Color.green;
            diag2Text.color = Color.green;
        }
        else if (boardHistory.colOk && boardHistory.diag1Ok && !boardHistory.diag2Ok)
        {
            colText.color = Color.green;
            diag1Text.color = Color.green;
            diag2Text.color = Color.red;
        }
        else if (!boardHistory.colOk && !boardHistory.diag1Ok && !boardHistory.diag2Ok)
        {
            colText.color = Color.red;
            diag1Text.color = Color.red;
            diag2Text.color = Color.red;
        }
        else if (boardHistory.colOk && !boardHistory.diag1Ok && boardHistory.diag2Ok)
        {
            colText.color = Color.green;
            diag1Text.color = Color.red;
            diag2Text.color = Color.green;
        }
        else if (boardHistory.colOk && !boardHistory.diag1Ok && !boardHistory.diag2Ok)
        {
            colText.color = Color.green;
            diag1Text.color = Color.red;
            diag2Text.color = Color.red;
        }
        else if (!boardHistory.colOk && !boardHistory.diag1Ok && boardHistory.diag2Ok)
        {
            colText.color = Color.red;
            diag1Text.color = Color.red;
            diag2Text.color = Color.green;
        }
        else if (!boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
        {
            colText.color = Color.red;
            diag1Text.color = Color.green;
            diag2Text.color = Color.green;
        }
        else if (!boardHistory.colOk && boardHistory.diag1Ok && !boardHistory.diag2Ok)
        {
            colText.color = Color.red;
            diag1Text.color = Color.green;
            diag2Text.color = Color.red;
        }
    }

    public bool printDiag1(int f, int c, int i, int j)
    {
        bool subDiag = false;
        bool topDiag = false;
        int indF = f - 1;
        int indC = c - 1;

        while (indF >= 0 && indC >= 0)
        {
            if (i == indF && j == indC)
            {
                subDiag = true;
            }
            indF -= 1;
            indC -= 1;
        }
        indF = f - 1;
        indC = c - 1;

        while (indF < tamaño && indC < tamaño)
        {
            if (i == indF && j == indC)
            {
                topDiag = true;
            }
            indF += 1;
            indC += 1;
        }

        return subDiag || topDiag;
    }

    public bool printDiag2(int f, int c, int i, int j)
    {
        bool subDiag = false;
        bool topDiag = false;
        int indF = f - 1;
        int indC = c + 1;

        while (indF >= 0 && indC < tamaño)
        {
            if (i == indF && j == indC)
            {
                subDiag = true;
            }
            indF -= 1;
            indC += 1;
        }
        indF = f + 1;
        indC = c - 1;

        while (indF < tamaño && indC >= 0)
        {
            if (i == indF && j == indC)
            {
                topDiag = true;
            }
            indF += 1;
            indC -= 1;
        }

        return subDiag || topDiag;
    }

    public bool printCol(int f, int c, int i, int j)
    {
        bool subCol = false;
        bool topCol = false;
        int indF = f - 1;
        int indC = c;

        while (indF >= 0)
        {
            if (i == indF && j == indC)
            {
                subCol = true;
            }
            indF -= 1;
        }
        indF = f + 1;
        indC = c;

        while (indF < tamaño)
        {
            if (i == indF && j == indC)
            {
                topCol = true;
            }
            indF += 1;
        }

        return subCol || topCol;
    }

    public Tuple<bool, bool, bool> EsFactible(int[,] board, int f, int c)
    {
        bool diag1Ok = true;
        int indF = f - 1;
        int indC = c - 1;

        while (indF >= 0 && indC >= 0 && diag1Ok == true)
        {
            if (board[indF, indC] == 1)
            {
                diag1Ok = false;
            }
            indF -= 1;
            indC -= 1;
        }
        bool diag2Ok = true;
        indF = f - 1;
        indC = c + 1;

        while (indF >= 0 && diag2Ok == true && indC < tamaño)
        {
            if (board[indF, indC] == 1)
            {
                diag2Ok = false;
            }
            indF -= 1;
            indC += 1;
        }

        bool colOk = true;
        indF = f - 1;
        indC = c;

        while (indF >= 0 && colOk == true)
        {
            if (board[indF, indC] == 1)
            {
                colOk = false;
            }
            indF -= 1;
        }

        return Tuple.Create(diag1Ok, diag2Ok, colOk);
    }

    public Tuple<int[,], bool> NReinas(int[,] board, int fil, int[,] boardVisualizer, List<int> vectorSolucion)
    {
        bool esSol;
        if (fil >= tamaño)
        {
            esSol = true;
        }
        else
        {
            esSol = false;
            int col = 0;
            while (esSol == false && col < tamaño)
            {
                boardVisualizer[fil, col] = 1;
                int[,] aux = new int[tamaño, tamaño];
                for (int i = 0; i < tamaño; i++)
                {
                    for (int j = 0; j < tamaño; j++)
                    {
                        aux[i, j] = boardVisualizer[i, j];
                    }
                }
                var factibles = EsFactible(board, fil, col);
                data = new DataHistory(aux, fil, col, factibles.Item1, factibles.Item2, factibles.Item3);
                boardHistory.Add(data);
                if (factibles.Item1 && factibles.Item2 && factibles.Item3)
                {
                    board[fil, col] = 1;
                    var result = NReinas(board, fil + 1, boardVisualizer, vectorSolucion);
                    board = result.Item1;
                    esSol = result.Item2;
                    if (esSol == false)
                    {
                        board[fil, col] = 0;
                    }
                }
                if (esSol == false)
                {
                    boardVisualizer[fil, col] = 0;
                }
                col += 1;
            }

        }
        return Tuple.Create(board, esSol);
    }

    public void playFunc()
    {
        if (firstTime && gameObject.active)
        {
            playButton.sprite = Resources.Load<Sprite>("pause");
            firstTime = false;
            tamaño = GameState.gameState.tamaño;
            int[,] board = new int[tamaño, tamaño];
            int[,] boardVisualizer = new int[tamaño, tamaño];
            boardHistory = new List<DataHistory>();
            vectorSolucion = new List<int>();
            NReinas(board, 0, boardVisualizer, vectorSolucion);
            boardSolved = true;
        }
        else if (play && this.gameObject.activeSelf)
        {
            playButton.sprite = Resources.Load<Sprite>("play");
            play = false;
            StopAllCoroutines();
        }
        else if (!play && this.gameObject.activeSelf)
        {
            playButton.sprite = Resources.Load<Sprite>("pause");
            play = true;
            recuperarEstadoCorutina = true;
        }

    }

    public void nextStepFunc()
    {
        if (!play && gameObject.active)
        {
            if (i < boardHistory.Count - 1)
            {
                nextStep = true;
                if (fase >= 13 && boardHistory[i].colOk && boardHistory[i].diag1Ok && boardHistory[i].diag2Ok)
                {
                    fase = 2;

                    foreach (TextMeshProUGUI t in pilaEjecucion[i].pilaPintado)
                    {
                        t.color = Color.white;
                    }

                    i += 1;
                    List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                    List<int> fasesAsociadas = new List<int>();
                    TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                    pilaEjecucion.Add(tableroEjecucion);
                }
                else if (fase >= 12 && !(boardHistory[i].colOk && boardHistory[i].diag1Ok && boardHistory[i].diag2Ok))
                {
                    fase = 7;
                    
                    foreach (TextMeshProUGUI t in pilaEjecucion[i].pilaPintado)
                    {
                        t.color = Color.white;
                    }
                    i += 1;
                    List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                    List<int> fasesAsociadas = new List<int>();
                    TextColor tableroEjecucion = new TextColor(fasesAsociadas, auxiliar);
                    pilaEjecucion.Add(tableroEjecucion);

                }
                else
                {
                    if (pilaEjecucion[i].fases.Count == 0)
                    {
                        fase = 2;
                    }
                    else
                    {
                        int lastObject = 0;
                        foreach (int t in pilaEjecucion[i].fases)
                        {
                            lastObject = t;
                        }
                        fase = lastObject;
                        fase += 1;
                    }
                }
            }
            else if (i == boardHistory.Count - 1 && !finished)
            {
                nextStep = true;
                if (pilaEjecucion[i].fases.Count == 0)
                {
                    fase = 2;
                }
                else if (fase >= 13)
                {
                    fase = 2;
                }
                else
                {
                    int lastObject = 0;
                    foreach (int t in pilaEjecucion[i].fases)
                    {
                        lastObject = t;
                    }
                    fase = lastObject;
                    fase += 1;
                }

            }
        }
    }

    public void backStepFunc()
    {
        if (!play && !moverReina && gameObject.active)
        {
            if (i >= 0)
            {
                backStep= true;
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
                    finished = false;

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
                
            }
        }
    }

}
