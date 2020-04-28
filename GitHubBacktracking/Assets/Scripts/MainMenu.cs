using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //Declaración de variables.
    public GameObject mainMenu, nReinasMenu,sudokuMenu;
    public Dropdown tamañoTablero;

    //Acceder al menu de NReinas.
    public void goNReinasMenu()
    {
        mainMenu.gameObject.SetActive(false);
        nReinasMenu.gameObject.SetActive(true);
        GameState.gameState.tamaño = 4;
    }

    //Acceder al menu de Sudoku.
    public void goSudokuMenu()
    {
        mainMenu.gameObject.SetActive(false);
        sudokuMenu.gameObject.SetActive(true);
        GameState.gameState.tamaño = 4;
    }

    //Acceder al MainMenu
    public void goMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        nReinasMenu.gameObject.SetActive(false);
        sudokuMenu.gameObject.SetActive(false);
    }

    //Cambiar a la escena del menu.
    public void goMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    //Cambiar a la escena del problema NReinas.
    public void goNReinasProblem(){
        SceneManager.LoadScene("NReinasProblem");
        GameState.gameState.problem = "NReinas";
    }

    //Cambiar a la escena del problema Sudoku.
    public void goSudokuProblem()
    {
        SceneManager.LoadScene("NReinasProblem");
        GameState.gameState.problem = "Sudoku";
    }

    //Cambiar a la escena del problema Laberinto.
    public void goLaberintoProblem()
    {
        SceneManager.LoadScene("NReinasProblem");
        GameState.gameState.problem = "Laberinto";
    }

    //Elige el tamaño del tablero de las NReinas.
    public void defineWidhtNReinas(int val)
    {
        if (val == 0) {
            GameState.gameState.tamaño = 4;
        }
        if (val == 1)
        {
            GameState.gameState.tamaño = 5;
        }
        if (val == 2)
        {
            GameState.gameState.tamaño = 6;
        }
        if (val == 3)
        {
            GameState.gameState.tamaño = 7;
        }
        if (val == 4)
        {
            GameState.gameState.tamaño = 8;
        }
        if (val == 5)
        {
            GameState.gameState.tamaño = 9;
        }
    }

    //Elige el tamaño del tablero del sudoku.
    public void defineWidhtSudoku(int val)
    {
        if (val == 0)
        {
            GameState.gameState.tamaño = 4;
        }
        if (val == 1)
        {
            GameState.gameState.tamaño = 9;
        }
    }
}
