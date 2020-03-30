using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState gameState;
    public int tamaño = 4;
    public string problem = "";

    public void Awake()
    {
        if (gameState == null)
        {
            gameState = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (gameState != this)
        {
            Destroy(gameObject);
        }
    }
}
