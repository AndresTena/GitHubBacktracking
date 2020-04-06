using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProblemSelected : MonoBehaviour
{
    public GameObject NReinas, Sudoku, Laberinto;
    // Start is called before the first frame update
    void Start()
    {
        if ("NReinas" == GameState.gameState.problem) {

            NReinas.gameObject.SetActive(true);
            Sudoku.gameObject.SetActive(false);

        }
        else if ("Sudoku" == GameState.gameState.problem)
        {
            Sudoku.gameObject.SetActive(true);
            NReinas.gameObject.SetActive(false);
        }

    }
    
}
