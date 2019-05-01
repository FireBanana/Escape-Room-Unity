using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AnswerQuestion(bool isCorrect)
    {
        var Score = MainGameManager.Instance.Score;
        Score = isCorrect ? Score + 100 : Score - 200;
        MainGameManager.Instance.Score = Score;
        MainGameManager.Instance.NetworkHandlerInstance.SendPointsUpdate(MainGameManager.Instance.TeamName, Score);
    }
    
    
}
