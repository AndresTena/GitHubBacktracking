using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardHistory : MonoBehaviour
{
    //Declaración de variables
    public List<int> fases;
    public List<TextMeshProUGUI> pilaPintado;

    public BoardHistory(List<int> fases, List<TextMeshProUGUI> pilaPintado) {
        //Objeto que va a contener los textos pintados con su correspondiente fase.
        this.fases = fases;
        this.pilaPintado = pilaPintado;

    }
}
