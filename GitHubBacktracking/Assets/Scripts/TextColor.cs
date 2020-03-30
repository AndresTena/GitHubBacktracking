using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextColor : MonoBehaviour
{
    public List<int> fases;
    public List<TextMeshProUGUI> pilaPintado;

    public TextColor(List<int> fases, List<TextMeshProUGUI> pilaPintado) {

        this.fases = fases;
        this.pilaPintado = pilaPintado;

    }
}
