using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ex1 : MonoBehaviour
{
    int vida = 10;
    int dano = 5;

    void Start()
    {
        int vidaRestante = vida - dano;
        Debug.Log(vidaRestante);
        Debug.Log(LessThanOrEqualToZero(vidaRestante));
    }
    bool LessThanOrEqualToZero(int number)
    {
        if (number >= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
