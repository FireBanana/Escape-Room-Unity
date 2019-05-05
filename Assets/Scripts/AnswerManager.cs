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

        if (isCorrect)
        {
            DialogManager.Instance.EnableDialogue("Congratulations!", "You got 100 points", "OK", DialogManager.Instance.DisableDialogue);
        }
        else
        {
            DialogManager.Instance.EnableDialogue("Wrong!", "200 points have been deducted", "OK", DialogManager.Instance.DisableDialogue);
        }
        
        MainGameManager.Instance.Score = Score;
        MainGameManager.Instance.NetworkHandlerInstance.SendPointsUpdate(MainGameManager.Instance.TeamName, Score);
    }
    
    
}
