﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;
    private bool finalQuestionAnswered;

    public GameObject FinalQuestion, FinalBody;


    public int TotalQuestions;
    [HideInInspector] public string FinalChoice;
    int questionCounter = 0;

    private List<int> usedButtonIds = new List<int>();
    
    public bool FinalQuestionAnswered
    {
        get { return finalQuestionAnswered; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void AnswerQuestion(AnswerButton ab)
    {
        if (usedButtonIds.Contains(ab.Id))
        {
            DialogManager.Instance.EnableDialogue("Already Answered", "This question was already answered and cannot be answered again", "OK", true, DialogManager.Instance.DisableDialogue);
            return;
        }

        if(ab.IsCorrect)
            usedButtonIds.Add(ab.Id);
        
        var Score = MainGameManager.Instance.Score;
        Score = ab.IsCorrect ? Score + 100 : Score - 200;

        if (ab.IsCorrect)
        {
            questionCounter++;
            ab.QuestionHeader.transform.Find("QuestionCheck").gameObject.SetActive(true);
            DialogManager.Instance.EnableDialogue("Congratulations!", "You got 100 points", "OK", true, DialogManager.Instance.DisableDialogue);
        }
        else
        {
            DialogManager.Instance.EnableDialogue("Wrong!", "200 points have been deducted", "OK", true, DialogManager.Instance.DisableDialogue);
        }
        
        MainGameManager.Instance.Score = Score;
        MainGameManager.Instance.NetworkHandlerInstance.SendPointsUpdate(MainGameManager.Instance.TeamName, ab.IsCorrect ? 100 : -200, false);

        if(HasAnsweredAll())
        {
            FinalQuestion.SetActive(true);
            FinalBody.SetActive(true);
        }
    }
    
    public bool HasAnsweredAll()
    {
        return questionCounter == TotalQuestions ? true : false;
    }

    public void AnswerFinalQuestion(string value)
    {
        finalQuestionAnswered = true;
        FinalChoice = value;
        DialogManager.Instance.DisableDialogue();
        if (QrManager.Instance.IsExitCodeScanned())
        {
            AudioManager.Instance.PlayAudio(9);
            DialogManager.Instance.EnableDialogue("Simulation Terminated", "You have made your final decision and successfully unlocked the escape door. Please exit the simulation area and await final instructions.",
                "OK", false, () =>
                {
                    NavigationManager.Instance.ActivateGameEndScreen();
                    DialogManager.Instance.DisableDialogue();
                });
        }
        else
        {
            AudioManager.Instance.PlayAudio(9);
            DialogManager.Instance.EnableDialogue("Enter the exit code", "You have made your final decision, but you still need to enter the 5-digit exit code, then scan the QR above it to successfully terminate the simulation",
                "OK", true,() =>
                {
                    DialogManager.Instance.DisableDialogue();
                });
        }
    }
    
    
}
