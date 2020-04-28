using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SharedCode : MonoBehaviour
{
    //Declaración de variables
    public List<List<TextMeshProUGUI>> piscinaTextos;
    public List<List<string>> piscinaTextosOriginal;
    public List<BoardHistory> pilaEjecucion;

    //Constructor donde se iniciarán las variables donde se guardarán los textos del problema que se este ejecutando en dicho momento.
    public SharedCode() {
        //Referencia a los textos por pantalla
        piscinaTextos = new List<List<TextMeshProUGUI>>();
        //Textos coloreados
        piscinaTextosOriginal = new List<List<string>>();
        pilaEjecucion = new List<BoardHistory>();
    }
    
    //Contiene todos los textos del codigo.
    public void addTextos(List<List<TextMeshProUGUI>> textos)
    {
        foreach (var texto in textos)
        {
            piscinaTextos.Add(texto);
            List<string> aux = new List<string>();
            aux.Add(texto[0].text);
            aux.Add(texto[1].text);
            piscinaTextosOriginal.Add(aux);
        }
    }

    //Añade el texto al que hemos avanzado a la pila con su correspondiente fase, además de marcar el texto en el que estamos coloreandolo y quitar el color a todos los demás
    public void pila(int fase, int camino, int iteracion, bool backStep, List<List<string>> textosSinPintar, DataHistory board, int problem)
    {
        //Limpiamos el color de todos los textos
        for (int i = 0; i < piscinaTextos.Count; i++)
        {
            for (int j = 0; j < piscinaTextos[i].Count; j++)
            {
                piscinaTextos[i][j].text = piscinaTextosOriginal[i][j];
                piscinaTextos[i][j].color = Color.white;
            }
        }

        //Actualizamos el texto por pantalla y lo coloreamos de azul
        piscinaTextos[fase][camino].text = textosSinPintar[fase][camino];
        piscinaTextos[fase][camino].color = Color.cyan;
        //Dependiento del problema en el que estemos, habrá unas excepciones para pintar u otras.
        switch (problem)
        {
            case 1:
                if (board.colOk && fase == 8)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.colOk && fase == 8)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                if (board.diag1Ok && fase == 9)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.diag1Ok && fase == 9)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                if (board.diag2Ok && fase == 10)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.diag2Ok && fase == 10)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                break;
            case 2:
                if (board.filOk && fase == 13)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.filOk && fase == 13)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }

                if (board.colOk && fase == 14)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.colOk && fase == 14)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                if (board.cuadroOk && fase == 19)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.cuadroOk && fase == 19)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                break;
            case 3:
                if (board.esFactible && fase == 9)
                {
                    piscinaTextos[fase][camino].color = Color.green;
                }
                else if (!board.esFactible && fase == 9)
                {
                    piscinaTextos[fase][camino].color = Color.red;
                }
                break;
        }

        //Solo se añaden textos a la pila cuando se dan pasos hacia delante.
        if (backStep == false)
        {
            pilaEjecucion[iteracion].fases.Add(fase);
            pilaEjecucion[iteracion].pilaPintado.Add(piscinaTextos[fase][camino]);
        }

    }

    //Mediante este metodo se avanzaa la fase de pintado, y devuelve cual es el siguiente paso.Bien sea avanzar en la ejecucion de la funcion o realizar una llamada recursiva.
    public Tuple<bool, bool, int> waitForNextText(bool lastMove, int fase, bool nextText, bool nextStep, DataHistory board, int problem)
    {
        //En cada problema la llamada recursiva se hace en una fase del pintado diferente, por tanto hay un caso para cada problema.
        if (!(lastMove && fase == 3))
        {
            switch (problem)
            {
                case 1:
                    if (!(board.colOk && board.diag1Ok && board.diag2Ok) && fase == 12)
                    {
                        fase = 5;
                        nextStep = true;
                    }
                    else if (fase == 13)
                    {
                        fase = 0;
                        nextStep = true;
                    }
                    break;
                case 2:
                    if (board.numeroBase && fase == 8)
                    {
                        fase = 0;
                        nextStep = true;
                    }
                    else if (!(board.colOk && board.filOk && board.cuadroOk) && fase == 21)
                    {
                        fase = 10;
                        nextStep = true;
                    }
                    else if (fase == 22)
                    {
                        fase = 0;
                        nextStep = true;
                    }
                    break;
                case 3:
                    if (!board.esFactible && fase == 10)
                    {
                        fase = 6;
                        nextStep = true;
                    }
                    else if (fase == 11)
                    {
                        fase = 0;
                        nextStep = true;
                    }
                    break;
            }

            if (!nextStep)
            {
                nextText = true;
            }
        }
        //Esto devolvera si bien hay que hacer una llamada recursiva, es decir nextStep=true, o bien simplemente hay que avanzar el texto de la llamada en la que estamos.
        return Tuple.Create(nextStep, nextText, fase);
    }

    //Metodo que pausa y reanuda la ejecució del codigo.
    public Tuple<bool, bool, bool, bool, bool> playFunc(bool play, bool backStep, bool firstTime, bool nextStep, bool recuperarEstadoCorutina, Image playButton, bool nextText)
    {
        if (play)
        {
            playButton.sprite = Resources.Load<Sprite>("play");
            play = false;
        }
        else if (!play)
        {
            playButton.sprite = Resources.Load<Sprite>("pause");
            play = true;
            backStep = false;
            firstTime = false;
            nextText = false;
            if (!nextStep)
            {
                recuperarEstadoCorutina = true;
            }
        }

        return Tuple.Create(play, backStep, firstTime, recuperarEstadoCorutina, nextText);
    }

    //Médotodo que desapila los textos y los elimina.
    public Tuple<bool, int, int> backStepFunc(bool backStep, int fase, int i)
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
                //Borramos el ultimo texto de la pila de pintado
                pilaEjecucion[i].pilaPintado.Remove(ultimoObjetoText);

                int ultimoObjetoInt = 0;
                foreach (int t in pilaEjecucion[i].fases)
                {
                    ultimoObjetoInt = t;
                }
                //También borramos su correspondiente fase
                pilaEjecucion[i].fases.Remove(ultimoObjetoInt);

                //Finalmente sacamos la fase de pintado del nuevo texto de la pila para pintarlo
                int lastObject = 0;
                foreach (int t in pilaEjecucion[i].fases)
                {
                    lastObject = t;
                }
                fase = lastObject;

            }
            else
            {
                if (i > 0)
                {
                    //Aqui tenemos un caso excepcional, ya que estamos ante el procedimiento inverso de una llamada recursiva y por cada llamada recursiva hay una pila de ejecucion, por tanto ademas de borrar el texto y su fase tambien hay que borrar la pila de ejecucion que queda vacia.
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
                    pilaEjecucion.RemoveAt(i);

                    i -= 1;

                    int lastObject = 0;
                    foreach (int t in pilaEjecucion[i].fases)
                    {
                        lastObject = t;
                    }
                    fase = lastObject;

                }
            }

        }

        return Tuple.Create(backStep, fase, i);
    }

}
