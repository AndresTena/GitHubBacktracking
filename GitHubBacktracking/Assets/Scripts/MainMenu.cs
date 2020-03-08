using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu, nReinasMenu;

    public void goNReinasMenu()
    {
        mainMenu.gameObject.SetActive(false);
        nReinasMenu.gameObject.SetActive(true);
    }

    public void goMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        nReinasMenu.gameObject.SetActive(false);
    }

    public void goMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void go4ReinasProblem(){
        GameState.gameState.tamaño = 4;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go5ReinasProblem()
    {
        GameState.gameState.tamaño = 5;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go6ReinasProblem()
    {
        GameState.gameState.tamaño = 6;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go7ReinasProblem()
    {
        GameState.gameState.tamaño = 7;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go8ReinasProblem()
    {
        GameState.gameState.tamaño = 8;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go9ReinasProblem()
    {
        GameState.gameState.tamaño = 9;
        SceneManager.LoadScene("NReinasProblem");
    }

    public void go10ReinasProblem()
    {
        GameState.gameState.tamaño = 10;
        SceneManager.LoadScene("NReinasProblem");
    }

}
