using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu, nReinasMenu,sudokuMenu;
    public Dropdown tamañoTablero;

    public void goNReinasMenu()
    {
        mainMenu.gameObject.SetActive(false);
        nReinasMenu.gameObject.SetActive(true);
        GameState.gameState.tamaño = 4;
    }

    public void goSudokuMenu()
    {
        mainMenu.gameObject.SetActive(false);
        sudokuMenu.gameObject.SetActive(true);
        GameState.gameState.tamaño = 4;
    }

    public void goMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        nReinasMenu.gameObject.SetActive(false);
        sudokuMenu.gameObject.SetActive(false);
    }

    public void goMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void goNReinasProblem(){
        SceneManager.LoadScene("NReinasProblem");
        GameState.gameState.problem = "NReinas";
    }
    public void goSudokuProblem()
    {
        SceneManager.LoadScene("NReinasProblem");
        GameState.gameState.problem = "Sudoku";
    }

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
