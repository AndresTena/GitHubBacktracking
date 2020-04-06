using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SharedCode : MonoBehaviour
{
    public List<TextMeshProUGUI> piscinaTextos;
    public List<string> piscinaTextosOriginal;

    public SharedCode() {
        piscinaTextos = new List<TextMeshProUGUI>();
        piscinaTextosOriginal = new List<string>();
    }

    //Contiene todos los textos del codigo.
    public void addTextos(List<TextMeshProUGUI> textos)
    {
        foreach (var texto in textos) {
            piscinaTextos.Add(texto);
            piscinaTextosOriginal.Add(texto.text);
        }
    }
}
