using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    [Header("UI Screens")] public GameObject LoginScreen;
    public GameObject InitialScreen;
    public GameObject MainScreen;
    public GameObject GameEndScreen;

    [Header("Game End Screen items")] public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;

    public GameObject MainBackground;
    public GameObject QuestionHolder;

    private DialogManager dialogManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogManager = DialogManager.Instance;
    }

    public void ActivateInitalScreen()
    {
        AudioManager.Instance.StopAudio();
        AudioManager.Instance.PlayAudio(0);
        LoginScreen.SetActive(false);
        MainScreen.SetActive(false);
        InitialScreen.SetActive(true);
    }

    public void ActivateMainScreen()
    {
        AudioManager.Instance.StopAudio();
        InitialScreen.SetActive(false);
        MainBackground.SetActive(true);
        MainScreen.SetActive(true);
        AudioManager.Instance.PlayAudio(1);

        dialogManager.EnableDialogue("Great Job!",
            "You have successfully calibrated the functional aspects of the game.\nPlease select the QR tab on the top right and scan the code located on the door or press the button below to go back if you haven't opened the door!",
            "Oops, we didn't open the door!", true, () =>
            {
                ActivateInitalScreen();
                dialogManager.DisableDialogue();
            });
    }

    public void ActivateGameEndScreen()
    {
        MainGameManager.Instance.GameEnded = true;
        
        MainBackground.SetActive(false);
        MainScreen.SetActive(false);
        GameEndScreen.SetActive(true);

        var span = TimeSpan.FromSeconds(MainGameManager.Instance.ElapsedTime);

        MainGameManager.Instance.NetworkHandlerInstance.SendGameEnd(MainGameManager.Instance.TeamName, AnswerManager.Instance.FinalChoice, ((int)MainGameManager.Instance.ElapsedTime), MainGameManager.Instance.Score);
        
        ScoreText.text = "Your final score is: " + MainGameManager.Instance.Score;
        TimeText.text = "Your final time is: " + Utilities.SecondsToFormattedString((int)MainGameManager.Instance.ElapsedTime);

    }

    public void ActivateQr()
    {
        MainBackground.SetActive(false);
        QuestionHolder.SetActive(false);
        LevelManager.Instance.SelectQr();
    }

    public void DeactivateQr()
    {
        MainBackground.SetActive(true);
        QuestionHolder.SetActive(true);
        LevelManager.Instance.DeselectQr();
    }
}