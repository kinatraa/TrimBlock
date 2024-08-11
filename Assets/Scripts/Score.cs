using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TMP_Text[] textOnScreen;

    void Start()
    {
        for(int i = 0; i < textOnScreen.Length; i++)
        {
            textOnScreen[i].text = "0";
        }
    }

    public void ChangeScore(int[] score)
    {
        for (int i = 0; i < textOnScreen.Length; i++)
        {
            textOnScreen[i].text = score[i].ToString();
        }
    }
}
