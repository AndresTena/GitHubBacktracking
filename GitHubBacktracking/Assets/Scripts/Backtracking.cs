using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Backtracking : MonoBehaviour
{
    public int NReinas = 4;
    public Text vectorSolucion;
    public Text tablero;
    public Text NReinasText;
    public Button play;

    public void printBoard(int[,] board)
    {
        for (int i = 0; i < NReinas; i++)
        {
            for (int j = 0; j < NReinas; j++)
            {
                tablero.text += board[i, j] + " ";
                if (board[i, j] == 1)
                {
                    vectorSolucion.text += j + " ";
                }
            }
            tablero.text+="\n";
        }
    }
    public bool toPlaceOrNotToPlace(int[,] board, int row, int col)
    {
        int i, j;
        for (i = 0; i < col; i++)
        {
            if (board[row, i] == 1) return false;
        }
        for (i = row, j = col; i >= 0 && j >= 0; i--, j--)
        {
            if (board[i, j] == 1) return false;
        }
        for (i = row, j = col; j >= 0 && i < NReinas; i++, j--)
        {
            if (board[i, j] == 1) return false;
        }
        return true;
    }
    public bool theBoardSolver(int[,] board, int col)
    {
        if (col >= NReinas) return true;
        for (int i = 0; i < NReinas; i++)
        {
            tablero.text = "";
            vectorSolucion.text = "";
                printBoard(board);
                if (toPlaceOrNotToPlace(board, i, col))
                {
                    board[i, col] = 1;
                    if (theBoardSolver(board, col + 1)) return true;
                    // Backtracking is hella important in this one.  
                    board[i, col] = 0;
                }
            
        }
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resolverTablero()
    {
        NReinas = int.Parse(NReinasText.text);

        int[,] board = new int[NReinas, NReinas];
        if (!theBoardSolver(board, 0))
        {
            Debug.Log("Solution not found.");
        }
        tablero.text = "";
        vectorSolucion.text = "";
        printBoard(board);
    } 

}