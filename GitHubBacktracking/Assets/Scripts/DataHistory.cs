using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHistory 
{
    public int[,] board;
    public int k;
    public int n;

    public DataHistory(int[,]board,int k,int n)
    {
        this.board = board;
        this.k = k;
        this.n = n;
    }
}
