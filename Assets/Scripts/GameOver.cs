using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public Score score;
    void Start()
    {
        score.ChangeScore(Board.scoreArray);
    }
}
