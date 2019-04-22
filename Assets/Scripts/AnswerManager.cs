using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;
    
    [HideInInspector] public int Score;

    private void Awake()
    {
        Instance = this;
    }

    public void AnswerQuestion(bool isCorrect)
    {
        Score = isCorrect ? Score + 100 : Score - 200;
        //Send Packet
    }
    
    
}
