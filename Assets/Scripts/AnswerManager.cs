using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;
    private bool finalQuestionAnswered;

    public bool FinalQuestionAnswered
    {
        get { return finalQuestionAnswered; }
    }

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
            DialogManager.Instance.EnableDialogue("Congratulations!", "You got 100 points", "OK", true, DialogManager.Instance.DisableDialogue);
        }
        else
        {
            DialogManager.Instance.EnableDialogue("Wrong!", "200 points have been deducted", "OK", true, DialogManager.Instance.DisableDialogue);
        }
        
        MainGameManager.Instance.Score = Score;
        MainGameManager.Instance.NetworkHandlerInstance.SendPointsUpdate(MainGameManager.Instance.TeamName, Score);
    }

    public void AnswerFinalQuestion()
    {
        finalQuestionAnswered = true;
        if (QrManager.Instance.IsExitCodeScanned())
        {
            DialogManager.Instance.EnableDialogue("Simulation Terminated", "You have made your final decision and successfully unlocked the escape door. Please exit the simulation area and await final instructions.",
                "OK", false, () =>
                {
                    //Open Final Screen
                    DialogManager.Instance.DisableDialogue();
                });
        }
        else
        {
            DialogManager.Instance.EnableDialogue("Enter the exit code", "You have made your final decision, but you still need to enter the 5-digit exit code, then scan the QR above it to successfully terminate the simulation",
                "OK", true,() =>
                {
                    DialogManager.Instance.DisableDialogue();
                });
        }
    }
    
    
}
