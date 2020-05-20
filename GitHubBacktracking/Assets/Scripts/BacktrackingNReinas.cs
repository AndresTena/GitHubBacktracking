using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BacktrackingNReinas : MonoBehaviour
{
    //Declaración de variables públicas.
    public int tamaño = 4;
    public Text vectorSolucionText;
    public Image playButton;
    public TextMeshProUGUI esFactibleText, colText, diag1Text, diag2Text, defNReinasText, ifKText, esSolTrueText, elseText, esSolFalseText, col0Text, whileNotEsSolText, IfEsFactibleText, defEsFactibleFunc, defEsFactibleText, returnEsFactibleText, tab1Text, NReinasRecursiveText, col1Text, returnDefReinasText;
    public TextMeshProUGUI filActualText, colActualText, esSolActualText, numActualText, progresoText;
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
    public Slider slider;


    //Declaración de variables privadas.
    List<List<TextMeshProUGUI>> textos;
    List<List<string>> textosSinPintar;
    bool boardSolved = false;
    bool firstTime = true;
    bool play = false;
    bool nextStep = true;
    bool nextText = false;
    bool nextStep2 = false;
    bool backStep = false;
    bool nextN = false;
    bool recuperarEstadoCorutina = false;
    bool finished = false;
    bool moverReina = false;
    DataHistory data;
    SharedCode menu;
    int problem = 1;
    int i = -1;
    int filaActual = 0;
    int colActual = 0;
    int fase = 0;
    int filAnt;
    float movementSpeed = 50f;
    Vector3 towardsTarget;
    float posicionXInicial;
    List<int> vectorSolucion;
    List<TextMeshProUGUI> piscinaTextos;
    List<string> piscinaTextosOriginal;
    
    void Start()
    {
        //Desactivamos el texto de num actual, ya que en este problema no se utiliza.
        numActualText.gameObject.SetActive(false);
        //Establecemos el tamaño elegido en el menu.
        tamaño = GameState.gameState.tamaño;
        //Inicializamos variables.
        piscinaTextos = new List<TextMeshProUGUI>();
        piscinaTextosOriginal = new List<string>();
        boardHistory = new List<DataHistory>();
        textos = new List<List<TextMeshProUGUI>>();
        textosSinPintar = new List<List<string>>();
        menu = new SharedCode();
        vectorSolucion = new List<int>();
        inicializarTextos();
        //Inicializacion de las piezas del tablero
        piezasTablero = new GameObject[tamaño, tamaño];
        int[,] board = new int[tamaño, tamaño];
        int[,] boardVisualizer = new int[tamaño, tamaño];
        //Resolvemos el problema y guardamos cada uno de los tableros en una lista.
        NReinas(board, 0, boardVisualizer, vectorSolucion);

        //Guardamos la referencia visual de cada una de las piezas del tablero en piezasTablero, para poder acceder a ellas.
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
            piezasTablero[indiceX, indiceY] = objetoTablero[tamaño - 4].transform.GetChild(i).gameObject;
            indiceY += 1;

        }
        //Inicializacion de las reinas del tablero.
        reinas = new GameObject[tamaño];
        for (int j = 0; j < tamaño; j++)
        {
            reinas[j] = objetoTablero[tamaño - 4].transform.GetChild(i).gameObject;
            i += 1;
        }

        posicionXInicial = posicionesIniciales[tamaño - 4].gameObject.transform.position.x;
        //Activamos el tablero correspondiente
        objetoTablero[tamaño - 4].gameObject.SetActive(true);

        printInitialBoard();
    }
    
    void Update()
    {
        slider.value = (float)i / (float)(boardHistory.Count - 1);
        progresoText.text = "Paso "+ (i + 1) +" / "+ boardHistory.Count;
        //Si una reina se esta moviendo, la ejecucion se espera a que finalize.
        if (moverReina)
        {
            moverReinas();
        }
        else if (play)
        {
            //Cuando una llamada recursiva se de.
            if (nextStep && i < boardHistory.Count - 1)
            {
                nextStep = false;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añadimos una nueva pila de ejecución para esta llamada recursiva.
                menu.pilaEjecucion.Add(tableroEjecucion);
                //Empezamos una corutina.
                StartCoroutine(waitForNextText(boardHistory[i], boardHistory.Count - 1 == i));
            }
            else if (nextText)
            {
                nextText = false;
                //Actualizamos la fase, es decir avanzamos hacia el siguiente paso. 
                fase += 1;
                //Empezamos una corutina.
                StartCoroutine(waitForNextText(boardHistory[i], boardHistory.Count - 1 == i));

            }
            else if (recuperarEstadoCorutina)
            {
                recuperarEstadoCorutina = false;
                //Retomamos la corutina en la que estabamos antes de pausar la ejecucion.
                StartCoroutine(waitForNextText(boardHistory[i], boardHistory.Count - 1 == i));
            }
        }
        //Mostramos el numero de llamadas recursivas hechas asi como la fila y la columna actual del tablero.
        if ((play || backStep || nextStep2  ) && !(boardHistory.Count - 1 == i))
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

    //Actualiza la posicion de la reina en cada iteración del update.
    public void moverReinas()
    {
        towardsTarget = targetPosition.transform.position - reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position;
        reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position += towardsTarget.normalized * movementSpeed * Time.deltaTime;
        float error = 0.03f;
        if (reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position.x > targetPosition.transform.position.x - error && reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position.x < targetPosition.transform.position.x + error)
        {
            reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position = targetPosition.transform.position;
            moverReina = false;
        }
        Debug.DrawLine(reinas[boardHistory[i].vectorSolucion.Count - 1].transform.position, targetPosition.transform.position, Color.green);
    }

    //Este método pintará la fase actual de la ejecucion, además de elegir el camino en caso de encontrarnos en un if o else, en base a los valores guardados en nuestro boardHistory, el cual guarda todos los valores de cada una de las fases del tablero.
    public void faseDePintado(DataHistory boardHistory, int fase, bool lastMove)
    {
        esSolActualText.text = "EsSol actual: " + boardHistory.esSol;
        if (boardHistory.esSol)
        {
            for (int j = 0; j < boardHistory.vectorSolucion.Count - 1; j++)
            {
                llamadasRecursivas[j].gameObject.SetActive(false);
            }
            pieLlamada[0].text = "" + 1;
        }
        int camino = 0;
        switch (fase)
        {
            case 0:
                //Pintamos el primer paso del tablero
                printBoardFirstStep(boardHistory);
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
                    finished = true;
                    camino = 0;
                }
                else
                {
                    camino = 1;
                }
                break;
            case 5:
                printBoardFirstStep(boardHistory);
                break;

            case 6:
                //Desactivamos el esFactible para que no sea visible si es un paso atras.
                if (backStep)
                {
                    defEsFactibleFunc.gameObject.SetActive(false);
                }
                break;

            case 7:
                //Activamos el esFactible para que no sea visible si es un paso adelante.
                printBoardFirstStep(boardHistory);
                defEsFactibleFunc.gameObject.SetActive(true);
                break;

            case 8:
                //Pintamos la columna del tablero, de verde en caso de ser factible y de rojo en caso de no serlo.
                printBoardCol(boardHistory);
                break;

            case 9:
                //Pintamos la diagonal 1 del tablero, de verde en caso de ser factible y de rojo en caso de no serlo.
                printBoardDiag1(boardHistory);
                break;

            case 10:
                //Pintamos la diagonal 2 del tablero, de verde en caso de ser factible y de rojo en caso de no serlo.
                printBoardDiag2(boardHistory);
                break;

            case 11:
                //Volvemos a pintar el tablero como en el primer paso para quitar los colores.
                printBoardFirstStep(boardHistory);
                if (backStep)
                {
                    defEsFactibleFunc.gameObject.SetActive(true);
                }

                break;

            case 12:
                defEsFactibleFunc.gameObject.SetActive(false);
                //Aqui nos encontramos ante el segundo if del problema en el cual tambien tendremos que elegir entre los dos caminos.
                if (boardHistory.colOk && boardHistory.diag1Ok && boardHistory.diag2Ok)
                {
                    camino = 0;
                }
                else
                {
                    camino = 1;
                }

                break;
        }

        menu.pila(fase, camino, i, backStep, textosSinPintar, boardHistory, problem);

    }

    //Este método pintará el siguiente estado del tablero y esperara un numero de segundos.
    IEnumerator waitForNextText(DataHistory boardHistory, bool lastMove)
    {
        faseDePintado(boardHistory, fase, lastMove);
        yield return new WaitForSeconds(speedNextMove.value);
        var results = menu.waitForNextText(lastMove, fase, nextText, nextStep, boardHistory, problem);
        nextStep = results.Item1;
        nextText = results.Item2;
        fase = results.Item3;
    }

    //Pinta el tablero inicial
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

    //Pinta el primer paso del tablero en cada llamada recursiva indicando en que casilla estamos.
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
                    //Obtenemos la posicion a la que se tiene que mover la reina.
                    targetPosition.transform.position = piezasTablero[i, j].transform.position;
                    moverReina = true;
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

    //Pintamos el tablero pero esta vez pintando la columna en la que estamos de su color correspondiente.
    public void printBoardCol(DataHistory boardHistory)
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
                    //Obtenemos la posicion a la que se tiene que mover la reina.
                    targetPosition.transform.position = piezasTablero[i, j].transform.position;
                    moverReina = true;
                    if (boardHistory.colOk)
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
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
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
    
    //Pintamos el tablero pero esta vez pintando la diagonal1 en la que estamos de su color correspondiente.
    public void printBoardDiag1(DataHistory boardHistory)
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
                    if (boardHistory.diag1Ok)
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
                    if (boardHistory.diag1Ok && printDiag1(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.green;
                        tablero.text += "<color=green>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else if (boardHistory.diag1Ok == false && printDiag1(boardHistory.fil, boardHistory.col, i, j))
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.red;
                        tablero.text += "<color=red>" + boardHistory.board[i, j] + "</color> ";
                    }
                    else
                    {
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
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

    //Pintamos el tablero pero esta vez pintando la diagonal2 en la que estamos de su color correspondiente.
    public void printBoardDiag2(DataHistory boardHistory)
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
                    if (boardHistory.diag2Ok)
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
                    if (boardHistory.diag2Ok && printDiag2(boardHistory.fil, boardHistory.col, i, j))
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
                        piezasTablero[i, j].GetComponent<Image>().color = Color.white;
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

    //Metodo que nos indica si la casilla que estamos analizando pertenece a la columna en la que estamos o no.
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

    //Metodo que nos indica si la casilla que estamos analizando pertenece a la diagoanl en la que estamos o no.
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

    //Metodo que nos indica si la casilla que estamos analizando pertenece a la diagoanl en la que estamos o no.
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

    //Metodo esfactible para resolver el problema
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

    //Codigo NReinas.
    public Tuple<int[,], bool> NReinas(int[,] board, int fil, int[,] boardVisualizer, List<int> vectorSolucion)
    {
        bool esSol;
        if (fil >= tamaño)
        {
            esSol = true;
            int[,] aux = new int[tamaño, tamaño];
            for (int i = 0; i < tamaño; i++)
            {
                for (int j = 0; j < tamaño; j++)
                {
                    aux[i, j] = boardVisualizer[i, j];
                }
            }
            var factibles = EsFactible(board, fil, tamaño - 1);
            data = new DataHistory(aux, fil, tamaño - 1, factibles.Item1, factibles.Item2, factibles.Item3, esSol);
            //Guardamos el ultimo tablero y lo añadimos a la lista.
            boardHistory.Add(data);
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
                //Añadimos cada uno de los tableros a la lista, con su correspondientes variables.
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

    //Inicializa y guarda referencia a todos los textos del codigo, tanto los pintados como los que no lo están.
    public void inicializarTextos()
    {
        List<TextMeshProUGUI> aux1 = new List<TextMeshProUGUI>();
        aux1.Add(defNReinasText);
        aux1.Add(defNReinasText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(ifKText);
        aux1.Add(ifKText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(esSolTrueText);
        aux1.Add(elseText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(returnDefReinasText);
        aux1.Add(esSolFalseText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(col0Text);
        aux1.Add(col0Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(whileNotEsSolText);
        aux1.Add(whileNotEsSolText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(IfEsFactibleText);
        aux1.Add(IfEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(defEsFactibleText);
        aux1.Add(defEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(colText);
        aux1.Add(colText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(diag1Text);
        aux1.Add(diag1Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(diag2Text);
        aux1.Add(diag2Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(returnEsFactibleText);
        aux1.Add(returnEsFactibleText);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(tab1Text);
        aux1.Add(col1Text);
        textos.Add(aux1);
        aux1 = new List<TextMeshProUGUI>();
        aux1.Add(NReinasRecursiveText);
        aux1.Add(NReinasRecursiveText);
        textos.Add(aux1);

        menu.addTextos(textos);

        List<string> aux = new List<string>();
        aux.Add("def NReinasVA(tab, fil):");
        aux.Add("def NReinasVA(tab, fil):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if fil == np.size(tab, 0):");
        aux.Add("if fil == np.size(tab, 0):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("esSol = True");
        aux.Add("else:");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("return tab, esSol");
        aux.Add("esSol = False");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("col = 0");
        aux.Add("col = 0");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("while not esSol and col < np.size(tab, 1):");
        aux.Add("while not esSol and col < np.size(tab, 1):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("if EsFactible(tab, fil, col):");
        aux.Add("if EsFactible(tab, fil, col):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("def EsFactible(tab, f, c):");
        aux.Add("def EsFactible(tab, f, c):");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("colOk = np.max(tab[:, c]) == 0 ");
        aux.Add("colOk = np.max(tab[:, c]) == 0 ");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("diag1Ok = True\n" +
                                 "indF = f - 1\n" +
                                 "indC = c - 1\n" +
                                 "while diag1Ok and indF >= 0 and indC >= 0:\n" +
                                 "      diag1Ok = tab[indF, indC] == 0\n" +
                                 "      indF -= 1\n" +
                                 "      indC -= 1\n");
        aux.Add("diag1Ok = True\n" +
                                 "indF = f - 1\n" +
                                 "indC = c - 1\n" +
                                 "while diag1Ok and indF >= 0 and indC >= 0:\n" +
                                 "      diag1Ok = tab[indF, indC] == 0\n" +
                                 "      indF -= 1\n" +
                                 "      indC -= 1\n");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("diag2Ok = True\n" +
                                 "indF = f - 1\n" +
                                 "indC = c + 1\n" +
                                 "while diag2Ok and indF >= 0 and indC < np.size(tab,1):\n" +
                                 "      diag2Ok = tab[indF, indC] == 0\n" +
                                 "      indF -= 1\n" +
                                 "      indC += 1\n");
        aux.Add("diag2Ok = True\n" +
                                 "indF = f - 1\n" +
                                 "indC = c + 1\n" +
                                 "while diag2Ok and indF >= 0 and indC < np.size(tab,1):\n" +
                                 "      diag2Ok = tab[indF, indC] == 0\n" +
                                 "      indF -= 1\n" +
                                 "      indC += 1\n");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("return colOk and diag1Ok and diag2Ok");
        aux.Add("return colOk and diag1Ok and diag2Ok");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("tab[fil][col] = 1");
        aux.Add("col += 1");
        textosSinPintar.Add(aux);
        aux = new List<string>();
        aux.Add("[tab, esSol] = NReinasVA(tab, fil + 1)");
        aux.Add("[tab, esSol] = NReinasVA(tab, fil + 1)");
        textosSinPintar.Add(aux);
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
        if (!play && gameObject.active && !firstTime && !moverReina)
        {
            backStep = false;
            nextStep2 = true;
            if (fase == 12 && !(boardHistory[i].diag1Ok && boardHistory[i].diag2Ok && boardHistory[i].colOk))
            {
                fase = 5;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
            }
            else if (fase == 13)
            {
                fase = 0;
                i += 1;
                List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                List<int> fasesAsociadas = new List<int>();
                BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                //Añade una nueva pila de ejecucion
                menu.pilaEjecucion.Add(tableroEjecucion);
                faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
            }
            else if (fase == 3 && boardHistory.Count - 1 == i)
            {
                //Ejecucion terminada;
            }
            else
            {
                fase += 1;
                if (i != 0)
                {
                    faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                }
                else if (i < 0)
                {
                    i += 1;
                    List<TextMeshProUGUI> auxiliar = new List<TextMeshProUGUI>();
                    List<int> fasesAsociadas = new List<int>();
                    BoardHistory tableroEjecucion = new BoardHistory(fasesAsociadas, auxiliar);
                    faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
                }
                else
                {
                    faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
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
                faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
            }
            else
            {
                faseDePintado(boardHistory[i], fase, boardHistory.Count - 1 == i);
            }

        }
    }

}
