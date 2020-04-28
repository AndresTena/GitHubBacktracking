using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProblemSelected : MonoBehaviour
{
    //Declaración de variables
    public GameObject NReinas, Sudoku, Laberinto;

    void Start()
    {
        //Dependiendo del problema elegido, en la escena se activará ese y los demás se desactivarán.
        if ("NReinas" == GameState.gameState.problem) {

            NReinas.gameObject.SetActive(true);
            Sudoku.gameObject.SetActive(false);
            Laberinto.gameObject.SetActive(false);

        }
        else if ("Sudoku" == GameState.gameState.problem)
        {
            Sudoku.gameObject.SetActive(true);
            NReinas.gameObject.SetActive(false);
            Laberinto.gameObject.SetActive(false);
        }
        else if ("Laberinto" == GameState.gameState.problem)
        {
            Laberinto.gameObject.SetActive(true);
            Sudoku.gameObject.SetActive(false);
            NReinas.gameObject.SetActive(false);
        }

    }
    

}
